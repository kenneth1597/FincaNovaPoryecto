using FincaNovaGestionLotes.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Bitacora.Controllers;

[Area("Bitacora")]
public class BitacoraController : Controller
{
    private readonly AppDbContext _db;

    public BitacoraController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? entidad,
        string? usuario,
        DateTime? desde,
        DateTime? hasta)
    {
        var query = _db.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(entidad))
        {
            entidad = entidad.Trim();

            query = query.Where(x =>
                x.Entidad.Contains(entidad));
        }

        if (!string.IsNullOrWhiteSpace(usuario))
        {
            usuario = usuario.Trim();

            query = query.Where(x =>
                x.Usuario != null &&
                x.Usuario.Contains(usuario));
        }

        if (desde.HasValue)
        {
            query = query.Where(x =>
                x.FechaHora >= desde.Value);
        }

        if (hasta.HasValue)
        {
            var fechaHasta =
                hasta.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.FechaHora < fechaHasta);
        }

        ViewBag.Entidad = entidad;
        ViewBag.Usuario = usuario;
        ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");

        var registros = await query
            .OrderByDescending(x => x.FechaHora)
            .Take(500)
            .ToListAsync();

        return View(registros);
    }
}