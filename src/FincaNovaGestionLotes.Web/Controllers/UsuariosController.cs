using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using FincaNovaGestionLotes.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Controllers;

public class UsuariosController(AppDbContext context) : Controller
{
    private readonly AppDbContext _context = context;

    // Método auxiliar para proteger que solo Administrador o Dueño entren
    private bool EsAdminODueno()
    {
        var rol = HttpContext.Session.GetString("UsuarioRol");
        return string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(rol, "Dueño", StringComparison.OrdinalIgnoreCase);
    }

    // GET: /Usuarios
    public async Task<IActionResult> Index()
    {
        var rolActual = HttpContext.Session.GetString("UsuarioRol");

        if (rolActual != "Administrador" && rolActual != "Dueño")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var usuarios = await _context.Usuarios.ToListAsync();
        return View(usuarios);
    }

    // GET: /Usuarios/Create
    public IActionResult Create()
    {
        if (!EsAdminODueno()) return Forbid();

        return View(new UsuarioCreateViewModel());
    }

    // POST: /Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        if (!EsAdminODueno()) return Forbid();

        if (!ModelState.IsValid) return View(model);

        var existeEmail = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (existeEmail)
        {
            ModelState.AddModelError("Email", "El correo ya está registrado en el sistema.");
            return View(model);
        }

        var usuarioActual = HttpContext.Session.GetString("UsuarioNombre") ?? "Sistema";

        var nuevoUsuario = new Usuario
        {
            Nombre = model.Nombre,
            Email = model.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Rol = model.Rol,
            Activo = model.Activo,
            FechaCreacion = DateTime.Now,
            CreadoPor = usuarioActual
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Usuario '{nuevoUsuario.Nombre}' creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Usuarios/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (!EsAdminODueno()) return Forbid();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        var model = new UsuarioEditViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Activo = usuario.Activo
        };

        return View(model);
    }

    // POST: /Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditViewModel model)
    {
        if (!EsAdminODueno()) return Forbid();

        if (!ModelState.IsValid) return View(model);

        var usuario = await _context.Usuarios.FindAsync(model.Id);
        if (usuario == null) return NotFound();

        // Corrección de la Línea 116: Traducción compatible con EF Core a SQL LOWER()
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower() && u.Id != model.Id);

        if (emailExiste)
        {
            ModelState.AddModelError("Email", "Este correo ya pertenece a otro usuario.");
            return View(model);
        }

        var usuarioActual = HttpContext.Session.GetString("UsuarioNombre") ?? "Sistema";

        // Actualizar datos
        usuario.Nombre = model.Nombre;
        usuario.Email = model.Email.ToLower();
        usuario.Rol = model.Rol;
        usuario.Activo = model.Activo;

        usuario.FechaModificacion = DateTime.Now;
        usuario.ModificadoPor = usuarioActual;

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        }

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Usuario '{usuario.Nombre}' actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        var rolActual = HttpContext.Session.GetString("UsuarioRol");
        if (rolActual != "Administrador" && rolActual != "Dueño")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        var usuarioActualId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuario.Id == usuarioActualId)
        {
            TempData["ErrorMessage"] = "No puedes cambiar el estado de tu propia cuenta.";
            return RedirectToAction(nameof(Index));
        }

        usuario.Activo = !usuario.Activo;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        string mensajeEstado = usuario.Activo ? "reactivado" : "desactivado";
        TempData["SuccessMessage"] = $"El usuario {usuario.Nombre} ha sido {mensajeEstado}.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rolActual = HttpContext.Session.GetString("UsuarioRol");
        if (rolActual != "Administrador" && rolActual != "Dueño")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        var usuarioActualId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuario.Id == usuarioActualId)
        {
            TempData["ErrorMessage"] = "No puedes borrar tu propia cuenta de la base de datos.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"El usuario {usuario.Nombre} ha sido eliminado permanentemente de la base de datos.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "No se puede eliminar el usuario de la base de datos porque tiene registros asociados. Te recomendamos desactivarlo en su lugar.";
        }

        return RedirectToAction(nameof(Index));
    }
}