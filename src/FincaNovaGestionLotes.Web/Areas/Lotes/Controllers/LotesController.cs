using FincaNovaGestionLotes.Web.Areas.Lotes.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Lotes;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Lotes.Controllers;


[Area("Lotes")]
public class LotesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public LotesController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    
    [HttpGet]
    public async Task<IActionResult> Index(LoteFiltroViewModel filtro)
    {
        var baseQuery = _db.Lotes.AsNoTracking().Where(l => !l.Eliminado);

        filtro.Conteos = await baseQuery
            .GroupBy(l => l.Estado)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var query = baseQuery;
        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var t = filtro.Q.Trim();
            query = query.Where(l => l.Codigo.Contains(t) || l.Nombre.Contains(t));
        }
        if (filtro.Tipo is { } tipo) query = query.Where(l => l.Tipo == tipo);
        if (filtro.Estado is { } estado) query = query.Where(l => l.Estado == estado);

        filtro.Resultados = await query
            .OrderBy(l => l.Codigo)
            .Select(l => new LoteListItemViewModel
            {
                Id = l.Id,
                Codigo = l.Codigo,
                Nombre = l.Nombre,
                Tipo = l.Tipo,
                LotePadre = l.LotePadre != null ? l.LotePadre.Codigo : null,
                AreaHectareas = l.AreaHectareas,
                VariedadCafe = l.VariedadCafe,
                AnioSiembra = l.AnioSiembra,
                Estado = l.Estado,
                MicroLotes = l.MicroLotes.Count(m => !m.Eliminado)
            })
            .ToListAsync();

        return View(filtro);
    }

    
    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var lote = await _db.Lotes.AsNoTracking()
            .Include(l => l.LotePadre)
            .FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
        if (lote is null) return NotFound();

        var microLotes = await _db.Lotes.AsNoTracking()
            .Where(l => l.LotePadreId == id && !l.Eliminado)
            .OrderBy(l => l.Codigo)
            .Select(l => new LoteListItemViewModel
            {
                Id = l.Id,
                Codigo = l.Codigo,
                Nombre = l.Nombre,
                Tipo = l.Tipo,
                AreaHectareas = l.AreaHectareas,
                VariedadCafe = l.VariedadCafe,
                AnioSiembra = l.AnioSiembra,
                Estado = l.Estado
            })
            .ToListAsync();

        return View(new LoteDetalleViewModel { Lote = lote, MicroLotes = microLotes });
    }

    // ---------- Registro lote ----------
    [HttpGet]
    public async Task<IActionResult> Crear(TipoLote tipo = TipoLote.Lote, int? padreId = null)
    {
        var vm = new LoteFormViewModel
        {
            Tipo = tipo,
            LotePadreId = padreId,
            LotesPadre = await LotesPadreAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(LoteFormViewModel model)
    {
        model.LotesPadre = await LotesPadreAsync();
        var fincaId = await _db.Fincas.Select(f => (int?)f.Id).FirstOrDefaultAsync();

        await ValidarComunAsync(model, fincaId);

        if (!ModelState.IsValid)
            return View(model);

        var lote = new Lote
        {
            FincaId = fincaId!.Value,
            Codigo = model.Codigo.Trim(),
            Nombre = model.Nombre.Trim(),
            Tipo = model.Tipo,
            LotePadreId = model.Tipo == TipoLote.MicroLote ? model.LotePadreId : null,
            AreaHectareas = model.AreaHectareas,
            VariedadCafe = model.VariedadCafe?.Trim(),
            AnioSiembra = model.AnioSiembra,
            Ubicacion = model.Ubicacion?.Trim(),
            Estado = EstadoLote.Activo,
            FechaUltimoCambioEstado = DateTime.UtcNow,
            EstadoCambiadoPor = null,
            Observaciones = model.Observaciones?.Trim()
        };

        _db.Lotes.Add(lote);
        await _db.SaveChangesAsync();
        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Lote), lote.Id.ToString(),
            $"Registro de {(lote.Tipo == TipoLote.MicroLote ? "micro lote" : "lote")} {lote.Codigo}");

        TempData["Ok"] = $"{(lote.Tipo == TipoLote.MicroLote ? "Micro lote" : "Lote")} «{lote.Codigo}» registrado correctamente.";
        return RedirectToAction(nameof(Detalle), new { id = lote.Id });
    }

   
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var lote = await _db.Lotes.FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
        if (lote is null) return NotFound();

        return View(new LoteFormViewModel
        {
            Id = lote.Id,
            Codigo = lote.Codigo,
            Nombre = lote.Nombre,
            Tipo = lote.Tipo,
            LotePadreId = lote.LotePadreId,
            AreaHectareas = lote.AreaHectareas,
            VariedadCafe = lote.VariedadCafe,
            AnioSiembra = lote.AnioSiembra,
            Ubicacion = lote.Ubicacion,
            Observaciones = lote.Observaciones,
            LotesPadre = await LotesPadreAsync(exceptoId: lote.Id)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(LoteFormViewModel model)
    {
        var lote = await _db.Lotes.FirstOrDefaultAsync(l => l.Id == model.Id && !l.Eliminado);
        if (lote is null) return NotFound();

        model.LotesPadre = await LotesPadreAsync(exceptoId: lote.Id);
        // El código es la llave histórica del lote y no se puede editar (regla de negocio).
        model.Codigo = lote.Codigo;
        model.Tipo = lote.Tipo;

        if (model.AreaHectareas <= 0)
            ModelState.AddModelError(nameof(model.AreaHectareas), "El área debe ser mayor a cero.");
        if (model.AnioSiembra is { } anio && (anio < 1900 || anio > 2100))
            ModelState.AddModelError(nameof(model.AnioSiembra), "El año de siembra no es válido.");

        if (!ModelState.IsValid)
            return View(model);

        lote.Nombre = model.Nombre.Trim();
        lote.AreaHectareas = model.AreaHectareas;
        lote.VariedadCafe = model.VariedadCafe?.Trim();
        lote.AnioSiembra = model.AnioSiembra;
        lote.Ubicacion = model.Ubicacion?.Trim();
        lote.Observaciones = model.Observaciones?.Trim();

        await _db.SaveChangesAsync();
        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Lote), lote.Id.ToString(),
            $"Edición del lote {lote.Codigo}");

        TempData["Ok"] = $"Lote «{lote.Codigo}» actualizado.";
        return RedirectToAction(nameof(Detalle), new { id = lote.Id });
    }

    // ---------- Modificación de estado ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoLote? estado)
    {
        var lote = await _db.Lotes.FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
        if (lote is null) return NotFound();

        if (estado is null || !Enum.IsDefined(estado.Value))
        {
            TempData["Error"] = "Debe seleccionar un estado válido.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (lote.Estado == estado.Value)
        {
            TempData["Error"] = "El lote ya se encuentra en ese estado.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        lote.Estado = estado.Value;
        lote.FechaUltimoCambioEstado = DateTime.UtcNow;
        lote.EstadoCambiadoPor = null;
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Lote), lote.Id.ToString(),
            $"Cambio de estado del lote {lote.Codigo} a {Ui.EstadoLoteTexto(estado.Value)}");

        TempData["Ok"] = $"El lote «{lote.Codigo}» pasó a estado «{Ui.EstadoLoteTexto(estado.Value)}».";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var lote = await _db.Lotes.FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
        if (lote is null) return NotFound();

        lote.Estado = EstadoLote.Inactivo;
        lote.FechaUltimoCambioEstado = DateTime.UtcNow;
        lote.EstadoCambiadoPor = null;
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Inactivar, nameof(Lote), lote.Id.ToString(),
            $"Inactivación del lote {lote.Codigo}; su historial se conserva");

        TempData["Ok"] = $"El lote «{lote.Codigo}» fue desactivado. Su historial se conservó.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var lote = await _db.Lotes.FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
        if (lote is null) return NotFound();

        if (await TieneRegistrosRelacionadosAsync(id))
        {
            TempData["Error"] = "Este lote tiene micro lotes u otra información asociada. Use «Desactivar» para conservar su historial.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        lote.Eliminado = true;
        lote.Estado = EstadoLote.Inactivo;
        await _db.SaveChangesAsync();
        await _audit.RegistrarAsync(AccionAuditoria.Eliminar, nameof(Lote), lote.Id.ToString(),
            $"Eliminación del lote {lote.Codigo} (sin registros asociados)");

        TempData["Ok"] = $"El lote «{lote.Codigo}» fue eliminado.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- helpers ----------
    private async Task ValidarComunAsync(LoteFormViewModel model, int? fincaId)
    {
        if (fincaId is null)
        {
            ModelState.AddModelError(string.Empty, "No hay una finca configurada.");
            return;
        }

        var codigo = model.Codigo.Trim();
        var existe = await _db.Lotes.AnyAsync(l => l.FincaId == fincaId && l.Codigo == codigo && l.Id != model.Id && !l.Eliminado);
        if (existe)
            ModelState.AddModelError(nameof(model.Codigo), "El código ya existe: ya hay un lote o micro lote registrado con ese código.");

        if (model.Tipo == TipoLote.MicroLote && model.LotePadreId is null)
            ModelState.AddModelError(nameof(model.LotePadreId), "Debe indicar el lote al que pertenece el micro lote.");

        if (model.AreaHectareas <= 0)
            ModelState.AddModelError(nameof(model.AreaHectareas), "El área debe ser mayor a cero.");
    }

    private async Task<IEnumerable<(int Id, string Texto)>> LotesPadreAsync(int? exceptoId = null)
    {
        var lotes = await _db.Lotes.AsNoTracking()
            .Where(l => l.Tipo == TipoLote.Lote && !l.Eliminado && l.Id != exceptoId)
            .OrderBy(l => l.Codigo)
            .Select(l => new { l.Id, l.Codigo, l.Nombre })
            .ToListAsync();
        return lotes.Select(l => (l.Id, $"{l.Codigo} — {l.Nombre}")).ToList();
    }

    
    private async Task<bool> TieneRegistrosRelacionadosAsync(int loteId)
        => await _db.Lotes.AnyAsync(x => x.LotePadreId == loteId && !x.Eliminado);
}
