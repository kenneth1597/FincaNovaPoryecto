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
            .Include(p => p.Terreno)
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
            TerrenoId = terrenoId ?? 0,
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

        var loteExiste = await _db.Terrenos.AnyAsync(
            l => l.Id == model.TerrenoId &&
                 !l.Eliminado);

        if (!loteExiste)
        {
            ModelState.AddModelError(
                nameof(model.TerrenoId),
                "La finca o el lote seleccionado no existe.");
        }

        if (model.FechaFin.HasValue &&
            model.FechaFin.Value.Date < model.FechaInicio.Date)
        {
            ModelState.AddModelError(
                nameof(model.FechaFin),
                "La fecha de finalización no puede ser anterior a la fecha de inicio.");
        }

        var periodoAbierto = await _db.PeriodosProductivos
            .AnyAsync(p =>
                p.TerrenoId == model.TerrenoId &&
                !p.Cerrado);

        if (periodoAbierto && !model.FechaFin.HasValue)
        {
            ModelState.AddModelError(
                string.Empty,
                "El lote ya tiene un período productivo abierto.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var periodo = new PeriodoProductivo
        {
            TerrenoId = model.TerrenoId,
            FechaInicio = model.FechaInicio,
            FechaFin = model.FechaFin,
            Observaciones = model.Observaciones?.Trim(),
            Cerrado = model.FechaFin.HasValue
        };

        _db.PeriodosProductivos.Add(periodo);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(PeriodoProductivo),
            periodo.Id.ToString(),
            $"Registro de período productivo para el lote {periodo.TerrenoId}");

        TempData["Ok"] =
            "El período productivo fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
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