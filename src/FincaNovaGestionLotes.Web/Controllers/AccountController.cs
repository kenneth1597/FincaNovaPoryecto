using BCrypt.Net;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using FincaNovaGestionLotes.Web.Models;
using FincaNovaGestionLotes.Web.Services; // <-- Asegúrate de tener la referencia a tus servicios
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());

        if (usuario == null)
        {
            // Por seguridad o retroalimentación en pruebas
            ModelState.AddModelError("Email", "No existe una cuenta asociada a este correo.");
            return View(model);
        }

        // 1. Generar token único de seguridad
        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        // 2. Guardar el token y su vencimiento en el usuario (30 min)
        usuario.ResetPasswordToken = token;
        usuario.ResetPasswordTokenExpiration = DateTime.Now.AddMinutes(30);

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        // 3. Crear el enlace que irá dentro del correo
        var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = usuario.Email }, Request.Scheme);

        // 4. Enviar el correo electrónico
        await emailService.SendPasswordResetEmailAsync(usuario.Email, resetLink!);

        // 5. Redirigir AL LOGIN (no a ResetPassword) con mensaje de éxito
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

        // Buscar al usuario por correo y validar que el token coincida y no haya expirado
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Email.ToLower() == model.Email.Trim().ToLower() &&
                u.ResetPasswordToken == model.Token &&
                u.ResetPasswordTokenExpiration > DateTime.Now);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "El enlace es inválido o ya ha expirado. Por favor solicita uno nuevo.");
            return View(model);
        }

        // 1. Encriptar la NUEVA contraseña con BCrypt
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        // 2. Anular el token para que no se pueda reutilizar
        usuario.ResetPasswordToken = null;
        usuario.ResetPasswordTokenExpiration = null;

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
        // Verificar que quien hace el cambio sea Administrador o Dueño
        var rolActual = HttpContext.Session.GetString("UsuarioRol");
        if (rolActual != "Administrador" && rolActual != "Dueño")
        {
            return Forbid(); // Acceso denegado si es un Trabajador
        }

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario != null)
        {
            usuario.Rol = nuevoRol; // "Administrador", "Dueño" o "Trabajador"
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Usuarios");
    }
}