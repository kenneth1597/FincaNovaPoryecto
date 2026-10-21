using FincaNovaGestionLotes.Web.Areas.Enfermedades.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Enfermedades.Controllers;

[Area("Enfermedades")]
public class EnfermedadesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public EnfermedadesController(
        AppDbContext db,
        IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }


    [HttpGet]
    public async Task<IActionResult> Index(EnfermedadFiltroViewModel filtro)
    {
        var query = _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .AsQueryable();

        if (filtro.TerrenoId.HasValue)
            query = query.Where(e => e.TerrenoId == filtro.TerrenoId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Nombre))
        {
            var nombre = filtro.Nombre.Trim();

            query = query.Where(e =>
                e.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Severidad))
        {
            query = query.Where(e =>
                e.Severidad == filtro.Severidad);
        }

        if (filtro.Estado == "Abiertas")
            query = query.Where(e => e.Estado != EstadoEnfermedad.Cerrada);
        else if (filtro.Estado == "Cerradas")
            query = query.Where(e => e.Estado == EstadoEnfermedad.Cerrada);

        if (filtro.Desde.HasValue)
        {
            query = query.Where(e =>
                e.FechaDeteccion >= filtro.Desde.Value);
        }

        if (filtro.Hasta.HasValue)
        {
            var hasta = filtro.Hasta.Value.Date.AddDays(1);

            query = query.Where(e =>
                e.FechaDeteccion < hasta);
        }

        var enfermedades = await query
            .OrderByDescending(e => e.FechaDeteccion)
            .ToListAsync();

        var ids = enfermedades.Select(e => e.Id).ToList();

        var nombresPorLote = enfermedades
            .GroupBy(e => new
            {
                e.TerrenoId,
                Nombre = e.Nombre.ToLower()
            })
            .ToDictionary(
                g => $"{g.Key.TerrenoId}|{g.Key.Nombre}",
                g => g.OrderBy(x => x.FechaDeteccion).ToList());

        filtro.Resultados = enfermedades
            .Select(e =>
            {
                var key = $"{e.TerrenoId}|{e.Nombre.ToLower()}";

                var lista = nombresPorLote[key];

                var indice = lista.FindIndex(x => x.Id == e.Id);

                return new EnfermedadListaViewModel
                {
                    Id = e.Id,
                    TerrenoId = e.TerrenoId,
                    TerrenoCodigo = e.Terreno.Codigo,
                    Nombre = e.Nombre,
                    FechaDeteccion = e.FechaDeteccion,
                    Severidad = e.Severidad,
                    Descripcion = e.Descripcion,
                    Atendida = e.Atendida,
                    EsRecurrencia = indice > 0,
                    Estado = e.Estado
                };
            })
            .ToList();

        var (paginaEnfermedades, infoEnfermedades) = filtro.Resultados.Paginar(filtro.Pagina);
        filtro.Resultados = paginaEnfermedades;
        filtro.Paginacion = infoEnfermedades;

        filtro.Terrenos = await ObtenerTerrenosAsync();

        return View(filtro);
    }


    [HttpGet]
    public async Task<IActionResult> Crear(int? terrenoId = null)
    {
        var model = new EnfermedadCrearViewModel
        {
            TerrenoId = terrenoId ?? 0,
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(EnfermedadCrearViewModel model)
    {
        model.Terrenos = await ObtenerTerrenosAsync();

        if (!await _db.Terrenos.AnyAsync(
                l => l.Id == model.TerrenoId && !l.Eliminado))
        {
            ModelState.AddModelError(
                nameof(model.TerrenoId),
                "La finca o el lote seleccionado no existe.");
        }

        if (!string.IsNullOrWhiteSpace(model.Nombre) && !CatalogoEnfermedades.Existe(model.Nombre))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Seleccione una enfermedad de la lista.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var enfermedad = new Enfermedad
        {
            TerrenoId = model.TerrenoId,
            Nombre = model.Nombre.Trim(),
            FechaDeteccion = model.FechaDeteccion,
            Severidad = model.Severidad.Trim(),
            Descripcion = model.Descripcion.Trim(),
            Observaciones = model.Observaciones?.Trim(),
            Atendida = false,
            FechaAtendida = null,
            AtendidaPor = null
        };

        _db.Enfermedades.Add(enfermedad);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Registro de enfermedad {enfermedad.Nombre} en el lote {enfermedad.TerrenoId}");

        TempData["Ok"] =
            "La enfermedad fue registrada correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = enfermedad.Id });
    }
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var enfermedad = await _db.Enfermedades
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada y ya no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var model = new EnfermedadEditarViewModel
        {
            Id = enfermedad.Id,
            TerrenoId = enfermedad.TerrenoId,
            Nombre = enfermedad.Nombre,
            FechaDeteccion = enfermedad.FechaDeteccion,
            Severidad = enfermedad.Severidad,
            Descripcion = enfermedad.Descripcion,
            Observaciones = enfermedad.Observaciones,
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        EnfermedadEditarViewModel model)
    {
        model.Terrenos = await ObtenerTerrenosAsync();

        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == model.Id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada y ya no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedad.Id });
        }

        var loteExiste = await _db.Terrenos.AnyAsync(
            l => l.Id == model.TerrenoId &&
                 !l.Eliminado);

        if (!loteExiste)
        {
            ModelState.AddModelError(
                nameof(model.TerrenoId),
                "La finca o el lote seleccionado no existe.");
        }

        var nombreSinCambio = string.Equals(model.Nombre?.Trim(), enfermedad.Nombre, StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(model.Nombre) && !nombreSinCambio && !CatalogoEnfermedades.Existe(model.Nombre))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Seleccione una enfermedad de la lista.");
        }

        if (!ModelState.IsValid)
            return View(model);

        enfermedad.TerrenoId = model.TerrenoId;
        enfermedad.Nombre = model.Nombre.Trim();
        enfermedad.FechaDeteccion = model.FechaDeteccion;
        enfermedad.Severidad = model.Severidad.Trim();
        enfermedad.Descripcion = model.Descripcion.Trim();
        enfermedad.Observaciones = model.Observaciones?.Trim();

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Modificación de la enfermedad {enfermedad.Nombre}");

        TempData["Ok"] =
            "La información de la enfermedad fue actualizada correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = enfermedad.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var enfermedad = await _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        return View(enfermedad);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int id, [Bind(Prefix = "Cierre")] CierreEnfermedadViewModel cierre)
    {
        var enfermedad = await _db.Enfermedades
            .Include(e => e.Tratamientos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad ya estaba cerrada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var errores = new List<string>();

        if (!enfermedad.Tratamientos.Any())
            errores.Add("No se puede cerrar una enfermedad sin haber aplicado al menos un tratamiento.");

        if (cierre.ResultadoCierre is not { } resultado || !Enum.IsDefined(resultado))
            errores.Add("Seleccione el resultado final.");

        if (string.IsNullOrWhiteSpace(cierre.ComentarioCierre))
            errores.Add("El comentario de cierre es obligatorio.");
        else if (cierre.ComentarioCierre.Trim().Length > 500)
            errores.Add("El comentario de cierre no puede superar 500 caracteres.");

        if (cierre.TratamientoEfectivoId is { } tratamientoId &&
            enfermedad.Tratamientos.All(t => t.Id != tratamientoId))
            errores.Add("El tratamiento seleccionado no pertenece a esta enfermedad.");

        if (!cierre.Confirmo)
            errores.Add("Debe confirmar que verificó en campo que la enfermedad está controlada.");

        if (errores.Count > 0)
        {
            TempData["ErroresCierre"] = string.Join("|", errores);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var usuario = HttpContext.Session.GetString("UsuarioNombre");

        enfermedad.Estado = EstadoEnfermedad.Cerrada;
        enfermedad.FechaCierre = DateTime.UtcNow;
        enfermedad.CerradaPor = usuario;
        enfermedad.ResultadoCierre = cierre.ResultadoCierre;
        enfermedad.ComentarioCierre = cierre.ComentarioCierre!.Trim();
        enfermedad.TratamientoEfectivoId = cierre.TratamientoEfectivoId;
        enfermedad.ModificadoPor = usuario;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Cierre de la enfermedad {enfermedad.Nombre}: {Ui.ResultadoCierreTexto(cierre.ResultadoCierre!.Value)}");

        TempData["Ok"] = "La enfermedad fue cerrada y quedó en el histórico.";

        return RedirectToAction(nameof(Detalle), new { id });
    }


    [HttpGet]
    public async Task<IActionResult> CrearTratamiento(int enfermedadId)
    {
        var estado = await _db.Enfermedades
            .Where(e => e.Id == enfermedadId)
            .Select(e => (EstadoEnfermedad?)e.Estado)
            .FirstOrDefaultAsync();

        if (estado is null)
            return NotFound();

        if (estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada; ya no admite tratamientos.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedadId });
        }

        var model = new TratamientoCrearViewModel
        {
            EnfermedadId = enfermedadId,
            Productos = new List<ProductoViewModel>
            {
                new ProductoViewModel()
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearTratamiento(
        TratamientoCrearViewModel model)
    {
        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == model.EnfermedadId);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada; ya no admite tratamientos.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedad.Id });
        }

        if (model.Efectividad is { } efectividad && !Enum.IsDefined(efectividad))
        {
            ModelState.AddModelError(
                nameof(model.Efectividad),
                "La efectividad indicada no es válida.");
        }

        if (model.Productos.Count == 0)
        {
            ModelState.AddModelError(
                nameof(model.Productos),
                "Debe registrar al menos un producto utilizado.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var tratamiento = new TratamientoEnfermedad
        {
            EnfermedadId = model.EnfermedadId,
            Tratamiento = model.Tratamiento.Trim(),
            FechaAplicacion = model.FechaAplicacion,
            Responsable = model.Responsable?.Trim(),
            Resultado = model.Resultado?.Trim(),
            Efectividad = model.Efectividad,
            Observaciones = model.Observaciones?.Trim()
        };

        if (enfermedad.Estado == EstadoEnfermedad.Activa)
            enfermedad.Estado = EstadoEnfermedad.EnTratamiento;

        foreach (var producto in model.Productos)
        {
            tratamiento.Productos.Add(
                new ProductoTratamiento
                {
                    NombreProducto = producto.NombreProducto.Trim(),
                    Cantidad = producto.Cantidad,
                    Unidad = producto.Unidad.Trim()
                });
        }

        _db.TratamientosEnfermedad.Add(tratamiento);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(TratamientoEnfermedad),
            tratamiento.Id.ToString(),
            $"Tratamiento registrado para la enfermedad {enfermedad.Nombre}");

        TempData["Ok"] =
            "El tratamiento y los productos utilizados fueron registrados.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = model.EnfermedadId });
    }


    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpGet]
    public async Task<IActionResult> Alertas(int pagina = 1)
    {
        var enfermedades = await _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .OrderBy(e => e.TerrenoId)
            .ThenBy(e => e.Nombre)
            .ThenBy(e => e.FechaDeteccion)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var alertas = enfermedades
            .GroupBy(e => new
            {
                e.TerrenoId,
                Nombre = e.Nombre.Trim().ToLower()
            })
            .SelectMany(grupo =>
                grupo
                    .OrderBy(e => e.FechaDeteccion)
                    .ThenBy(e => e.Id)
                    .Skip(1))
            .OrderByDescending(e => e.FechaDeteccion)
            .ToList();

        var (paginaAlertas, infoAlertas) = alertas.Paginar(pagina);
        ViewBag.Paginacion = infoAlertas;

        return View(paginaAlertas);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarAtendida(int id)
    {
        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        enfermedad.Atendida = true;
        enfermedad.FechaAtendida = DateTime.UtcNow;
        enfermedad.AtendidaPor = User.Identity?.Name;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Alerta de recurrencia marcada como atendida: {enfermedad.Nombre}");

        TempData["Ok"] =
            "La alerta fue marcada como atendida.";

        return RedirectToAction(nameof(Alertas));
    }

    private async Task<IEnumerable<TerrenoOpcionViewModel>>
        ObtenerTerrenosAsync()
    {
        return await _db.Terrenos
            .AsNoTracking()
            .Where(l => !l.Eliminado)
            .OrderBy(l => l.Codigo)
            .Select(l => new TerrenoOpcionViewModel
            {
                Id = l.Id,
                Texto = l.Codigo + " — " + l.Nombre
            })
            .ToListAsync();
    }
}