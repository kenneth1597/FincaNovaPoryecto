using BCrypt.Net;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using FincaNovaGestionLotes.Web.Models;
using FincaNovaGestionLotes.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FincaNovaGestionLotes.Web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    #region Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower() && u.Activo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(model);
        }

        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
        HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
        HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

        return RedirectToAction("Index", "Home");
    }
    #endregion

    #region Registro
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existeEmail = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (existeEmail)
        {
            ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
            return View(model);
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = model.Nombre,
            Email = model.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Rol = "Trabajador", // Rol por defecto al registrarse
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cuenta creada exitosamente. Ya puedes iniciar sesión.";
        return RedirectToAction("Login");
    }
    #endregion

    #region Olvidé mi Contraseña
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, [FromServices] IEmailService emailService)
    {
        if (!ModelState.IsValid)
            return View(model);

        var emailLimpio = model.Email.Trim().ToLower();
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailLimpio);

        if (usuario == null)
        {
            ModelState.AddModelError("Email", "No existe una cuenta asociada a este correo.");
            return View(model);
        }

        // 1. Generar token limpio seguro
        string token = Guid.NewGuid().ToString("N");

        // 2. Guardar expiración en UTC
        usuario.ResetPasswordToken = token;
        usuario.ResetPasswordTokenExpiration = DateTime.UtcNow.AddMinutes(30);

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        // 3. Crear enlace de restablecimiento
        var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = usuario.Email }, Request.Scheme);

        // 4. Enviar correo
        await emailService.SendPasswordResetEmailAsync(usuario.Email, resetLink!);

        TempData["SuccessMessage"] = "Te hemos enviado un correo electrónico con el enlace para restablecer tu contraseña.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "El enlace de recuperación es inválido.";
            return RedirectToAction("ForgotPassword");
        }

        var model = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var emailLimpio = model.Email.Trim().ToLower();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == emailLimpio &&
                u.ResetPasswordToken == model.Token &&
                u.ResetPasswordTokenExpiration > DateTime.UtcNow);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "El enlace es inválido o ya ha expirado. Por favor solicita uno nuevo.");
            return View(model);
        }

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        usuario.ResetPasswordToken = null;
        usuario.ResetPasswordTokenExpiration = null;
        usuario.FechaModificacion = DateTime.UtcNow;
        usuario.ModificadoPor = "Restablecimiento de Contraseña";

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Tu contraseña ha sido restablecida con éxito. Ya puedes iniciar sesión.";
        return RedirectToAction("Login");
    }
    #endregion

    #region Logout
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
    #endregion

    [HttpPost]
    public async Task<IActionResult> CambiarRol(int usuarioId, string nuevoRol)
    {
        var rolActual = HttpContext.Session.GetString("UsuarioRol");
        if (rolActual != "Administrador" && rolActual != "Dueño")
        {
            return Forbid();
        }

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario != null)
        {
            usuario.Rol = nuevoRol;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Usuarios");
    }

    #region Mi Perfil

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        // Obtener el ID del usuario desde la Sesión
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null)
        {
            return RedirectToAction("Login");
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == usuarioId.Value);

        if (usuario == null)
        {
            return NotFound();
        }

        var model = new PerfilViewModel
        {
            Id = usuario.Id,
            NombreCompleto = usuario.Nombre, // Se asigna la propiedad Nombre
            Email = usuario.Email,
            RolNombre = string.IsNullOrEmpty(usuario.Rol) ? "Sin Rol" : usuario.Rol
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel model)
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null || usuarioId.Value != model.Id)
        {
            return RedirectToAction("Login");
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == usuarioId.Value);

        if (usuario == null)
        {
            return NotFound();
        }

        model.RolNombre = string.IsNullOrEmpty(usuario.Rol) ? "Sin Rol" : usuario.Rol;

        var emailLimpio = model.Email.Trim().ToLower();
        bool emailExistente = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == emailLimpio && u.Id != usuarioId.Value);

        if (emailExistente)
        {
            ModelState.AddModelError("Email", "El correo ingresado ya pertenece a otra cuenta.");
        }

        bool deseaCambiarPassword = !string.IsNullOrWhiteSpace(model.NewPassword);

        if (deseaCambiarPassword)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Debe ingresar su contraseña actual para autorizar el cambio.");
            }
            else if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, usuario.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Actualizar datos del usuario
        usuario.Nombre = model.NombreCompleto.Trim();
        usuario.Email = emailLimpio;

        if (deseaCambiarPassword && !string.IsNullOrWhiteSpace(model.NewPassword))
        {
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        }

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        // Actualizar el nombre en la sesión
        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);

        TempData["SuccessMessage"] = "Tu perfil ha sido actualizado correctamente.";
        return RedirectToAction("Perfil");
    }

    #endregion
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}