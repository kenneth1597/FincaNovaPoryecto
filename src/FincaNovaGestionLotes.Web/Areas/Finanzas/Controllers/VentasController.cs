using FincaNovaGestionLotes.Web.Areas.Finanzas.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Finanzas;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Controllers;

[Area("Finanzas")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class VentasController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public VentasController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    private string UsuarioActual => HttpContext.Session.GetString("UsuarioNombre") ?? "Administrador";

    // HU-20: historial de ventas de café con filtro por fechas y total vendido.
    [HttpGet]
    public async Task<IActionResult> Index(VentaFiltroViewModel filtro)
    {
        var query = _db.Ventas.AsNoTracking().AsQueryable();

        if (filtro.Desde.HasValue)
            query = query.Where(v => v.Fecha >= filtro.Desde.Value.Date);

        if (filtro.Hasta.HasValue)
            query = query.Where(v => v.Fecha <= filtro.Hasta.Value.Date);

        filtro.TotalVendido = await query.SumAsync(v => (decimal?)v.Total) ?? 0;

        var (resultados, paginacion) = await query
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.Id)
            .Select(v => new VentaListItemViewModel
            {
                Id = v.Id,
                Fecha = v.Fecha,
                Comprador = v.Comprador,
                Cantidad = v.Cantidad,
                Unidad = v.Unidad,
                PrecioUnitario = v.PrecioUnitario,
                Total = v.Total,
                Periodo = v.PeriodoProductivo != null ? v.PeriodoProductivo.Nombre : null
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;
        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new VentaFormViewModel();
        await CargarPeriodosAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(VentaFormViewModel model)
    {
        await ValidarAsync(model);

        if (!ModelState.IsValid)
        {
            await CargarPeriodosAsync(model);
            return View(model);
        }

        var venta = new Venta
        {
            Fecha = model.Fecha!.Value.Date,
            Comprador = model.Comprador.Trim(),
            Cantidad = model.Cantidad!.Value,
            Unidad = model.Unidad,
            PrecioUnitario = model.PrecioUnitario!.Value,
            PeriodoProductivoId = model.PeriodoProductivoId,
            Observaciones = model.Observaciones?.Trim(),
            RegistradoPor = UsuarioActual,
            CreadoPor = UsuarioActual
        };

        _db.Ventas.Add(venta);
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Venta), venta.Id.ToString(),
            $"Venta de {venta.Cantidad:0.##} {venta.Unidad.ToString().ToLower()}(s) de café a {venta.Comprador}",
            valorNuevo: $"PrecioUnitario={venta.PrecioUnitario:0.##}");

        TempData["Ok"] = $"Venta a {venta.Comprador} registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var venta = await _db.Ventas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        if (venta is null) return NotFound();

        var model = new VentaFormViewModel
        {
            Id = venta.Id,
            Fecha = venta.Fecha,
            Comprador = venta.Comprador,
            Cantidad = venta.Cantidad,
            Unidad = venta.Unidad,
            PrecioUnitario = venta.PrecioUnitario,
            PeriodoProductivoId = venta.PeriodoProductivoId,
            Observaciones = venta.Observaciones
        };
        await CargarPeriodosAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, VentaFormViewModel model)
    {
        if (id != model.Id) return NotFound();

        var venta = await _db.Ventas.FirstOrDefaultAsync(v => v.Id == id);
        if (venta is null) return NotFound();

        await ValidarAsync(model);

        if (!ModelState.IsValid)
        {
            await CargarPeriodosAsync(model);
            return View(model);
        }

        var anterior = $"Cantidad={venta.Cantidad:0.##}; PrecioUnitario={venta.PrecioUnitario:0.##}; Comprador={venta.Comprador}";

        venta.Fecha = model.Fecha!.Value.Date;
        venta.Comprador = model.Comprador.Trim();
        venta.Cantidad = model.Cantidad!.Value;
        venta.Unidad = model.Unidad;
        venta.PrecioUnitario = model.PrecioUnitario!.Value;
        venta.PeriodoProductivoId = model.PeriodoProductivoId;
        venta.Observaciones = model.Observaciones?.Trim();
        venta.ModificadoPor = UsuarioActual;
        venta.FechaModificacion = DateTime.Now;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Venta), venta.Id.ToString(),
            $"Actualización de la venta a {venta.Comprador}",
            valorAnterior: anterior,
            valorNuevo: $"Cantidad={venta.Cantidad:0.##}; PrecioUnitario={venta.PrecioUnitario:0.##}; Comprador={venta.Comprador}");

        TempData["Ok"] = "Venta actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // Reglas que no caben en las anotaciones del modelo.
    private async Task ValidarAsync(VentaFormViewModel model)
    {
        if (model.Fecha is { } fecha && fecha.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.Fecha), "La fecha de la venta no puede ser futura.");

        if (model.PeriodoProductivoId is { } periodoId &&
            !await _db.PeriodosProductivos.AnyAsync(p => p.Id == periodoId))
            ModelState.AddModelError(nameof(model.PeriodoProductivoId), "El período seleccionado no existe.");
    }

    private async Task CargarPeriodosAsync(VentaFormViewModel model)
    {
        model.Periodos = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.FechaInicio)
            .Select(p => new SelectListItem(p.Nombre, p.Id.ToString()))
            .ToListAsync();
    }
}
