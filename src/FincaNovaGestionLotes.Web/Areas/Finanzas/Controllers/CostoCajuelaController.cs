using FincaNovaGestionLotes.Web.Areas.Finanzas.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Controllers;

[Area("Finanzas")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class CostoCajuelaController : Controller
{
    private readonly AppDbContext _db;

    public CostoCajuelaController(AppDbContext db)
    {
        _db = db;
    }

    // HU-58: costo por cajuela = gastos del período entre las cajuelas recolectadas en ese período.
    [HttpGet]
    public async Task<IActionResult> Index(CostoCajuelaViewModel filtro)
    {
        var periodos = _db.PeriodosProductivos.AsNoTracking().AsQueryable();

        if (filtro.PeriodoProductivoId.HasValue)
            periodos = periodos.Where(p => p.Id == filtro.PeriodoProductivoId.Value);

        // Se suman gastos y cajuelas por período; un período sin movimientos queda en cero.
        var gastosPorPeriodo = await _db.Gastos.AsNoTracking()
            .Where(g => g.PeriodoProductivoId != null)
            .GroupBy(g => g.PeriodoProductivoId!.Value)
            .Select(x => new { PeriodoId = x.Key, Total = x.Sum(g => g.Monto) })
            .ToDictionaryAsync(x => x.PeriodoId, x => x.Total);

        var cajuelasPorPeriodo = await _db.Recolecciones.AsNoTracking()
            .GroupBy(r => r.PeriodoProductivoId)
            .Select(x => new { PeriodoId = x.Key, Total = x.Sum(r => r.Cajuelas) })
            .ToDictionaryAsync(x => x.PeriodoId, x => x.Total);

        var filas = (await periodos
                .OrderByDescending(p => p.FechaInicio)
                .Select(p => new { p.Id, p.Nombre, p.FechaInicio, p.FechaFin, p.Cerrado })
                .ToListAsync())
            .Select(p => new CostoCajuelaPeriodoViewModel
            {
                Periodo = p.Nombre,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Cerrado = p.Cerrado,
                Gastos = gastosPorPeriodo.GetValueOrDefault(p.Id),
                Cajuelas = cajuelasPorPeriodo.GetValueOrDefault(p.Id)
            })
            .ToList();

        filtro.TotalGastos = filas.Sum(f => f.Gastos);
        filtro.TotalCajuelas = filas.Sum(f => f.Cajuelas);

        // Los gastos sin período no se pueden repartir entre cajuelas; se avisa para que no sorprendan.
        if (!filtro.PeriodoProductivoId.HasValue)
        {
            filtro.GastosSinPeriodo = await _db.Gastos.AsNoTracking()
                .Where(g => g.PeriodoProductivoId == null)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;
        }

        var (pagina, paginacion) = filas.Paginar(filtro.Pagina);
        filtro.Filas = pagina;
        filtro.Paginacion = paginacion;

        filtro.Periodos = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
            .ToListAsync();

        return View(filtro);
    }
}
