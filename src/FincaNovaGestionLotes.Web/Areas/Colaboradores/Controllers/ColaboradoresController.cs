using FincaNovaGestionLotes.Web.Areas.Colaboradores.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using Microsoft.AspNetCore.Mvc;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Areas.Colaboradores.Controllers;

[Area("Colaboradores")]
[RequiereRol("Administrador", "Dueño")]
public class ColaboradoresController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public ColaboradoresController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
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

        if (filtro.Activo is { } activo)
            query = query.Where(c => c.Activo == activo);

        var (resultados, paginacion) = await query
            .OrderBy(c => c.Nombre)
            .Select(c => new ColaboradorListItemViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Identificacion = c.Identificacion,
                Telefono = c.Telefono,
                Cargo = c.Cargo,
                TarifaJornada = c.TarifaJornada,
                Activo = c.Activo,
                Labores = c.Labores.Count(),
                UsuarioEmail = c.Usuario != null ? c.Usuario.Email : null
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;

        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new ColaboradorFormViewModel();
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
