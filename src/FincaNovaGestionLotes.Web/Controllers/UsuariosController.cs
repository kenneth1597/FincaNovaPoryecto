using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using FincaNovaGestionLotes.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Controllers;

public class UsuariosController : Controller
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // Método auxiliar para proteger que solo Administrador o Dueño entren
    private bool EsAdminODueno()
    {
        var rol = HttpContext.Session.GetString("UsuarioRol");
        return rol == "Administrador" || rol == "Dueño";
    }

    // GET: /Usuarios
    public async Task<IActionResult> Index()
    {
        if (!EsAdminODueno()) return Forbid();

        var usuarios = await _context.Usuarios
            .OrderByDescending(u => u.FechaCreacion)
            .ToListAsync();

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

        var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());
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
            CreadoPor = usuarioActual // Guarda quién creó el usuario
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

        // Validar correo duplicado si se cambió el email
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

        // Registrar auditoría de modificación
        usuario.FechaModificacion = DateTime.Now;
        usuario.ModificadoPor = usuarioActual;

        // Si digitó una contraseña nueva, actualizarla
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        }

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Usuario '{usuario.Nombre}' actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}