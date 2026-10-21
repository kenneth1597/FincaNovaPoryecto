using FincaNovaGestionLotes.Web.Areas.Terrenos.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Terrenos;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Terrenos.Controllers;

[Area("Terrenos")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class TerrenosController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public TerrenosController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    private string? NombreUsuario => HttpContext.Session.GetString("UsuarioNombre");

    [HttpGet]
    public async Task<IActionResult> Index(TerrenoFiltroViewModel filtro)
    {
        var baseQuery = _db.Terrenos.AsNoTracking().Where(t => !t.Eliminado);

        filtro.Conteos = await baseQuery
            .GroupBy(t => t.Estado)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var query = baseQuery;
        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var q = filtro.Q.Trim();
            query = query.Where(t => t.Codigo.Contains(q) || t.Nombre.Contains(q) ||
                                     (t.Ubicacion != null && t.Ubicacion.Contains(q)));
        }
        if (!string.IsNullOrWhiteSpace(filtro.Ubicacion))
        {
            var u = filtro.Ubicacion.Trim();
            query = query.Where(t => t.Ubicacion != null && t.Ubicacion.Contains(u));
        }
        if (filtro.Tipo is { } tipo) query = query.Where(t => t.Tipo == tipo);
        if (filtro.Estado is { } estado) query = query.Where(t => t.Estado == estado);

        var (resultados, paginacion) = await query
            .OrderBy(t => t.FincaPadreId == null ? t.Codigo : t.FincaPadre!.Codigo)
            .ThenBy(t => t.Tipo)
            .ThenBy(t => t.Codigo)
            .Select(t => new TerrenoListItemViewModel
            {
                Id = t.Id,
                Codigo = t.Codigo,
                Nombre = t.Nombre,
                Tipo = t.Tipo,
                FincaPadre = t.FincaPadre != null ? t.FincaPadre.Codigo : null,
                AreaHectareas = t.AreaHectareas,
                AreaAsignada = t.Lotes.Where(l => !l.Eliminado).Sum(l => (decimal?)l.AreaHectareas) ?? 0,
                VariedadCafe = t.VariedadCafe,
                AnioSiembra = t.AnioSiembra,
                Ubicacion = t.Ubicacion,
                Estado = t.Estado,
                Lotes = t.Lotes.Count(l => !l.Eliminado)
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;

        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var terreno = await _db.Terrenos.AsNoTracking()
            .Include(t => t.FincaPadre)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        if (terreno is null) return NotFound();

        var lotes = await _db.Terrenos.AsNoTracking()
            .Where(t => t.FincaPadreId == id && !t.Eliminado)
            .OrderBy(t => t.Codigo)
            .Select(t => new TerrenoListItemViewModel
            {
                Id = t.Id,
                Codigo = t.Codigo,
                Nombre = t.Nombre,
                Tipo = t.Tipo,
                AreaHectareas = t.AreaHectareas,
                VariedadCafe = t.VariedadCafe,
                AnioSiembra = t.AnioSiembra,
                Ubicacion = t.Ubicacion,
                Estado = t.Estado
            })
            .ToListAsync();

        var labores = await _db.Labores.AsNoTracking()
            .Where(l => l.TerrenoId == id)
            .OrderByDescending(l => l.Fecha)
            .ThenByDescending(l => l.Id)
            .Select(l => new LaborBitacoraViewModel
            {
                Id = l.Id,
                Fecha = l.Fecha,
                Tipo = l.Tipo,
                HorasTotales = l.HorasTotales,
                Estado = l.Estado,
                Observaciones = l.Observaciones,
                Colaboradores = l.Colaboradores.Select(c => c.Colaborador.Nombre).ToList()
            })
            .ToListAsync();

        return View(new TerrenoDetalleViewModel
        {
            Terreno = terreno,
            Lotes = lotes,
            AreaAsignada = lotes.Sum(l => l.AreaHectareas),
            Labores = labores
        });
    }

    [HttpGet]
    public async Task<IActionResult> Crear(TipoTerreno tipo = TipoTerreno.Finca, int? padreId = null)
    {
        var fincas = await FincasPadreAsync();

        if (tipo == TipoTerreno.Lote && !fincas.Any())
        {
            TempData["Error"] = "Primero registre una finca activa: todo lote debe pertenecer a una finca.";
            return RedirectToAction(nameof(Crear), new { tipo = TipoTerreno.Finca });
        }

        return View(new TerrenoFormViewModel
        {
            Tipo = tipo,
            FincaPadreId = padreId,
            FincasPadre = fincas
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(TerrenoFormViewModel model)
    {
        model.FincasPadre = await FincasPadreAsync();
        var empresaId = await _db.Empresas.Select(e => (int?)e.Id).FirstOrDefaultAsync();

        if (!Enum.IsDefined(model.Tipo))
            ModelState.AddModelError(nameof(model.Tipo), "El tipo no es válido.");

        await ValidarCodigoAsync(model, empresaId);

        if (model.Tipo == TipoTerreno.Lote)
            await ValidarFincaPadreAsync(model, empresaId);

        if (!ModelState.IsValid)
            return View(model);

        var terreno = new Terreno
        {
            EmpresaId = empresaId!.Value,
            Codigo = model.Codigo.Trim(),
            Nombre = model.Nombre.Trim(),
            Tipo = model.Tipo,
            FincaPadreId = model.Tipo == TipoTerreno.Lote ? model.FincaPadreId : null,
            AreaHectareas = model.AreaHectareas,
            VariedadCafe = model.VariedadCafe?.Trim(),
            AnioSiembra = model.AnioSiembra,
            Ubicacion = model.Ubicacion?.Trim(),
            Estado = EstadoTerreno.Activo,
            FechaUltimoCambioEstado = DateTime.UtcNow,
            EstadoCambiadoPor = NombreUsuario,
            Observaciones = model.Observaciones?.Trim(),
            CreadoPor = NombreUsuario
        };

        _db.Terrenos.Add(terreno);
        await _db.SaveChangesAsync();

        var nombreTipo = Ui.TipoTerrenoTexto(terreno.Tipo);
        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Terreno), terreno.Id.ToString(),
            $"Registro de {nombreTipo.ToLower()} {terreno.Codigo}");

        TempData["Ok"] = $"{nombreTipo} «{terreno.Codigo}» registrad{Ui.TerminacionGenero(terreno.Tipo)} correctamente.";
        return RedirectToAction(nameof(Detalle), new { id = terreno.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var terreno = await _db.Terrenos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        if (terreno is null) return NotFound();

        return View(new TerrenoFormViewModel
        {
            Id = terreno.Id,
            Codigo = terreno.Codigo,
            Nombre = terreno.Nombre,
            Tipo = terreno.Tipo,
            FincaPadreId = terreno.FincaPadreId,
            AreaHectareas = terreno.AreaHectareas,
            VariedadCafe = terreno.VariedadCafe,
            AnioSiembra = terreno.AnioSiembra,
            Ubicacion = terreno.Ubicacion,
            Observaciones = terreno.Observaciones,
            FincasPadre = await FincasPadreAsync(exceptoId: terreno.Id, incluirId: terreno.FincaPadreId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(TerrenoFormViewModel model)
    {
        var terreno = await _db.Terrenos.FirstOrDefaultAsync(t => t.Id == model.Id && !t.Eliminado);
        if (terreno is null) return NotFound();

        model.FincasPadre = await FincasPadreAsync(exceptoId: terreno.Id, incluirId: terreno.FincaPadreId);
        model.Codigo = terreno.Codigo;
        model.Tipo = terreno.Tipo;
        model.FincaPadreId = terreno.FincaPadreId;

        if (terreno.Tipo == TipoTerreno.Lote && terreno.FincaPadreId is { } fincaId)
        {
            var finca = await _db.Terrenos.AsNoTracking().FirstAsync(t => t.Id == fincaId);
            var asignadoOtros = await AreaAsignadaAsync(fincaId, excluirLoteId: terreno.Id);
            var disponible = finca.AreaHectareas - asignadoOtros;
            if (model.AreaHectareas > disponible)
                ModelState.AddModelError(nameof(model.AreaHectareas),
                    $"El área supera lo disponible en la finca «{finca.Codigo}»: quedan {Math.Max(0, disponible):N2} ha de {finca.AreaHectareas:N2} ha.");
        }

        if (terreno.Tipo == TipoTerreno.Finca)
        {
            var asignado = await AreaAsignadaAsync(terreno.Id);
            if (model.AreaHectareas < asignado)
                ModelState.AddModelError(nameof(model.AreaHectareas),
                    $"La finca no puede quedar más pequeña que la suma de sus lotes ({asignado:N2} ha asignadas).");
        }

        if (!ModelState.IsValid)
            return View(model);

        terreno.Nombre = model.Nombre.Trim();
        terreno.AreaHectareas = model.AreaHectareas;
        terreno.VariedadCafe = model.VariedadCafe?.Trim();
        terreno.AnioSiembra = model.AnioSiembra;
        terreno.Ubicacion = model.Ubicacion?.Trim();
        terreno.Observaciones = model.Observaciones?.Trim();
        terreno.ModificadoPor = NombreUsuario;

        await _db.SaveChangesAsync();

        var nombreTipo = Ui.TipoTerrenoTexto(terreno.Tipo);
        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Terreno), terreno.Id.ToString(),
            $"Edición de {Ui.TipoTerrenoConArticulo(terreno.Tipo)} {terreno.Codigo}");

        TempData["Ok"] = $"{nombreTipo} «{terreno.Codigo}» actualizad{Ui.TerminacionGenero(terreno.Tipo)}.";
        return RedirectToAction(nameof(Detalle), new { id = terreno.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoTerreno? estado)
    {
        var terreno = await _db.Terrenos.FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        if (terreno is null) return NotFound();

        var conArticulo = Ui.TipoTerrenoConArticulo(terreno.Tipo);

        if (estado is null || !Enum.IsDefined(estado.Value))
        {
            TempData["Error"] = "Debe seleccionar un estado válido.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (terreno.Estado == estado.Value)
        {
            TempData["Error"] = $"{Mayuscula(conArticulo)} ya se encuentra en ese estado.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        terreno.Estado = estado.Value;
        terreno.FechaUltimoCambioEstado = DateTime.UtcNow;
        terreno.EstadoCambiadoPor = NombreUsuario;
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Terreno), terreno.Id.ToString(),
            $"Cambio de estado de {conArticulo} {terreno.Codigo} a {Ui.EstadoTerrenoTexto(estado.Value)}");

        TempData["Ok"] = $"{Mayuscula(conArticulo)} «{terreno.Codigo}» pasó a estado «{Ui.EstadoTerrenoTexto(estado.Value)}».";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var terreno = await _db.Terrenos.FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        if (terreno is null) return NotFound();

        var conArticulo = Ui.TipoTerrenoConArticulo(terreno.Tipo);

        terreno.Estado = EstadoTerreno.Inactivo;
        terreno.FechaUltimoCambioEstado = DateTime.UtcNow;
        terreno.EstadoCambiadoPor = NombreUsuario;
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Inactivar, nameof(Terreno), terreno.Id.ToString(),
            $"Inactivación de {conArticulo} {terreno.Codigo}; su historial se conserva");

        TempData["Ok"] = $"{Mayuscula(conArticulo)} «{terreno.Codigo}» fue desactivad{Ui.TerminacionGenero(terreno.Tipo)}. Su historial se conservó.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var terreno = await _db.Terrenos.FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        if (terreno is null) return NotFound();

        var conArticulo = Ui.TipoTerrenoConArticulo(terreno.Tipo);

        if (await TieneRegistrosRelacionadosAsync(id))
        {
            TempData["Error"] = $"{Mayuscula(conArticulo)} tiene lotes, labores, enfermedades o períodos asociados. Use «Desactivar» para conservar su historial.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        terreno.Eliminado = true;
        terreno.Estado = EstadoTerreno.Inactivo;
        terreno.ModificadoPor = NombreUsuario;
        await _db.SaveChangesAsync();
        await _audit.RegistrarAsync(AccionAuditoria.Eliminar, nameof(Terreno), terreno.Id.ToString(),
            $"Eliminación de {conArticulo} {terreno.Codigo} (sin registros asociados)");

        TempData["Ok"] = $"{Mayuscula(conArticulo)} «{terreno.Codigo}» fue eliminad{Ui.TerminacionGenero(terreno.Tipo)}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarCodigoAsync(TerrenoFormViewModel model, int? empresaId)
    {
        if (empresaId is null)
        {
            ModelState.AddModelError(string.Empty, "No hay una empresa configurada.");
            return;
        }

        if (string.IsNullOrWhiteSpace(model.Codigo))
            return;

        var codigo = model.Codigo.Trim();
        var existe = await _db.Terrenos.AnyAsync(t =>
            t.EmpresaId == empresaId && t.Codigo == codigo && t.Id != model.Id && !t.Eliminado);
        if (existe)
            ModelState.AddModelError(nameof(model.Codigo), "El código ya existe: ya hay una finca o un lote registrado con ese código.");
    }

    private async Task ValidarFincaPadreAsync(TerrenoFormViewModel model, int? empresaId)
    {
        if (model.FincaPadreId is not { } fincaId)
        {
            ModelState.AddModelError(nameof(model.FincaPadreId), "Debe indicar la finca a la que pertenece el lote.");
            return;
        }

        var finca = await _db.Terrenos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == fincaId);

        if (finca is null || finca.Eliminado || finca.EmpresaId != empresaId)
        {
            ModelState.AddModelError(nameof(model.FincaPadreId), "La finca seleccionada no existe.");
            return;
        }

        if (finca.Tipo != TipoTerreno.Finca)
        {
            ModelState.AddModelError(nameof(model.FincaPadreId), "Un lote solo puede pertenecer a una finca, no a otro lote.");
            return;
        }

        if (finca.Estado == EstadoTerreno.Inactivo)
        {
            ModelState.AddModelError(nameof(model.FincaPadreId), "La finca seleccionada está inactiva; actívela antes de agregarle lotes.");
            return;
        }

        var disponible = finca.AreaHectareas - await AreaAsignadaAsync(finca.Id);
        if (model.AreaHectareas > disponible)
            ModelState.AddModelError(nameof(model.AreaHectareas),
                $"El área supera lo disponible en la finca «{finca.Codigo}»: quedan {Math.Max(0, disponible):N2} ha de {finca.AreaHectareas:N2} ha.");
    }

    private async Task<decimal> AreaAsignadaAsync(int fincaId, int? excluirLoteId = null) =>
        await _db.Terrenos
            .Where(t => t.FincaPadreId == fincaId && !t.Eliminado && t.Id != excluirLoteId)
            .SumAsync(t => (decimal?)t.AreaHectareas) ?? 0;

    private async Task<IEnumerable<FincaOpcionViewModel>> FincasPadreAsync(int? exceptoId = null, int? incluirId = null)
    {
        var fincas = await _db.Terrenos.AsNoTracking()
            .Where(t => t.Tipo == TipoTerreno.Finca && !t.Eliminado && t.Id != exceptoId &&
                        (t.Estado != EstadoTerreno.Inactivo || t.Id == incluirId))
            .OrderBy(t => t.Codigo)
            .Select(t => new
            {
                t.Id,
                t.Codigo,
                t.Nombre,
                t.AreaHectareas,
                Asignada = t.Lotes.Where(l => !l.Eliminado).Sum(l => (decimal?)l.AreaHectareas) ?? 0
            })
            .ToListAsync();

        return fincas.Select(f => new FincaOpcionViewModel
        {
            Id = f.Id,
            Texto = $"{f.Codigo} — {f.Nombre}",
            AreaDisponible = Math.Max(0, f.AreaHectareas - f.Asignada)
        }).ToList();
    }

    private async Task<bool> TieneRegistrosRelacionadosAsync(int terrenoId) =>
        await _db.Terrenos.AnyAsync(t => t.FincaPadreId == terrenoId && !t.Eliminado) ||
        await _db.Labores.AnyAsync(l => l.TerrenoId == terrenoId) ||
        await _db.Enfermedades.AnyAsync(e => e.TerrenoId == terrenoId) ||
        await _db.PeriodosProductivos.AnyAsync(p => p.TerrenoId == terrenoId);

    private static string Mayuscula(string texto) =>
        string.IsNullOrEmpty(texto) ? texto : char.ToUpper(texto[0]) + texto[1..];
}
