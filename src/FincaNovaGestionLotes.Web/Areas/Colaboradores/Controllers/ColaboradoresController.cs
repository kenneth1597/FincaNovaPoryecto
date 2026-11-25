using FincaNovaGestionLotes.Web.Areas.Colaboradores.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.Services.Labores;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaNovaGestionLotes.Web.Areas.Colaboradores.Controllers;

[Area("Colaboradores")]
[RequiereRol("Administrador", "Dueño")]
public class ColaboradoresController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ParametrosJornada _jornada;

    public ColaboradoresController(AppDbContext db, IAuditService audit, IOptions<ParametrosJornada> jornada)
    {
        _db = db;
        _audit = audit;
        _jornada = jornada.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ColaboradorFiltroViewModel filtro)
    {
        var query = _db.Colaboradores.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var t = filtro.Q.Trim();
            query = query.Where(c =>
                c.Nombre.Contains(t) ||
                c.Identificacion.Contains(t) ||
                c.Cargo.Contains(t));
        }

        if (filtro.Estado == "Activos")
            query = query.Where(c => c.Activo);
        else if (filtro.Estado == "Inactivos")
            query = query.Where(c => !c.Activo);

        var proyeccion = query.Select(c => new ColaboradorListItemViewModel
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Identificacion = c.Identificacion,
            Telefono = c.Telefono,
            Cargo = c.Cargo,
            TarifaJornada = c.TarifaJornada,
            Activo = c.Activo,
            Labores = c.Labores.Count(),
            Horas = _db.LaboresAvances.Where(a => a.ColaboradorId == c.Id).Sum(a => (decimal?)a.Horas) ?? 0,
            UltimaLabor = c.Labores.Max(lc => (DateTime?)lc.Labor.Fecha),
            UsuarioEmail = c.Usuario != null ? c.Usuario.Email : null
        });

        proyeccion = filtro.Orden switch
        {
            "nombre_desc" => proyeccion.OrderByDescending(c => c.Nombre),
            "cargo" => proyeccion.OrderBy(c => c.Cargo).ThenBy(c => c.Nombre),
            "labores" => proyeccion.OrderByDescending(c => c.Labores).ThenBy(c => c.Nombre),
            "horas" => proyeccion.OrderByDescending(c => c.Horas).ThenBy(c => c.Nombre),
            "reciente" => proyeccion.OrderByDescending(c => c.UltimaLabor).ThenBy(c => c.Nombre),
            _ => proyeccion.OrderBy(c => c.Nombre)
        };

        var (resultados, paginacion) = await proyeccion.PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;

        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Historial(int id, int? terrenoId, DateTime? desde, DateTime? hasta, int pagina = 1)
    {
        var vm = await _db.Colaboradores.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ColaboradorHistorialViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Identificacion = c.Identificacion,
                Telefono = c.Telefono,
                Cargo = c.Cargo,
                TarifaJornada = c.TarifaJornada,
                Activo = c.Activo,
                UsuarioEmail = c.Usuario != null ? c.Usuario.Email : null
            })
            .FirstOrDefaultAsync();

        if (vm is null) return NotFound();

        vm.TerrenoId = terrenoId;
        vm.Desde = desde;
        vm.Hasta = hasta;
        vm.Pagina = pagina;
        vm.HorasPorJornada = _jornada.HorasPorJornada;

        var participaciones = _db.LaboresColaboradores.AsNoTracking()
            .Where(lc => lc.ColaboradorId == id);

        if (terrenoId is { } idTerreno)
            participaciones = participaciones.Where(lc => lc.Labor.TerrenoId == idTerreno);
        if (desde is { } d)
            participaciones = participaciones.Where(lc => lc.Labor.Fecha >= d.Date);
        if (hasta is { } h)
        {
            var limite = h.Date.AddDays(1);
            participaciones = participaciones.Where(lc => lc.Labor.Fecha < limite);
        }

        var avances = _db.LaboresAvances.AsNoTracking()
            .Where(a => a.ColaboradorId == id && participaciones.Any(lc => lc.LaborId == a.LaborId));

        vm.TotalLabores = await participaciones.CountAsync();
        vm.TotalDias = await avances.Select(a => a.Fecha).Distinct().CountAsync();
        vm.TotalHoras = await avances.SumAsync(a => (decimal?)a.Horas) ?? 0;
        vm.TotalJornadas = await avances.SumAsync(a => (decimal?)(a.HorasPorJornada > 0 ? a.Horas / a.HorasPorJornada : 0)) ?? 0;
        vm.TotalMonto = await avances.SumAsync(a => (decimal?)a.Costo) ?? 0;

        var (historial, paginacion) = await participaciones
            .OrderByDescending(lc => lc.Labor.Fecha)
            .ThenByDescending(lc => lc.LaborId)
            .Select(lc => new HistorialLaborViewModel
            {
                LaborId = lc.LaborId,
                Fecha = lc.Labor.Fecha,
                TerrenoCodigo = lc.Labor.Terreno.Codigo,
                TerrenoNombre = lc.Labor.Terreno.Nombre,
                Tipo = lc.Labor.Tipo,
                TipoOtro = lc.Labor.TipoOtro,
                Estado = lc.Labor.Estado,
                Dias = lc.Labor.Avances.Where(a => a.ColaboradorId == id).Select(a => a.Fecha).Distinct().Count(),
                Horas = lc.Labor.Avances.Where(a => a.ColaboradorId == id).Sum(a => (decimal?)a.Horas) ?? 0,
                Jornadas = lc.Labor.Avances.Where(a => a.ColaboradorId == id)
                    .Sum(a => (decimal?)(a.HorasPorJornada > 0 ? a.Horas / a.HorasPorJornada : 0)) ?? 0,
                Monto = lc.Labor.Avances.Where(a => a.ColaboradorId == id).Sum(a => (decimal?)a.Costo) ?? 0
            })
            .PaginarAsync(pagina);

        vm.Historial = historial;
        vm.Paginacion = paginacion;

        vm.Terrenos = (await _db.LaboresColaboradores.AsNoTracking()
                .Where(lc => lc.ColaboradorId == id)
                .Select(lc => new { lc.Labor.TerrenoId, lc.Labor.Terreno.Codigo, lc.Labor.Terreno.Nombre })
                .Distinct()
                .OrderBy(t => t.Codigo)
                .ToListAsync())
            .Select(t => (t.TerrenoId, $"{t.Codigo} — {t.Nombre}"))
            .ToList();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new ColaboradorFormViewModel { TarifaJornada = _jornada.TarifaPorDefecto };
        await CargarUsuariosAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ColaboradorFormViewModel model)
    {
        await ValidarIdentificacionAsync(model);
        await ValidarUsuarioAsync(model);
        await CargarUsuariosAsync(model);

        if (!ModelState.IsValid)
            return View(model);

        var colaborador = new Colaborador
        {
            Nombre = model.Nombre.Trim(),
            Identificacion = model.Identificacion.Trim(),
            Telefono = model.Telefono.Trim(),
            Cargo = model.Cargo.Trim(),
            TarifaJornada = model.TarifaJornada,
            Activo = model.Activo,
            UsuarioId = model.UsuarioId,
            CreadoPor = HttpContext.Session.GetString("UsuarioNombre")
        };

        _db.Colaboradores.Add(colaborador);
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Colaborador), colaborador.Id.ToString(),
            $"Registro del colaborador {colaborador.Nombre} ({colaborador.Identificacion})");

        TempData["Ok"] = $"Colaborador «{colaborador.Nombre}» registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var c = await _db.Colaboradores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (c is null) return NotFound();

        var model = new ColaboradorFormViewModel
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Identificacion = c.Identificacion,
            Telefono = c.Telefono,
            Cargo = c.Cargo,
            TarifaJornada = c.TarifaJornada,
            Activo = c.Activo,
            UsuarioId = c.UsuarioId
        };
        await CargarUsuariosAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(ColaboradorFormViewModel model)
    {
        var colaborador = await _db.Colaboradores.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (colaborador is null) return NotFound();

        await ValidarIdentificacionAsync(model);
        await ValidarUsuarioAsync(model);
        await CargarUsuariosAsync(model);

        if (!ModelState.IsValid)
            return View(model);

        colaborador.Nombre = model.Nombre.Trim();
        colaborador.Identificacion = model.Identificacion.Trim();
        colaborador.Telefono = model.Telefono.Trim();
        colaborador.Cargo = model.Cargo.Trim();
        colaborador.TarifaJornada = model.TarifaJornada;
        colaborador.Activo = model.Activo;
        colaborador.UsuarioId = model.UsuarioId;
        colaborador.ModificadoPor = HttpContext.Session.GetString("UsuarioNombre");

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Colaborador), colaborador.Id.ToString(),
            $"Edición del colaborador {colaborador.Nombre}");

        TempData["Ok"] = $"Colaborador «{colaborador.Nombre}» actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        var colaborador = await _db.Colaboradores.FirstOrDefaultAsync(x => x.Id == id);
        if (colaborador is null) return NotFound();

        colaborador.Activo = !colaborador.Activo;
        colaborador.ModificadoPor = HttpContext.Session.GetString("UsuarioNombre");
        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(
            colaborador.Activo ? AccionAuditoria.Modificar : AccionAuditoria.Inactivar,
            nameof(Colaborador), colaborador.Id.ToString(),
            $"Colaborador {colaborador.Nombre} {(colaborador.Activo ? "activado" : "inactivado")}");

        TempData["Ok"] = $"Colaborador «{colaborador.Nombre}» {(colaborador.Activo ? "activado" : "inactivado")}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarIdentificacionAsync(ColaboradorFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Identificacion))
            return;

        var identificacion = model.Identificacion.Trim();
        var existe = await _db.Colaboradores.AnyAsync(c => c.Identificacion == identificacion && c.Id != model.Id);
        if (existe)
            ModelState.AddModelError(nameof(model.Identificacion), "Ya existe un colaborador registrado con esa identificación.");
    }

    private async Task ValidarUsuarioAsync(ColaboradorFormViewModel model)
    {
        if (model.UsuarioId is not { } usuarioId)
            return;

        var usuarioValido = await _db.Usuarios.AnyAsync(u => u.Id == usuarioId && u.Activo);
        if (!usuarioValido)
        {
            ModelState.AddModelError(nameof(model.UsuarioId), "El usuario seleccionado no existe o está inactivo.");
            return;
        }

        var yaVinculado = await _db.Colaboradores.AnyAsync(c => c.UsuarioId == usuarioId && c.Id != model.Id);
        if (yaVinculado)
            ModelState.AddModelError(nameof(model.UsuarioId), "Ese usuario ya está vinculado a otro colaborador.");
    }

    private async Task CargarUsuariosAsync(ColaboradorFormViewModel model)
    {
        var vinculados = _db.Colaboradores
            .Where(c => c.UsuarioId != null && c.Id != model.Id)
            .Select(c => c.UsuarioId);

        model.Usuarios = await _db.Usuarios.AsNoTracking()
            .Where(u => (u.Activo && !vinculados.Contains(u.Id)) || u.Id == model.UsuarioId)
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioOpcionViewModel { Id = u.Id, Texto = u.Nombre + " (" + u.Email + ") — " + u.Rol })
            .ToListAsync();
    }
}
