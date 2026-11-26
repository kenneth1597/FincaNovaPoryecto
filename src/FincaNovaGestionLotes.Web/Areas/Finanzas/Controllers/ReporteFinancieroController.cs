using System.Globalization;
using FincaNovaGestionLotes.Web.Areas.Finanzas.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Controllers;

[Area("Finanzas")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class ReporteFinancieroController : Controller
{
    private readonly AppDbContext _db;

    public ReporteFinancieroController(AppDbContext db)
    {
        _db = db;
    }

    // HU-62: reporte consolidado de ingresos, egresos y balance, por período productivo y por mes.
    [HttpGet]
    public async Task<IActionResult> Index(ReporteFinancieroViewModel filtro)
    {
        var ventas = _db.Ventas.AsNoTracking().AsQueryable();
        var gastos = _db.Gastos.AsNoTracking().AsQueryable();

        if (filtro.PeriodoProductivoId.HasValue)
        {
            var periodoId = filtro.PeriodoProductivoId.Value;
            ventas = ventas.Where(v => v.PeriodoProductivoId == periodoId);
            gastos = gastos.Where(g => g.PeriodoProductivoId == periodoId);
        }

        // Totales por período; lo que no tiene período se muestra aparte para que los totales cuadren.
        var ingresosPeriodo = await ventas
            .GroupBy(v => v.PeriodoProductivoId)
            .Select(x => new { PeriodoId = x.Key, Total = x.Sum(v => v.Total) })
            .ToListAsync();

        var egresosPeriodo = await gastos
            .GroupBy(g => g.PeriodoProductivoId)
            .Select(x => new { PeriodoId = x.Key, Total = x.Sum(g => g.Monto) })
            .ToListAsync();

        var nombres = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.FechaInicio)
            .Select(p => new { p.Id, p.Nombre })
            .ToListAsync();

        var idsConMovimiento = ingresosPeriodo.Select(x => x.PeriodoId)
            .Concat(egresosPeriodo.Select(x => x.PeriodoId))
            .ToHashSet();

        var porPeriodo = nombres
            .Where(p => idsConMovimiento.Contains(p.Id))
            .Select(p => new ResumenFinancieroViewModel
            {
                Nombre = p.Nombre,
                Ingresos = ingresosPeriodo.FirstOrDefault(x => x.PeriodoId == p.Id)?.Total ?? 0,
                Egresos = egresosPeriodo.FirstOrDefault(x => x.PeriodoId == p.Id)?.Total ?? 0
            })
            .ToList();

        if (idsConMovimiento.Contains(null))
        {
            porPeriodo.Add(new ResumenFinancieroViewModel
            {
                Nombre = "Sin período asignado",
                Ingresos = ingresosPeriodo.FirstOrDefault(x => x.PeriodoId == null)?.Total ?? 0,
                Egresos = egresosPeriodo.FirstOrDefault(x => x.PeriodoId == null)?.Total ?? 0
            });
        }

        // Totales por mes, del más reciente al más antiguo.
        var ingresosMes = await ventas
            .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
            .Select(x => new { x.Key.Year, x.Key.Month, Total = x.Sum(v => v.Total) })
            .ToListAsync();

        var egresosMes = await gastos
            .GroupBy(g => new { g.Fecha.Year, g.Fecha.Month })
            .Select(x => new { x.Key.Year, x.Key.Month, Total = x.Sum(g => g.Monto) })
            .ToListAsync();

        var meses = ingresosMes.Select(x => (x.Year, x.Month))
            .Concat(egresosMes.Select(x => (x.Year, x.Month)))
            .Distinct()
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList();

        var cultura = new CultureInfo("es-CR");
        filtro.PorMes = meses
            .Select(m => new ResumenFinancieroViewModel
            {
                Nombre = cultura.TextInfo.ToTitleCase(new DateTime(m.Year, m.Month, 1).ToString("MMMM yyyy", cultura)),
                Ingresos = ingresosMes.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Total ?? 0,
                Egresos = egresosMes.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month)?.Total ?? 0
            })
            .ToList();

        filtro.PorPeriodo = porPeriodo;
        filtro.TotalIngresos = porPeriodo.Sum(p => p.Ingresos);
        filtro.TotalEgresos = porPeriodo.Sum(p => p.Egresos);

        filtro.Periodos = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
            .ToListAsync();

        return View(filtro);
    }
}
