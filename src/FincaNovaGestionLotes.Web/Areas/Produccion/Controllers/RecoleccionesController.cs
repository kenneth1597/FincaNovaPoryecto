using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Controllers;

[Area("Produccion")]
[RequiereRol(Roles.Administrador, Roles.Duenio)]
public class RecoleccionesController : Controller
{
    private readonly AppDbContext _context;

    public RecoleccionesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Produccion/Recolecciones
    public async Task<IActionResult> Index()
    {
        var recolecciones = await _context.Recolecciones
            .Include(r => r.Colaborador)
            .Include(r => r.Terreno)
            .Include(r => r.PeriodoProductivo)
            .OrderByDescending(r => r.Fecha)
            .ThenByDescending(r => r.Id)
            .ToListAsync();
        return View(recolecciones);
    }

    // GET: Produccion/Recolecciones/Crear
    public async Task<IActionResult> Crear()
    {
        await CargarListasAsync();
        return View(new Recoleccion { Fecha = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Recoleccion recoleccion)
    {
        ModelState.Remove(nameof(Recoleccion.Colaborador));
        ModelState.Remove(nameof(Recoleccion.Terreno));
        ModelState.Remove(nameof(Recoleccion.PeriodoProductivo));
        ModelState.Remove(nameof(Recoleccion.PeriodoTerreno));
        ValidarFecha(recoleccion);

        if (ModelState.IsValid)
        {
            try
            {
                recoleccion.KgPorCajuela = 12.5m;
                recoleccion.RegistradoPor = HttpContext.Session.GetString("UsuarioNombre")
                                ?? User.Identity?.Name
                                ?? "Administrador";

                _context.Add(recoleccion);
                await _context.SaveChangesAsync();

                TempData["Ok"] = "Recolección diaria registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Capturamos el error de base de datos (como el conflicto de PeriodoTerreno) y mostramos advertencia amigable
                ModelState.AddModelError(string.Empty, "Error al guardar: El terreno seleccionado no está asociado al período productivo seleccionado. Por favor, verifique la asignación.");
            }
        }

        await CargarListasAsync(recoleccion);
        return View(recoleccion);
    }

    // GET: Produccion/Recolecciones/Editar/5
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recoleccion = await _context.Recolecciones.FindAsync(id);
        if (recoleccion == null)
        {
            return NotFound();
        }

        await CargarListasAsync(recoleccion);
        return View(recoleccion);
    }

    // POST: Produccion/Recolecciones/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Recoleccion recoleccion)
    {
        if (id != recoleccion.Id)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(Recoleccion.Colaborador));
        ModelState.Remove(nameof(Recoleccion.Terreno));
        ModelState.Remove(nameof(Recoleccion.PeriodoProductivo));
        ModelState.Remove(nameof(Recoleccion.PeriodoTerreno));
        ValidarFecha(recoleccion);

        if (ModelState.IsValid)
        {
            try
            {
                recoleccion.KgPorCajuela = 12.5m;

                _context.Update(recoleccion);
                await _context.SaveChangesAsync();

                TempData["Ok"] = "Recolección actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Recolecciones.Any(e => e.Id == recoleccion.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                // Capturamos el error de base de datos también en la edición
                ModelState.AddModelError(string.Empty, "Error al actualizar: El terreno seleccionado no está asociado al período productivo seleccionado. Por favor, verifique la asignación.");
            }
        }

        await CargarListasAsync(recoleccion);
        return View(recoleccion);
    }


    // GET: Produccion/Recolecciones/Eliminar/5
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var recoleccion = await _context.Recolecciones
            .Include(r => r.Colaborador)
            .Include(r => r.Terreno)
            .Include(r => r.PeriodoProductivo)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (recoleccion == null)
        {
            return NotFound();
        }

        return View(recoleccion);
    }

    // POST: Produccion/Recolecciones/Eliminar/5
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmed(int id)
    {
        var recoleccion = await _context.Recolecciones.FindAsync(id);
        if (recoleccion != null)
        {
            _context.Recolecciones.Remove(recoleccion);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Recolección eliminada correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Una recolección no puede tener fecha futura.
    private void ValidarFecha(Recoleccion recoleccion)
    {
        if (recoleccion.Fecha.Date > DateTime.Today)
            ModelState.AddModelError(nameof(Recoleccion.Fecha), "La fecha de la recolección no puede ser futura.");
    }

    // Carga las listas desplegables del formulario; con la recolección, deja marcada la opción elegida.
    private async Task CargarListasAsync(Recoleccion? r = null)
    {
        ViewData["Colaboradores"] = new SelectList(
            await _context.Colaboradores.Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync(),
            "Id", "Nombre", r?.ColaboradorId);

        ViewData["Terrenos"] = new SelectList(
            await _context.Terrenos.Where(t => t.Eliminado == false).OrderBy(t => t.Nombre).ToListAsync(),
            "Id", "Nombre", r?.TerrenoId);

        // Se muestra el nombre del período, no su número.
        ViewData["PeriodosProductivos"] = new SelectList(
            await _context.PeriodosProductivos.OrderByDescending(p => p.FechaInicio).ToListAsync(),
            "Id", "Nombre", r?.PeriodoProductivoId);
    }

    // GET: Produccion/Recolecciones/ConsultaPeriodo
    public async Task<IActionResult> ConsultaPeriodo(int? periodoId)
    {
        // Cargamos los períodos para el selector de filtro
        ViewData["PeriodosProductivos"] = new SelectList(await _context.PeriodosProductivos.ToListAsync(), "Id", "Id", periodoId);

        var query = _context.Recolecciones
            .Include(r => r.Colaborador)
            .Include(r => r.Terreno)
            .Include(r => r.PeriodoProductivo)
            .AsQueryable();

        // Si el usuario seleccionó un período específico en el filtro, lo aplicamos
        if (periodoId.HasValue)
        {
            query = query.Where(r => r.PeriodoProductivoId == periodoId.Value);
        }

        var listaRecolecciones = await query.OrderByDescending(r => r.Fecha).ToListAsync();

        // Calculamos totales rápidos para mostrar en tarjetas resumen
        ViewBag.TotalCajuelas = listaRecolecciones.Sum(r => r.Cajuelas);
        ViewBag.TotalKilos = listaRecolecciones.Sum(r => r.PesoEstimadoKg);

        return View(listaRecolecciones);
    }



    // GET: Produccion/Recolecciones/CalcularPagos
    public async Task<IActionResult> CalcularPagos(DateTime? fechaInicio, DateTime? fechaFin, decimal tarifaCajuela = 1500m)
    {
        // Si no se seleccionan fechas, por defecto consultamos la última semana
        var inicio = fechaInicio ?? DateTime.Today.AddDays(-7);
        var fin = fechaFin ?? DateTime.Today;

        ViewBag.FechaInicio = inicio.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fin.ToString("yyyy-MM-dd");
        ViewBag.TarifaCajuela = tarifaCajuela;

        // Consultamos las recolecciones en ese rango de fechas
        var recolecciones = await _context.Recolecciones
            .Include(r => r.Colaborador)
            .Where(r => r.Fecha >= inicio && r.Fecha <= fin)
            .ToListAsync();

        // Agrupamos por colaborador para calcular su total de cajuelas y el pago correspondiente
        var resumenPagos = recolecciones
            .GroupBy(r => new { r.ColaboradorId, NombreColaborador = r.Colaborador != null ? r.Colaborador.Nombre : "Sin Nombre" })
            .Select(g => new
            {
                ColaboradorId = g.Key.ColaboradorId,
                Nombre = g.Key.NombreColaborador,
                TotalCajuelas = g.Sum(r => r.Cajuelas),
                TotalKilos = g.Sum(r => r.PesoEstimadoKg),
                MontoAPagar = g.Sum(r => r.Cajuelas) * tarifaCajuela
            })
            .ToList();

        ViewBag.GranTotalPagar = resumenPagos.Sum(x => x.MontoAPagar);
        ViewBag.GranTotalCajuelas = resumenPagos.Sum(x => x.TotalCajuelas);

        return View(resumenPagos);
    }
}
