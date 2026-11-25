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
public class ControlFinancieroController : Controller
{
    private readonly AppDbContext _db;

    public ControlFinancieroController(AppDbContext db)
    {
        _db = db;
    }

    // Control de ingresos (ventas) y egresos (gastos) con balance.
    [HttpGet]
    public async Task<IActionResult> Index(ControlFinancieroViewModel filtro)
    {
        if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde > filtro.Hasta)
            ModelState.AddModelError(nameof(filtro.Hasta), "La fecha final no puede ser anterior a la inicial.");

        var ventas = _db.Ventas.AsNoTracking().AsQueryable();
        var gastos = _db.Gastos.AsNoTracking().AsQueryable();

        if (filtro.Desde.HasValue)
        {
            var desde = filtro.Desde.Value.Date;
            ventas = ventas.Where(v => v.Fecha >= desde);
            gastos = gastos.Where(g => g.Fecha >= desde);
        }

        if (filtro.Hasta.HasValue)
        {
            var hasta = filtro.Hasta.Value.Date;
            ventas = ventas.Where(v => v.Fecha <= hasta);
            gastos = gastos.Where(g => g.Fecha <= hasta);
        }

        if (filtro.PeriodoProductivoId.HasValue)
        {
            var periodoId = filtro.PeriodoProductivoId.Value;
            ventas = ventas.Where(v => v.PeriodoProductivoId == periodoId);
            gastos = gastos.Where(g => g.PeriodoProductivoId == periodoId);
        }

        filtro.TotalIngresos = await ventas.SumAsync(v => (decimal?)v.Total) ?? 0;
        filtro.TotalEgresos = await gastos.SumAsync(g => (decimal?)g.Monto) ?? 0;

        filtro.EgresosPorCategoria = await gastos
            .GroupBy(g => g.CategoriaGasto.Nombre)
            .Select(x => new EgresoCategoriaViewModel { Categoria = x.Key, Total = x.Sum(g => g.Monto) })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        // Se unen ventas y gastos en una sola línea de tiempo, de lo más reciente a lo más antiguo.
        var ingresos = await ventas
            .Select(v => new MovimientoViewModel
            {
                Fecha = v.Fecha,
                EsIngreso = true,
                Concepto = "Venta de café",
                Detalle = v.Comprador,
                Monto = v.Total,
                Periodo = v.PeriodoProductivo != null ? v.PeriodoProductivo.Nombre : null
            })
            .ToListAsync();

        var egresos = await gastos
            .Select(g => new MovimientoViewModel
            {
                Fecha = g.Fecha,
                EsIngreso = false,
                Concepto = g.CategoriaGasto.Nombre,
                Detalle = g.Descripcion,
                Monto = g.Monto,
                Periodo = g.PeriodoProductivo != null ? g.PeriodoProductivo.Nombre : null
            })
            .ToListAsync();

        var (movimientos, paginacion) = ingresos
            .Concat(egresos)
            .OrderByDescending(m => m.Fecha)
            .ThenBy(m => m.EsIngreso)
            .Paginar(filtro.Pagina);

        filtro.Movimientos = movimientos;
        filtro.Paginacion = paginacion;
        filtro.Periodos = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
            .ToListAsync();

        return View(filtro);
    }
}
