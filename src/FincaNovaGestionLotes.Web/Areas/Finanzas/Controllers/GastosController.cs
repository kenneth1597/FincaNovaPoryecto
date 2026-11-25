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
public class GastosController : Controller
{
    private const long TamanoMaximoBytes = 5 * 1024 * 1024;

    // Solo se aceptan estos tipos de comprobante (HU-52).
    private static readonly Dictionary<string, string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png"
    };

    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly IWebHostEnvironment _env;

    public GastosController(AppDbContext db, IAuditService audit, IWebHostEnvironment env)
    {
        _db = db;
        _audit = audit;
        _env = env;
    }

    private string UsuarioActual => HttpContext.Session.GetString("UsuarioNombre") ?? "Administrador";

    // Los comprobantes se guardan fuera de wwwroot: solo se descargan por la acción Comprobante.
    private string CarpetaComprobantes => Path.Combine(_env.ContentRootPath, "App_Data", "comprobantes");

    // HU-19: historial de gastos con filtro por fechas y categoría, y total gastado.
    [HttpGet]
    public async Task<IActionResult> Index(GastoFiltroViewModel filtro)
    {
        var query = _db.Gastos.AsNoTracking().AsQueryable();

        if (filtro.Desde.HasValue)
            query = query.Where(g => g.Fecha >= filtro.Desde.Value.Date);

        if (filtro.Hasta.HasValue)
            query = query.Where(g => g.Fecha <= filtro.Hasta.Value.Date);

        if (filtro.CategoriaId.HasValue)
            query = query.Where(g => g.CategoriaGastoId == filtro.CategoriaId.Value);

        filtro.TotalGastado = await query.SumAsync(g => (decimal?)g.Monto) ?? 0;

        var (resultados, paginacion) = await query
            .OrderByDescending(g => g.Fecha)
            .ThenByDescending(g => g.Id)
            .Select(g => new GastoListItemViewModel
            {
                Id = g.Id,
                Fecha = g.Fecha,
                Descripcion = g.Descripcion,
                Proveedor = g.Proveedor,
                Categoria = g.CategoriaGasto.Nombre,
                Terreno = g.Terreno != null ? g.Terreno.Codigo + " · " + g.Terreno.Nombre : null,
                Monto = g.Monto,
                TieneComprobante = g.AdjuntoRuta != null
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;
        filtro.Categorias = await _db.CategoriasGasto.AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToListAsync();
        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new GastoFormViewModel();
        await CargarOpcionesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(GastoFormViewModel model)
    {
        await ValidarAsync(model);

        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync(model);
            return View(model);
        }

        var gasto = new Gasto
        {
            Fecha = model.Fecha!.Value.Date,
            Monto = model.Monto!.Value,
            CategoriaGastoId = model.CategoriaGastoId!.Value,
            TerrenoId = model.TerrenoId,
            PeriodoProductivoId = model.PeriodoProductivoId,
            Descripcion = model.Descripcion.Trim(),
            Proveedor = model.Proveedor?.Trim(),
            RegistradoPor = UsuarioActual,
            CreadoPor = UsuarioActual
        };

        string? archivoGuardado = null;
        if (model.Comprobante is { Length: > 0 })
            archivoGuardado = await GuardarComprobanteAsync(model.Comprobante, gasto);

        try
        {
            _db.Gastos.Add(gasto);
            await _db.SaveChangesAsync();
        }
        catch
        {
            // Si no se pudo guardar el gasto, no se deja el archivo huérfano.
            BorrarArchivo(archivoGuardado);
            throw;
        }

        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Gasto), gasto.Id.ToString(),
            $"Gasto de ₡{gasto.Monto:N2}: {gasto.Descripcion}" +
            (gasto.AdjuntoRuta is null ? "" : " (con comprobante)"),
            valorNuevo: $"CategoriaGastoId={gasto.CategoriaGastoId}; Monto={gasto.Monto:0.##}");

        TempData["Ok"] = "Gasto registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var gasto = await _db.Gastos.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
        if (gasto is null) return NotFound();

        var model = new GastoFormViewModel
        {
            Id = gasto.Id,
            Fecha = gasto.Fecha,
            Monto = gasto.Monto,
            CategoriaGastoId = gasto.CategoriaGastoId,
            TerrenoId = gasto.TerrenoId,
            PeriodoProductivoId = gasto.PeriodoProductivoId,
            Descripcion = gasto.Descripcion,
            Proveedor = gasto.Proveedor,
            ComprobanteActual = gasto.AdjuntoNombreOriginal
        };
        await CargarOpcionesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, GastoFormViewModel model)
    {
        if (id != model.Id) return NotFound();

        var gasto = await _db.Gastos.FirstOrDefaultAsync(g => g.Id == id);
        if (gasto is null) return NotFound();

        await ValidarAsync(model);

        if (!ModelState.IsValid)
        {
            model.ComprobanteActual = gasto.AdjuntoNombreOriginal;
            await CargarOpcionesAsync(model);
            return View(model);
        }

        var anterior = $"Monto={gasto.Monto:0.##}; CategoriaGastoId={gasto.CategoriaGastoId}; Descripcion={gasto.Descripcion}";
        var archivoAnterior = gasto.AdjuntoRuta;

        gasto.Fecha = model.Fecha!.Value.Date;
        gasto.Monto = model.Monto!.Value;
        gasto.CategoriaGastoId = model.CategoriaGastoId!.Value;
        gasto.TerrenoId = model.TerrenoId;
        gasto.PeriodoProductivoId = model.PeriodoProductivoId;
        gasto.Descripcion = model.Descripcion.Trim();
        gasto.Proveedor = model.Proveedor?.Trim();
        gasto.ModificadoPor = UsuarioActual;
        gasto.FechaModificacion = DateTime.Now;

        string? archivoNuevo = null;
        var reemplazaArchivo = false;

        if (model.Comprobante is { Length: > 0 })
        {
            archivoNuevo = await GuardarComprobanteAsync(model.Comprobante, gasto);
            reemplazaArchivo = true;
        }
        else if (model.QuitarComprobante && gasto.AdjuntoRuta is not null)
        {
            gasto.AdjuntoNombreOriginal = null;
            gasto.AdjuntoRuta = null;
            gasto.AdjuntoTipoContenido = null;
            gasto.AdjuntoTamanoBytes = null;
            reemplazaArchivo = true;
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            BorrarArchivo(archivoNuevo);
            throw;
        }

        // Solo cuando el cambio ya quedó guardado se elimina el archivo anterior.
        if (reemplazaArchivo) BorrarArchivo(archivoAnterior);

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Gasto), gasto.Id.ToString(),
            $"Actualización del gasto «{gasto.Descripcion}»",
            valorAnterior: anterior,
            valorNuevo: $"Monto={gasto.Monto:0.##}; CategoriaGastoId={gasto.CategoriaGastoId}; Descripcion={gasto.Descripcion}");

        TempData["Ok"] = "Gasto actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // HU-52: descarga del comprobante adjunto.
    [HttpGet]
    public async Task<IActionResult> Comprobante(int id)
    {
        var gasto = await _db.Gastos.AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new { g.AdjuntoRuta, g.AdjuntoNombreOriginal, g.AdjuntoTipoContenido })
            .FirstOrDefaultAsync();

        if (gasto?.AdjuntoRuta is null) return NotFound();

        // GetFileName evita que una ruta guardada pueda salirse de la carpeta de comprobantes.
        var ruta = Path.Combine(CarpetaComprobantes, Path.GetFileName(gasto.AdjuntoRuta));
        if (!System.IO.File.Exists(ruta)) return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return PhysicalFile(ruta, gasto.AdjuntoTipoContenido ?? "application/octet-stream",
            gasto.AdjuntoNombreOriginal);
    }

    // Reglas que no caben en las anotaciones del modelo.
    private async Task ValidarAsync(GastoFormViewModel model)
    {
        if (model.Fecha is { } fecha && fecha.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.Fecha), "La fecha del gasto no puede ser futura.");

        if (model.CategoriaGastoId is { } categoriaId &&
            !await _db.CategoriasGasto.AnyAsync(c => c.Id == categoriaId && c.Activo))
            ModelState.AddModelError(nameof(model.CategoriaGastoId), "La categoría seleccionada no existe.");

        if (model.TerrenoId is { } terrenoId &&
            !await _db.Terrenos.AnyAsync(t => t.Id == terrenoId && !t.Eliminado))
            ModelState.AddModelError(nameof(model.TerrenoId), "La finca o lote seleccionado no existe.");

        if (model.PeriodoProductivoId is { } periodoId &&
            !await _db.PeriodosProductivos.AnyAsync(p => p.Id == periodoId))
            ModelState.AddModelError(nameof(model.PeriodoProductivoId), "El período seleccionado no existe.");

        var errorComprobante = await ValidarComprobanteAsync(model.Comprobante);
        if (errorComprobante is not null)
            ModelState.AddModelError(nameof(model.Comprobante), errorComprobante);
    }

    // Revisa tamaño, extensión y que el contenido sea realmente de ese tipo (no solo el nombre).
    private static async Task<string?> ValidarComprobanteAsync(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0) return null;

        if (archivo.Length > TamanoMaximoBytes)
            return "El comprobante no puede pesar más de 5 MB.";

        var extension = Path.GetExtension(archivo.FileName);
        if (!TiposPermitidos.ContainsKey(extension))
            return "El comprobante debe ser un archivo PDF, JPG o PNG.";

        var cabecera = new byte[8];
        await using var flujo = archivo.OpenReadStream();
        var leidos = await flujo.ReadAsync(cabecera.AsMemory(0, cabecera.Length));

        if (!CabeceraCoincide(extension.ToLowerInvariant(), cabecera.AsSpan(0, leidos)))
            return "El contenido del archivo no corresponde a un PDF, JPG o PNG válido.";

        return null;
    }

    private static bool CabeceraCoincide(string extension, ReadOnlySpan<byte> cabecera) => extension switch
    {
        ".pdf" => cabecera.StartsWith("%PDF"u8),
        ".jpg" or ".jpeg" => cabecera.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }),
        ".png" => cabecera.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
        _ => false
    };

    // Guarda el archivo con un nombre interno aleatorio y anota sus datos en el gasto.
    private async Task<string> GuardarComprobanteAsync(IFormFile archivo, Gasto gasto)
    {
        Directory.CreateDirectory(CarpetaComprobantes);

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        var nombreInterno = $"{Guid.NewGuid():N}{extension}";

        await using (var destino = System.IO.File.Create(Path.Combine(CarpetaComprobantes, nombreInterno)))
        {
            await archivo.CopyToAsync(destino);
        }

        var nombreOriginal = Path.GetFileName(archivo.FileName);
        gasto.AdjuntoNombreOriginal = nombreOriginal.Length > 255 ? nombreOriginal[..255] : nombreOriginal;
        gasto.AdjuntoRuta = nombreInterno;
        gasto.AdjuntoTipoContenido = TiposPermitidos[extension];
        gasto.AdjuntoTamanoBytes = archivo.Length;

        return nombreInterno;
    }

    private void BorrarArchivo(string? nombreInterno)
    {
        if (string.IsNullOrEmpty(nombreInterno)) return;

        var ruta = Path.Combine(CarpetaComprobantes, Path.GetFileName(nombreInterno));
        if (System.IO.File.Exists(ruta)) System.IO.File.Delete(ruta);
    }

    private async Task CargarOpcionesAsync(GastoFormViewModel model)
    {
        model.Categorias = await _db.CategoriasGasto.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToListAsync();

        model.Terrenos = await _db.Terrenos.AsNoTracking()
            .Where(t => !t.Eliminado)
            .OrderBy(t => t.Codigo)
            .Select(t => new SelectListItem(t.Codigo + " · " + t.Nombre, t.Id.ToString()))
            .ToListAsync();

        model.Periodos = await _db.PeriodosProductivos.AsNoTracking()
            .OrderByDescending(p => p.FechaInicio)
            .Select(p => new SelectListItem(p.Nombre, p.Id.ToString()))
            .ToListAsync();
    }
}
