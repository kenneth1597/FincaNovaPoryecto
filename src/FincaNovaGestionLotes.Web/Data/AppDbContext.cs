using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Auditoria;
using FincaNovaGestionLotes.Web.Domain.Lotes;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Finca> Fincas => Set<Finca>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Usuario>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<Lote>(e =>
        {
            // Filtrado por Eliminado = 0: un codigo de un lote eliminado queda libre para reutilizarse.
            e.HasIndex(l => new { l.FincaId, l.Codigo }).IsUnique().HasFilter("[Eliminado] = 0");
            e.HasOne(l => l.Finca).WithMany(f => f.Lotes)
                .HasForeignKey(l => l.FincaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.LotePadre).WithMany(l => l.MicroLotes)
                .HasForeignKey(l => l.LotePadreId).OnDelete(DeleteBehavior.Restrict);
        });

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }
    }

    public override int SaveChanges()
    {
        StampAuditableEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void StampAuditableEntities()
    {
        var ahora = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Domain.Common.AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.FechaCreacion = ahora;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.FechaModificacion = ahora;
            }
        }
    }
}
