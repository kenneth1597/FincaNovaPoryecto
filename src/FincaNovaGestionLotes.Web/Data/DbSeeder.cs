using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Data;


public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Empresas.AnyAsync())
        {
            db.Empresas.Add(new Empresa
            {
                Nombre = "Café Chaperno",
                Ubicacion = "Poás, Alajuela, Costa Rica",
                Propietario = "Edy Alberto Salazar Quesada",
                Telefono = "83222672"
            });
            await db.SaveChangesAsync();
        }

        await CrearAdministradorInicialAsync(db, services);
    }

    private static async Task CrearAdministradorInicialAsync(AppDbContext db, IServiceProvider services)
    {
        if (await db.Usuarios.AnyAsync())
            return;

        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DbSeeder));

        var email = config["AdminInicial:Email"];
        var nombre = config["AdminInicial:Nombre"];
        var password = config["AdminInicial:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No hay usuarios y no se configuró AdminInicial (Email y Password). " +
                "Nadie podrá iniciar sesión hasta que se configure.");
            return;
        }

        db.Usuarios.Add(new Usuario
        {
            Nombre = string.IsNullOrWhiteSpace(nombre) ? "Administrador" : nombre.Trim(),
            Email = email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Rol = Roles.Administrador,
            Activo = true,
            CreadoPor = "Sistema"
        });
        await db.SaveChangesAsync();

        logger.LogInformation("Administrador inicial creado: {Email}", email.Trim().ToLower());
    }
}
