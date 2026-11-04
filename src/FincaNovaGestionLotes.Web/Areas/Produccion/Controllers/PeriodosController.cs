using FincaNovaGestionLotes.Web.Areas.Produccion.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Controllers;

[Area("Produccion")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class PeriodosController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public PeriodosController(
        AppDbContext db,
        IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pagina = 1)
    {
        var (periodos, paginacion) = await _db.PeriodosProductivos
            .AsNoTracking()
            .Include(p => p.Terrenos)
                .ThenInclude(pt => pt.Terreno)
            .OrderByDescending(p => p.FechaInicio)
            .PaginarAsync(pagina);

        ViewBag.Paginacion = paginacion;

        return View(periodos);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int? terrenoId = null)
    {
        var model = new PeriodoCrearViewModel
        {
            TerrenoIds = terrenoId.HasValue ? new List<int> { terrenoId.Value } : new List<int>(),
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        PeriodoCrearViewModel model)
    {
        model.Terrenos = await ObtenerTerrenosAsync();
        model.TerrenoIds = model.TerrenoIds.Distinct().ToList();

        if (model.FechaFin.HasValue &&
            model.FechaFin.Value.Date < model.FechaInicio.Date)
        {
            ModelState.AddModelError(
                nameof(model.FechaFin),
                "La fecha de finalización no puede ser anterior a la fecha de inicio.");
        }
        else
        {
            await ValidarTerrenosAsync(model.TerrenoIds, model.FechaInicio, model.FechaFin, periodoExcluidoId: null);
        }

        if (!ModelState.IsValid)
            return View(model);

        var periodo = new PeriodoProductivo
        {
            Nombre = model.Nombre.Trim(),
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            Observaciones = model.Observaciones?.Trim(),
            // Un período con fecha de fin futura (ej. cosecha nov-ene) sigue abierto.
            Cerrado = model.FechaFin.HasValue && model.FechaFin.Value.Date < DateTime.Today,
            Terrenos = model.TerrenoIds
                .Select(id => new PeriodoTerreno { TerrenoId = id })
                .ToList()
        };

        _db.PeriodosProductivos.Add(periodo);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(PeriodoProductivo),
            periodo.Id.ToString(),
            $"Registro del período productivo «{periodo.Nombre}» con {periodo.Terrenos.Count} finca(s)/lote(s)");

        TempData["Ok"] =
            "El período productivo fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Terrenos(int id)
    {
        var periodo = await _db.PeriodosProductivos
            .AsNoTracking()
            .Include(p => p.Terrenos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (periodo is null)
            return NotFound();

        var model = new PeriodoAsignarTerrenosViewModel
        {
            PeriodoId = periodo.Id,
            Nombre = periodo.Nombre,
            FechaInicio = periodo.FechaInicio,
            FechaFin = periodo.FechaFin,
            TerrenoIds = periodo.Terrenos.Select(pt => pt.TerrenoId).ToList(),
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Terrenos(PeriodoAsignarTerrenosViewModel model)
    {
        var periodo = await _db.PeriodosProductivos
            .Include(p => p.Terrenos)
            .FirstOrDefaultAsync(p => p.Id == model.PeriodoId);

        if (periodo is null)
            return NotFound();

        model.Nombre = periodo.Nombre;
        model.FechaInicio = periodo.FechaInicio;
        model.FechaFin = periodo.FechaFin;
        model.Terrenos = await ObtenerTerrenosAsync();
        model.TerrenoIds = model.TerrenoIds.Distinct().ToList();

        var actuales = periodo.Terrenos.Select(pt => pt.TerrenoId).ToList();
        var agregados = model.TerrenoIds.Except(actuales).ToList();
        var quitados = actuales.Except(model.TerrenoIds).ToList();

        await ValidarTerrenosAsync(model.TerrenoIds, periodo.FechaInicio, periodo.FechaFin, periodo.Id);

        // No se puede sacar del período un terreno que ya tiene producción o gastos en él.
        foreach (var terrenoId in quitados)
        {
            if (await TieneRegistrosEnPeriodoAsync(periodo.Id, terrenoId))
            {
                var texto = model.Terrenos.FirstOrDefault(t => t.Id == terrenoId)?.Texto ?? terrenoId.ToString();
                ModelState.AddModelError(nameof(model.TerrenoIds),
                    $"No se puede quitar {texto}: ya tiene recolecciones, pesos o gastos registrados en este período.");
            }
        }

        if (!ModelState.IsValid)
            return View(model);

        _db.PeriodosTerrenos.RemoveRange(periodo.Terrenos.Where(pt => quitados.Contains(pt.TerrenoId)));
        _db.PeriodosTerrenos.AddRange(agregados.Select(id =>
            new PeriodoTerreno { PeriodoProductivoId = periodo.Id, TerrenoId = id }));

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(PeriodoProductivo),
            periodo.Id.ToString(),
            $"Asignación de fincas/lotes del período «{periodo.Nombre}»",
            valorAnterior: string.Join(",", actuales.OrderBy(x => x)),
            valorNuevo: string.Join(",", model.TerrenoIds.OrderBy(x => x)));

        TempData["Ok"] = "Las fincas y lotes del período fueron actualizados.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Al menos un terreno, todos existentes, y ninguno con otro período cuyas
    /// fechas se crucen (un período sin fecha de fin sigue abierto).
    /// </summary>
    private async Task ValidarTerrenosAsync(
        List<int> terrenoIds, DateTime inicio, DateTime? fin, int? periodoExcluidoId)
    {
        if (terrenoIds.Count == 0)
        {
            ModelState.AddModelError(
                nameof(PeriodoCrearViewModel.TerrenoIds),
                "Debe seleccionar al menos una finca o lote.");
            return;
        }

        var existentes = await _db.Terrenos
            .Where(t => terrenoIds.Contains(t.Id) && !t.Eliminado)
            .Select(t => new { t.Id, t.Codigo, t.Nombre })
            .ToListAsync();

        if (existentes.Count != terrenoIds.Count)
        {
            ModelState.AddModelError(
                nameof(PeriodoCrearViewModel.TerrenoIds),
                "Alguna de las fincas o lotes seleccionados no existe.");
            return;
        }

        var finNuevo = fin ?? DateTime.MaxValue;

        var cruces = await _db.PeriodosTerrenos
            .Where(pt => terrenoIds.Contains(pt.TerrenoId) &&
                         pt.PeriodoProductivoId != periodoExcluidoId &&
                         pt.PeriodoProductivo.FechaInicio <= finNuevo &&
                         (pt.PeriodoProductivo.FechaFin == null || pt.PeriodoProductivo.FechaFin >= inicio))
            .Select(pt => new { pt.TerrenoId, pt.PeriodoProductivo.Nombre })
            .ToListAsync();

        foreach (var cruce in cruces)
        {
            var terreno = existentes.First(t => t.Id == cruce.TerrenoId);
            ModelState.AddModelError(
                nameof(PeriodoCrearViewModel.TerrenoIds),
                $"{terreno.Codigo} — {terreno.Nombre} ya está en el período «{cruce.Nombre}», que se cruza con estas fechas.");
        }
    }

    private async Task<bool> TieneRegistrosEnPeriodoAsync(int periodoId, int terrenoId) =>
        await _db.Recolecciones.AnyAsync(r => r.PeriodoProductivoId == periodoId && r.TerrenoId == terrenoId) ||
        await _db.PesosSecado.AnyAsync(p => p.PeriodoProductivoId == periodoId && p.TerrenoId == terrenoId) ||
        await _db.PesosProcesado.AnyAsync(p => p.PeriodoProductivoId == periodoId && p.TerrenoId == terrenoId) ||
        await _db.Gastos.AnyAsync(g => g.PeriodoProductivoId == periodoId && g.TerrenoId == terrenoId);

    private async Task<IEnumerable<TerrenoOpcionViewModel>>
        ObtenerTerrenosAsync()
    {
        // Cada finca seguida de sus lotes.
        return await _db.Terrenos
            .AsNoTracking()
            .Where(l => !l.Eliminado)
            .OrderBy(l => l.FincaPadreId == null ? l.Codigo : l.FincaPadre!.Codigo)
            .ThenBy(l => l.FincaPadreId != null)
            .ThenBy(l => l.Codigo)
            .Select(l => new TerrenoOpcionViewModel
            {
                Id = l.Id,
                Texto = l.Codigo + " — " + l.Nombre,
                Tipo = l.Tipo
            })
            .ToListAsync();
    }
}
