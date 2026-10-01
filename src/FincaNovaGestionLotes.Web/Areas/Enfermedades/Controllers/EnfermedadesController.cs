using FincaNovaGestionLotes.Web.Areas.Enfermedades.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.AspNetCore.Mvc;
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
            .Include(e => e.Lote)
            .AsQueryable();

        if (filtro.LoteId.HasValue)
            query = query.Where(e => e.LoteId == filtro.LoteId.Value);

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
                e.LoteId,
                Nombre = e.Nombre.ToLower()
            })
            .ToDictionary(
                g => $"{g.Key.LoteId}|{g.Key.Nombre}",
                g => g.OrderBy(x => x.FechaDeteccion).ToList());

        filtro.Resultados = enfermedades
            .Select(e =>
            {
                var key = $"{e.LoteId}|{e.Nombre.ToLower()}";

                var lista = nombresPorLote[key];

                var indice = lista.FindIndex(x => x.Id == e.Id);

                return new EnfermedadListaViewModel
                {
                    Id = e.Id,
                    LoteId = e.LoteId,
                    LoteCodigo = e.Lote.Codigo,
                    Nombre = e.Nombre,
                    FechaDeteccion = e.FechaDeteccion,
                    Severidad = e.Severidad,
                    Descripcion = e.Descripcion,
                    Atendida = e.Atendida,
                    EsRecurrencia = indice > 0
                };
            })
            .ToList();

        filtro.Lotes = await ObtenerLotesAsync();

        return View(filtro);
    }


    [HttpGet]
    public async Task<IActionResult> Crear(int? loteId = null)
    {
        var model = new EnfermedadCrearViewModel
        {
            LoteId = loteId ?? 0,
            Lotes = await ObtenerLotesAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(EnfermedadCrearViewModel model)
    {
        model.Lotes = await ObtenerLotesAsync();

        if (!await _db.Lotes.AnyAsync(
                l => l.Id == model.LoteId && !l.Eliminado))
        {
            ModelState.AddModelError(
                nameof(model.LoteId),
                "El lote seleccionado no existe.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var enfermedad = new Enfermedad
        {
            LoteId = model.LoteId,
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
            $"Registro de enfermedad {enfermedad.Nombre} en el lote {enfermedad.LoteId}");

        TempData["Ok"] =
            "La enfermedad fue registrada correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = enfermedad.Id });
    }
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var enfermedad = await _db.Enfermedades
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        var model = new EnfermedadEditarViewModel
        {
            Id = enfermedad.Id,
            LoteId = enfermedad.LoteId,
            Nombre = enfermedad.Nombre,
            FechaDeteccion = enfermedad.FechaDeteccion,
            Severidad = enfermedad.Severidad,
            Descripcion = enfermedad.Descripcion,
            Observaciones = enfermedad.Observaciones,
            Lotes = await ObtenerLotesAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        EnfermedadEditarViewModel model)
    {
        model.Lotes = await ObtenerLotesAsync();

        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == model.Id);

        if (enfermedad is null)
            return NotFound();

        var loteExiste = await _db.Lotes.AnyAsync(
            l => l.Id == model.LoteId &&
                 !l.Eliminado);

        if (!loteExiste)
        {
            ModelState.AddModelError(
                nameof(model.LoteId),
                "El lote seleccionado no existe.");
        }

        if (!ModelState.IsValid)
            return View(model);

        enfermedad.LoteId = model.LoteId;
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
            .Include(e => e.Lote)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        return View(enfermedad);
    }


    [HttpGet]
    public async Task<IActionResult> CrearTratamiento(int enfermedadId)
    {
        var existe = await _db.Enfermedades
            .AnyAsync(e => e.Id == enfermedadId);

        if (!existe)
            return NotFound();

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
            Observaciones = model.Observaciones?.Trim()
        };

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


    [HttpGet]
    public async Task<IActionResult> Alertas()
    {
        var enfermedades = await _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Lote)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .OrderBy(e => e.LoteId)
            .ThenBy(e => e.Nombre)
            .ThenBy(e => e.FechaDeteccion)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var alertas = enfermedades
            .GroupBy(e => new
            {
                e.LoteId,
                Nombre = e.Nombre.Trim().ToLower()
            })
            .SelectMany(grupo =>
                grupo
                    .OrderBy(e => e.FechaDeteccion)
                    .ThenBy(e => e.Id)
                    .Skip(1))
            .OrderByDescending(e => e.FechaDeteccion)
            .ToList();

        return View(alertas);
    }

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

    private async Task<IEnumerable<LoteOpcionViewModel>>
        ObtenerLotesAsync()
    {
        return await _db.Lotes
            .AsNoTracking()
            .Where(l => !l.Eliminado)
            .OrderBy(l => l.Codigo)
            .Select(l => new LoteOpcionViewModel
            {
                Id = l.Id,
                Texto = l.Codigo + " — " + l.Nombre
            })
            .ToListAsync();
    }
}