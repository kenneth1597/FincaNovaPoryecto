using FincaNovaGestionLotes.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Data;


public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (!await db.Fincas.AnyAsync())
        {
            db.Fincas.Add(new Finca
            {
                Nombre = "Café Chaperno",
                Ubicacion = "Poás, Alajuela, Costa Rica",
                Propietario = "Edy Alberto Salazar Quesada",
                Telefono = "83222672"
            });
            await db.SaveChangesAsync();
        }
    }
}
