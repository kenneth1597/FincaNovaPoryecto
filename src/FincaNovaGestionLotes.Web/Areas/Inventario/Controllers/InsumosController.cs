using FincaNovaGestionLotes.Web.Areas.Inventario.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Inventario;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.Services.Inventario;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Inventario.Controllers;

[Area("Inventario")]
[RequiereRol(Roles.Trabajador, Roles.Capataz, Roles.Administrador, Roles.Duenio)]
public class InsumosController : Controller
{
    private const string MensajeVencimiento = "Para insumos químicos es obligatorio registrar la fecha de caducidad";

    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly IInventarioService _inventario;

    public InsumosController(AppDbContext db, IAuditService audit, IInventarioService inventario)
    {
        _db = db;
        _audit = audit;
        _inventario = inventario;
    }

    private bool EsAdmin => Roles.EsAdmin(HttpContext);

    // HU-23: existencias actuales, búsqueda por coincidencia y alerta visual de stock mínimo.
    [HttpGet]
    public async Task<IActionResult> Index(InsumoFiltroViewModel filtro)
    {
        var query = _db.Insumos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var texto = filtro.Q.Trim();
            query = query.Where(i => i.Nombre.Contains(texto) || i.CategoriaInsumo.Nombre.Contains(texto));
        }

        if (filtro.CategoriaId.HasValue)
            query = query.Where(i => i.CategoriaInsumoId == filtro.CategoriaId.Value);

        query = filtro.Estado switch
        {
            "Inactivos" => query.Where(i => !i.Activo),
            "Todos" => query,
            _ => query.Where(i => i.Activo)
        };

        if (filtro.SoloBajoMinimo)
            query = query.Where(i => i.Activo && i.StockMinimo > 0 && i.StockActual <= i.StockMinimo);

        var hoy = DateTime.Today;
        var (resultados, paginacion) = await query
            // Los que están bajo el mínimo primero, para que se vean de inmediato.
            .OrderByDescending(i => i.Activo && i.StockMinimo > 0 && i.StockActual <= i.StockMinimo)
            .ThenBy(i => i.Nombre)
            .Select(i => new InsumoListItemViewModel
            {
                Id = i.Id,
                Nombre = i.Nombre,
                Categoria = i.CategoriaInsumo.Nombre,
                Unidad = i.UnidadMedida.Nombre,
                StockActual = i.StockActual,
                StockMinimo = i.StockMinimo,
                Activo = i.Activo,
                ProximoVencimiento = i.Movimientos
                    .Where(m => m.Tipo == TipoMovimientoInventario.Entrada && m.FechaVencimiento >= hoy)
                    .Min(m => m.FechaVencimiento)
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;
        filtro.Categorias = await _db.CategoriasInsumo.AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new OpcionViewModel { Id = c.Id, Texto = c.Nombre })
            .ToListAsync();
        filtro.AlertasActivas = await _db.AlertasStock.CountAsync(a => a.Activa);
        filtro.EsAdmin = EsAdmin;

        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, int pagina = 1)
    {
        var vm = await _db.Insumos.AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new InsumoDetalleViewModel
            {
                Id = i.Id,
                Nombre = i.Nombre,
                Categoria = i.CategoriaInsumo.Nombre,
                RequiereVencimiento = i.CategoriaInsumo.RequiereVencimiento,
                Unidad = i.UnidadMedida.Nombre,
                StockActual = i.StockActual,
                StockMinimo = i.StockMinimo,
                Descripcion = i.Descripcion,
                Activo = i.Activo
            })
            .FirstOrDefaultAsync();

        if (vm is null) return NotFound();

        var (movimientos, paginacion) = await _db.MovimientosInventario.AsNoTracking()
            .Where(m => m.InsumoId == id)
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.Id)
            .Select(m => new MovimientoItemViewModel
            {
                Fecha = m.Fecha,
                Tipo = m.Tipo,
                Cantidad = m.Cantidad,
                StockResultante = m.StockResultante,
                FechaVencimiento = m.FechaVencimiento,
                Origen = m.ProductoTratamiento != null
                    ? "Tratamiento de " + m.ProductoTratamiento.TratamientoEnfermedad.Enfermedad.Nombre
                    : m.LaborInsumo != null
                        ? "Labor en " + m.LaborInsumo.Labor.Terreno.Codigo
                        : m.Tipo == TipoMovimientoInventario.Entrada ? "Ingreso al inventario" : null,
                Observaciones = m.Observaciones,
                RegistradoPor = m.RegistradoPor
            })
            .PaginarAsync(pagina);

        vm.Movimientos = movimientos;
        vm.Paginacion = paginacion;
        vm.EsAdmin = EsAdmin;
        return View(vm);
    }

    // HU-22: registro de ingreso de fertilizantes, pesticidas o herramientas.
    [HttpGet]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Ingreso(int? insumoId = null)
    {
        var model = new IngresoFormViewModel
        {
            InsumoId = insumoId,
            EsNuevo = insumoId is null && !await _db.Insumos.AnyAsync(i => i.Activo)
        };
        await CargarOpcionesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Ingreso(IngresoFormViewModel model)
    {
        Insumo? insumo = null;
        bool requiereVencimiento;

        if (model.EsNuevo)
        {
            var nombre = model.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre del insumo es obligatorio.");
            else if (await _db.Insumos.AnyAsync(i => i.Nombre == nombre))
                ModelState.AddModelError(nameof(model.Nombre),
                    "Ya existe un insumo con ese nombre. Selecciónelo en «Insumo existente».");

            var categoria = model.CategoriaInsumoId is { } catId
                ? await _db.CategoriasInsumo.FirstOrDefaultAsync(c => c.Id == catId && c.Activo)
                : null;
            if (categoria is null)
                ModelState.AddModelError(nameof(model.CategoriaInsumoId), "Seleccione el tipo de insumo.");

            if (model.UnidadMedidaId is not { } unidadId ||
                !await _db.UnidadesMedida.AnyAsync(u => u.Id == unidadId && u.Activo))
                ModelState.AddModelError(nameof(model.UnidadMedidaId), "Seleccione la unidad de medida.");

            requiereVencimiento = categoria?.RequiereVencimiento ?? false;

            if (ModelState.IsValid)
            {
                insumo = new Insumo
                {
                    Nombre = nombre!,
                    CategoriaInsumoId = categoria!.Id,
                    UnidadMedidaId = model.UnidadMedidaId!.Value,
                    StockMinimo = model.StockMinimo ?? 0,
                    CreadoPor = HttpContext.Session.GetString("UsuarioNombre")
                };
                _db.Insumos.Add(insumo);
            }
        }
        else
        {
            insumo = model.InsumoId is { } id
                ? await _db.Insumos.Include(i => i.CategoriaInsumo).FirstOrDefaultAsync(i => i.Id == id && i.Activo)
                : null;
            if (insumo is null)
                ModelState.AddModelError(nameof(model.InsumoId), "Seleccione el insumo que ingresa.");

            requiereVencimiento = insumo?.CategoriaInsumo.RequiereVencimiento ?? false;
        }

        // HU-22 esc. 2: los químicos no se guardan sin fecha de caducidad.
        if (requiereVencimiento && model.FechaVencimiento is null)
            ModelState.AddModelError(string.Empty, MensajeVencimiento);

        if (model.FechaVencimiento is { } vence && model.Fecha is { } ingreso && vence.Date < ingreso.Date)
            ModelState.AddModelError(nameof(model.FechaVencimiento),
                "La fecha de vencimiento no puede ser anterior a la fecha de ingreso.");

        if (model.Fecha is { } fecha && fecha.Date > DateTime.Today)
            ModelState.AddModelError(nameof(model.Fecha), "La fecha de ingreso no puede ser futura.");

        if (!ModelState.IsValid)
        {
            if (insumo is not null && insumo.Id == 0)
                _db.Insumos.Remove(insumo);
            await CargarOpcionesAsync(model);
            return View(model);
        }

        await _inventario.RegistrarEntradaAsync(insumo!, model.Cantidad!.Value, model.Fecha!.Value.Date,
            model.FechaVencimiento?.Date, model.Observaciones?.Trim());
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            _db.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty, InventarioService.MensajeConcurrencia);
            await CargarOpcionesAsync(model);
            return View(model);
        }

        var unidad = await _db.UnidadesMedida.Where(u => u.Id == insumo!.UnidadMedidaId)
            .Select(u => u.Nombre).FirstAsync();
        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(MovimientoInventario), insumo!.Id.ToString(),
            $"Ingreso de {Ui.CantidadConUnidad(model.Cantidad.Value, unidad)} de {insumo.Nombre}" +
            (model.EsNuevo ? " (insumo nuevo)" : ""),
            valorAnterior: (insumo.StockActual - model.Cantidad.Value).ToString("0.##"),
            valorNuevo: insumo.StockActual.ToString("0.##"));

        TempData["Ok"] = "Inventario actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    // HU-25 esc. 1: aquí se configura el stock mínimo.
    [HttpGet]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Editar(int id)
    {
        var insumo = await _db.Insumos.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (insumo is null) return NotFound();

        var model = new InsumoEditarViewModel
        {
            Id = insumo.Id,
            Nombre = insumo.Nombre,
            CategoriaInsumoId = insumo.CategoriaInsumoId,
            UnidadMedidaId = insumo.UnidadMedidaId,
            StockMinimo = insumo.StockMinimo,
            Descripcion = insumo.Descripcion,
            Activo = insumo.Activo,
            StockActual = insumo.StockActual
        };
        await CargarOpcionesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Editar(InsumoEditarViewModel model)
    {
        var insumo = await _db.Insumos.FirstOrDefaultAsync(i => i.Id == model.Id);
        if (insumo is null) return NotFound();

        var nombre = model.Nombre.Trim();
        if (await _db.Insumos.AnyAsync(i => i.Nombre == nombre && i.Id != model.Id))
            ModelState.AddModelError(nameof(model.Nombre), "Ya existe otro insumo con ese nombre.");

        if (model.CategoriaInsumoId is { } catId && !await _db.CategoriasInsumo.AnyAsync(c => c.Id == catId))
            ModelState.AddModelError(nameof(model.CategoriaInsumoId), "Seleccione el tipo de insumo.");

        var tieneMovimientos = await _db.MovimientosInventario.AnyAsync(m => m.InsumoId == insumo.Id);
        if (tieneMovimientos && model.UnidadMedidaId != insumo.UnidadMedidaId)
            ModelState.AddModelError(nameof(model.UnidadMedidaId),
                "No se puede cambiar la unidad de un insumo que ya tiene movimientos.");
        else if (model.UnidadMedidaId is { } uniId && !await _db.UnidadesMedida.AnyAsync(u => u.Id == uniId))
            ModelState.AddModelError(nameof(model.UnidadMedidaId), "Seleccione la unidad de medida.");

        if (!ModelState.IsValid)
        {
            model.StockActual = insumo.StockActual;
            await CargarOpcionesAsync(model);
            return View(model);
        }

        var minimoAnterior = insumo.StockMinimo;
        insumo.Nombre = nombre;
        insumo.CategoriaInsumoId = model.CategoriaInsumoId!.Value;
        insumo.UnidadMedidaId = model.UnidadMedidaId!.Value;
        insumo.StockMinimo = model.StockMinimo!.Value;
        insumo.Descripcion = model.Descripcion?.Trim();
        insumo.Activo = model.Activo;
        insumo.ModificadoPor = HttpContext.Session.GetString("UsuarioNombre");

        // Cambiar el mínimo o inactivar puede abrir o cerrar la alerta.
        await _inventario.SincronizarAlertaAsync(insumo);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Error"] = InventarioService.MensajeConcurrencia;
            return RedirectToAction(nameof(Editar), new { id = model.Id });
        }

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Insumo), insumo.Id.ToString(),
            $"Actualización del insumo {insumo.Nombre}",
            valorAnterior: $"StockMinimo={minimoAnterior:0.##}",
            valorNuevo: $"StockMinimo={insumo.StockMinimo:0.##}; Activo={insumo.Activo}");

        TempData["Ok"] = $"Insumo «{insumo.Nombre}» actualizado.";
        return RedirectToAction(nameof(Detalle), new { id = insumo.Id });
    }

    // HU-25: alertas activas hasta que el insumo se reabastece.
    [HttpGet]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Alertas()
    {
        var alertas = _db.AlertasStock.AsNoTracking()
            .Select(a => new
            {
                a.Activa,
                Item = new AlertaItemViewModel
                {
                    InsumoId = a.InsumoId,
                    Insumo = a.Insumo.Nombre,
                    Unidad = a.Insumo.UnidadMedida.Nombre,
                    StockActual = a.Activa ? a.Insumo.StockActual : a.StockAlMomento,
                    StockMinimo = a.StockMinimo,
                    FechaAlerta = a.FechaAlerta,
                    FechaResolucion = a.FechaResolucion
                }
            });

        var vm = new AlertasStockViewModel
        {
            Activas = await alertas.Where(a => a.Activa)
                .OrderBy(a => a.Item.FechaAlerta).Select(a => a.Item).ToListAsync(),
            Resueltas = await alertas.Where(a => !a.Activa)
                .OrderByDescending(a => a.Item.FechaResolucion).Select(a => a.Item).Take(10).ToListAsync()
        };
        return View(vm);
    }

    private async Task CargarOpcionesAsync(IngresoFormViewModel model)
    {
        model.Insumos = await _db.Insumos.AsNoTracking()
            .Where(i => i.Activo)
            .OrderBy(i => i.Nombre)
            .Select(i => new InsumoOpcionViewModel
            {
                Id = i.Id,
                Texto = i.Nombre + " (" + i.UnidadMedida.Nombre + ")",
                RequiereVencimiento = i.CategoriaInsumo.RequiereVencimiento
            })
            .ToListAsync();
        model.Categorias = await CategoriasAsync();
        model.Unidades = await UnidadesAsync();
    }

    private async Task CargarOpcionesAsync(InsumoEditarViewModel model)
    {
        model.TieneMovimientos = await _db.MovimientosInventario.AnyAsync(m => m.InsumoId == model.Id);
        model.Categorias = await CategoriasAsync();
        model.Unidades = await UnidadesAsync();
    }

    private Task<List<CategoriaOpcionViewModel>> CategoriasAsync() =>
        _db.CategoriasInsumo.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaOpcionViewModel
            {
                Id = c.Id,
                Texto = c.Nombre,
                RequiereVencimiento = c.RequiereVencimiento
            })
            .ToListAsync();

    private Task<List<OpcionViewModel>> UnidadesAsync() =>
        _db.UnidadesMedida.AsNoTracking()
            .Where(u => u.Activo)
            .OrderBy(u => u.Nombre)
            .Select(u => new OpcionViewModel { Id = u.Id, Texto = u.Nombre + " (" + u.Abreviatura + ")" })
            .ToListAsync();
}
