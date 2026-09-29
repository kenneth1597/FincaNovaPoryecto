using FincaNovaGestionLotes.Web.Areas.Produccion.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Controllers;

[Area("Produccion")]
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
    public async Task<IActionResult> Index()
    {
        var periodos = await _db.PeriodosProductivos
            .AsNoTracking()
            .Include(p => p.Lote)
            .OrderByDescending(p => p.FechaInicio)
            .ToListAsync();

        return View(periodos);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int? loteId = null)
    {
        var model = new PeriodoCrearViewModel
        {
            LoteId = loteId ?? 0,
            Lotes = await ObtenerLotesAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        PeriodoCrearViewModel model)
    {
        model.Lotes = await ObtenerLotesAsync();

        var loteExiste = await _db.Lotes.AnyAsync(
            l => l.Id == model.LoteId &&
                 !l.Eliminado);

        if (!loteExiste)
        {
            ModelState.AddModelError(
                nameof(model.LoteId),
                "El lote seleccionado no existe.");
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
                p.LoteId == model.LoteId &&
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
            LoteId = model.LoteId,
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
            $"Registro de período productivo para el lote {periodo.LoteId}");

        TempData["Ok"] =
            "El período productivo fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
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