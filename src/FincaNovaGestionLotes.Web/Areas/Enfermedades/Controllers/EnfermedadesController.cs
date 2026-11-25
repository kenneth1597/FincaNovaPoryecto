using FincaNovaGestionLotes.Web.Areas.Enfermedades.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.Services.Inventario;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Enfermedades.Controllers;

[Area("Enfermedades")]
public class EnfermedadesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly IInventarioService _inventario;

    public EnfermedadesController(
        AppDbContext db,
        IAuditService audit,
        IInventarioService inventario)
    {
        _db = db;
        _audit = audit;
        _inventario = inventario;
    }

    private Task<List<InsumoTratamientoOpcion>> InsumosDisponiblesAsync() =>
        _db.Insumos.AsNoTracking()
            .Where(i => i.Activo)
            .OrderBy(i => i.Nombre)
            .Select(i => new InsumoTratamientoOpcion
            {
                Id = i.Id,
                Nombre = i.Nombre,
                Unidad = i.UnidadMedida.Nombre,
                StockActual = i.StockActual
            })
            .ToListAsync();


    [HttpGet]
    public async Task<IActionResult> Index(EnfermedadFiltroViewModel filtro)
    {
        var query = _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .AsQueryable();

        if (filtro.TerrenoId.HasValue)
            query = query.Where(e => e.TerrenoId == filtro.TerrenoId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Nombre))
        {
            var nombre = filtro.Nombre.Trim();

            query = query.Where(e =>
                e.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Severidad))
        {
            query = query.Where(e =>
                e.Severidad == filtro.Severidad);
        }

        if (filtro.Estado == "Abiertas")
            query = query.Where(e => e.Estado != EstadoEnfermedad.Cerrada);
        else if (filtro.Estado == "Cerradas")
            query = query.Where(e => e.Estado == EstadoEnfermedad.Cerrada);

        if (filtro.Desde.HasValue)
        {
            query = query.Where(e =>
                e.FechaDeteccion >= filtro.Desde.Value);
        }

        if (filtro.Hasta.HasValue)
        {
            var hasta = filtro.Hasta.Value.Date.AddDays(1);

            query = query.Where(e =>
                e.FechaDeteccion < hasta);
        }

        var enfermedades = await query
            .OrderByDescending(e => e.FechaDeteccion)
            .ToListAsync();

        var ids = enfermedades.Select(e => e.Id).ToList();

        var nombresPorLote = enfermedades
            .GroupBy(e => new
            {
                e.TerrenoId,
                Nombre = e.Nombre.ToLower()
            })
            .ToDictionary(
                g => $"{g.Key.TerrenoId}|{g.Key.Nombre}",
                g => g.OrderBy(x => x.FechaDeteccion).ToList());

        filtro.Resultados = enfermedades
            .Select(e =>
            {
                var key = $"{e.TerrenoId}|{e.Nombre.ToLower()}";

                var lista = nombresPorLote[key];

                var indice = lista.FindIndex(x => x.Id == e.Id);

                return new EnfermedadListaViewModel
                {
                    Id = e.Id,
                    TerrenoId = e.TerrenoId,
                    TerrenoCodigo = e.Terreno.Codigo,
                    Nombre = e.Nombre,
                    FechaDeteccion = e.FechaDeteccion,
                    Severidad = e.Severidad,
                    Descripcion = e.Descripcion,
                    Atendida = e.Atendida,
                    EsRecurrencia = indice > 0,
                    Estado = e.Estado
                };
            })
            .ToList();

        var (paginaEnfermedades, infoEnfermedades) = filtro.Resultados.Paginar(filtro.Pagina);
        filtro.Resultados = paginaEnfermedades;
        filtro.Paginacion = infoEnfermedades;

        filtro.Terrenos = await ObtenerTerrenosAsync();

        return View(filtro);
    }


    [HttpGet]
    public async Task<IActionResult> Crear(int? terrenoId = null)
    {
        var model = new EnfermedadCrearViewModel
        {
            TerrenoId = terrenoId ?? 0,
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(EnfermedadCrearViewModel model)
    {
        model.Terrenos = await ObtenerTerrenosAsync();

        if (!await _db.Terrenos.AnyAsync(
                l => l.Id == model.TerrenoId && !l.Eliminado))
        {
            ModelState.AddModelError(
                nameof(model.TerrenoId),
                "La finca o el lote seleccionado no existe.");
        }

        if (!string.IsNullOrWhiteSpace(model.Nombre) && !CatalogoEnfermedades.Existe(model.Nombre))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Seleccione una enfermedad de la lista.");
        }

        if (!ModelState.IsValid)
            return View(model);

        var enfermedad = new Enfermedad
        {
            TerrenoId = model.TerrenoId,
            Nombre = model.Nombre.Trim(),
            FechaDeteccion = model.FechaDeteccion,
            Severidad = model.Severidad.Trim(),
            Descripcion = model.Descripcion.Trim(),
            Observaciones = model.Observaciones?.Trim(),
            Atendida = false,
            FechaAtendida = null,
            AtendidaPor = null
        };

        _db.Enfermedades.Add(enfermedad);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Registro de enfermedad {enfermedad.Nombre} en el lote {enfermedad.TerrenoId}");

        TempData["Ok"] =
            "La enfermedad fue registrada correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = enfermedad.Id });
    }
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var enfermedad = await _db.Enfermedades
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada y ya no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var model = new EnfermedadEditarViewModel
        {
            Id = enfermedad.Id,
            TerrenoId = enfermedad.TerrenoId,
            Nombre = enfermedad.Nombre,
            FechaDeteccion = enfermedad.FechaDeteccion,
            Severidad = enfermedad.Severidad,
            Descripcion = enfermedad.Descripcion,
            Observaciones = enfermedad.Observaciones,
            Terrenos = await ObtenerTerrenosAsync()
        };

        return View(model);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        EnfermedadEditarViewModel model)
    {
        model.Terrenos = await ObtenerTerrenosAsync();

        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == model.Id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada y ya no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedad.Id });
        }

        var loteExiste = await _db.Terrenos.AnyAsync(
            l => l.Id == model.TerrenoId &&
                 !l.Eliminado);

        if (!loteExiste)
        {
            ModelState.AddModelError(
                nameof(model.TerrenoId),
                "La finca o el lote seleccionado no existe.");
        }

        var nombreSinCambio = string.Equals(model.Nombre?.Trim(), enfermedad.Nombre, StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(model.Nombre) && !nombreSinCambio && !CatalogoEnfermedades.Existe(model.Nombre))
        {
            ModelState.AddModelError(
                nameof(model.Nombre),
                "Seleccione una enfermedad de la lista.");
        }

        if (!ModelState.IsValid)
            return View(model);

        enfermedad.TerrenoId = model.TerrenoId;
        enfermedad.Nombre = model.Nombre.Trim();
        enfermedad.FechaDeteccion = model.FechaDeteccion;
        enfermedad.Severidad = model.Severidad.Trim();
        enfermedad.Descripcion = model.Descripcion.Trim();
        enfermedad.Observaciones = model.Observaciones?.Trim();

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Modificación de la enfermedad {enfermedad.Nombre}");

        TempData["Ok"] =
            "La información de la enfermedad fue actualizada correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id = enfermedad.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var enfermedad = await _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        return View(enfermedad);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int id, [Bind(Prefix = "Cierre")] CierreEnfermedadViewModel cierre)
    {
        var enfermedad = await _db.Enfermedades
            .Include(e => e.Tratamientos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad ya estaba cerrada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var errores = new List<string>();

        if (!enfermedad.Tratamientos.Any())
            errores.Add("No se puede cerrar una enfermedad sin haber aplicado al menos un tratamiento.");

        if (cierre.ResultadoCierre is not { } resultado || !Enum.IsDefined(resultado))
            errores.Add("Seleccione el resultado final.");

        if (string.IsNullOrWhiteSpace(cierre.ComentarioCierre))
            errores.Add("El comentario de cierre es obligatorio.");
        else if (cierre.ComentarioCierre.Trim().Length > 500)
            errores.Add("El comentario de cierre no puede superar 500 caracteres.");

        if (cierre.TratamientoEfectivoId is { } tratamientoId &&
            enfermedad.Tratamientos.All(t => t.Id != tratamientoId))
            errores.Add("El tratamiento seleccionado no pertenece a esta enfermedad.");

        if (!cierre.Confirmo)
            errores.Add("Debe confirmar que verificó en campo que la enfermedad está controlada.");

        if (errores.Count > 0)
        {
            TempData["ErroresCierre"] = string.Join("|", errores);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var usuario = HttpContext.Session.GetString("UsuarioNombre");

        enfermedad.Estado = EstadoEnfermedad.Cerrada;
        enfermedad.FechaCierre = DateTime.UtcNow;
        enfermedad.CerradaPor = usuario;
        enfermedad.ResultadoCierre = cierre.ResultadoCierre;
        enfermedad.ComentarioCierre = cierre.ComentarioCierre!.Trim();
        enfermedad.TratamientoEfectivoId = cierre.TratamientoEfectivoId;
        enfermedad.ModificadoPor = usuario;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Cierre de la enfermedad {enfermedad.Nombre}: {Ui.ResultadoCierreTexto(cierre.ResultadoCierre!.Value)}");

        TempData["Ok"] = "La enfermedad fue cerrada y quedó en el histórico.";

        return RedirectToAction(nameof(Detalle), new { id });
    }


    [HttpGet]
    public async Task<IActionResult> CrearTratamiento(int enfermedadId)
    {
        var estado = await _db.Enfermedades
            .Where(e => e.Id == enfermedadId)
            .Select(e => (EstadoEnfermedad?)e.Estado)
            .FirstOrDefaultAsync();

        if (estado is null)
            return NotFound();

        if (estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada; ya no admite tratamientos.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedadId });
        }

        var model = new TratamientoCrearViewModel
        {
            EnfermedadId = enfermedadId,
            Productos = new List<ProductoViewModel>
            {
                new ProductoViewModel()
            },
            Insumos = await InsumosDisponiblesAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearTratamiento(
        TratamientoCrearViewModel model)
    {
        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == model.EnfermedadId);

        if (enfermedad is null)
            return NotFound();

        if (enfermedad.Estado == EstadoEnfermedad.Cerrada)
        {
            TempData["Error"] = "La enfermedad está cerrada; ya no admite tratamientos.";
            return RedirectToAction(nameof(Detalle), new { id = enfermedad.Id });
        }

        if (model.Efectividad is { } efectividad && !Enum.IsDefined(efectividad))
        {
            ModelState.AddModelError(
                nameof(model.Efectividad),
                "La efectividad indicada no es válida.");
        }

        // Las filas que el usuario agregó y dejó en blanco no cuentan.
        for (var i = model.Productos.Count - 1; i >= 0; i--)
        {
            if (model.Productos[i].EstaVacio && model.Productos.Count > 1)
            {
                foreach (var clave in ModelState.Keys.Where(k => k.StartsWith($"Productos[{i}]")).ToList())
                    ModelState.Remove(clave);
                model.Productos.RemoveAt(i);
            }
        }

        if (model.Productos.Count == 0 || model.Productos.All(p => p.EstaVacio))
        {
            ModelState.AddModelError(
                nameof(model.Productos),
                "Debe registrar al menos un producto utilizado.");
        }

        // Cada producto viene del inventario o se describe a mano (nombre y unidad).
        var insumos = new Dictionary<int, Domain.Inventario.Insumo>();
        for (var i = 0; i < model.Productos.Count; i++)
        {
            var producto = model.Productos[i];
            if (producto.InsumoId is { } insumoId)
            {
                var insumo = await _db.Insumos.Include(x => x.UnidadMedida)
                    .FirstOrDefaultAsync(x => x.Id == insumoId && x.Activo);
                if (insumo is null)
                    ModelState.AddModelError($"Productos[{i}].InsumoId", "El insumo seleccionado no está disponible.");
                else
                    insumos[insumoId] = insumo;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(producto.NombreProducto))
                    ModelState.AddModelError($"Productos[{i}].NombreProducto", "El nombre del producto es obligatorio.");
                if (string.IsNullOrWhiteSpace(producto.Unidad))
                    ModelState.AddModelError($"Productos[{i}].Unidad", "La unidad es obligatoria.");
            }
        }

        if (!ModelState.IsValid)
        {
            model.Insumos = await InsumosDisponiblesAsync();
            return View(model);
        }

        var tratamiento = new TratamientoEnfermedad
        {
            EnfermedadId = model.EnfermedadId,
            Tratamiento = model.Tratamiento.Trim(),
            FechaAplicacion = model.FechaAplicacion,
            Responsable = model.Responsable?.Trim(),
            Resultado = model.Resultado?.Trim(),
            Efectividad = model.Efectividad,
            Observaciones = model.Observaciones?.Trim()
        };

        if (enfermedad.Estado == EstadoEnfermedad.Activa)
            enfermedad.Estado = EstadoEnfermedad.EnTratamiento;

        var rebajados = new List<string>();
        try
        {
            foreach (var producto in model.Productos)
            {
                var insumo = producto.InsumoId is { } id ? insumos[id] : null;
                var productoTratamiento = new ProductoTratamiento
                {
                    NombreProducto = insumo?.Nombre ?? producto.NombreProducto!.Trim(),
                    Cantidad = producto.Cantidad,
                    Unidad = insumo?.UnidadMedida.Nombre ?? producto.Unidad!.Trim(),
                    Insumo = insumo
                };
                tratamiento.Productos.Add(productoTratamiento);

                // HU-24 esc. 1: el inventario se rebaja junto con el tratamiento, sin doble digitación.
                if (insumo is not null)
                {
                    await _inventario.RegistrarSalidaAsync(insumo, producto.Cantidad, model.FechaAplicacion,
                        $"Tratamiento de {enfermedad.Nombre}", productoTratamiento: productoTratamiento);
                    rebajados.Add(Ui.CantidadConUnidad(producto.Cantidad, insumo.UnidadMedida.Nombre) + " de " + insumo.Nombre);
                }
            }
        }
        catch (InventarioInsuficienteException ex)
        {
            // Nada se guardó todavía: se descartan el tratamiento y los rebajos preparados.
            _db.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty,
                $"No es posible registrar el tratamiento: Inventario insuficiente de {ex.Insumo.Nombre}. " +
                $"Existencias actuales: {ex.Existencias}");
            model.Insumos = await InsumosDisponiblesAsync();
            return View(model);
        }

        _db.TratamientosEnfermedad.Add(tratamiento);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            _db.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty, InventarioService.MensajeConcurrencia);
            model.Insumos = await InsumosDisponiblesAsync();
            return View(model);
        }

        await _audit.RegistrarAsync(
            AccionAuditoria.Crear,
            nameof(TratamientoEnfermedad),
            tratamiento.Id.ToString(),
            $"Tratamiento registrado para la enfermedad {enfermedad.Nombre}");

        TempData["Ok"] = rebajados.Count == 0
            ? "El tratamiento y los productos utilizados fueron registrados."
            : "El tratamiento fue registrado y se rebajó del inventario: " + string.Join(", ", rebajados) + ".";

        return RedirectToAction(
            nameof(Detalle),
            new { id = model.EnfermedadId });
    }


    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpGet]
    public async Task<IActionResult> Alertas(int pagina = 1)
    {
        var enfermedades = await _db.Enfermedades
            .AsNoTracking()
            .Include(e => e.Terreno)
            .Include(e => e.Tratamientos)
                .ThenInclude(t => t.Productos)
            .OrderBy(e => e.TerrenoId)
            .ThenBy(e => e.Nombre)
            .ThenBy(e => e.FechaDeteccion)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var alertas = enfermedades
            .GroupBy(e => new
            {
                e.TerrenoId,
                Nombre = e.Nombre.Trim().ToLower()
            })
            .SelectMany(grupo =>
                grupo
                    .OrderBy(e => e.FechaDeteccion)
                    .ThenBy(e => e.Id)
                    .Skip(1))
            .OrderByDescending(e => e.FechaDeteccion)
            .ToList();

        var (paginaAlertas, infoAlertas) = alertas.Paginar(pagina);
        ViewBag.Paginacion = infoAlertas;

        return View(paginaAlertas);
    }

    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarAtendida(int id)
    {
        var enfermedad = await _db.Enfermedades
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enfermedad is null)
            return NotFound();

        enfermedad.Atendida = true;
        enfermedad.FechaAtendida = DateTime.UtcNow;
        enfermedad.AtendidaPor = User.Identity?.Name;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            AccionAuditoria.Modificar,
            nameof(Enfermedad),
            enfermedad.Id.ToString(),
            $"Alerta de recurrencia marcada como atendida: {enfermedad.Nombre}");

        TempData["Ok"] =
            "La alerta fue marcada como atendida.";

        return RedirectToAction(nameof(Alertas));
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