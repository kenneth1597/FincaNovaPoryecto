using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Auditoria;
using FincaNovaGestionLotes.Web.Domain.Lotes;
using Microsoft.EntityFrameworkCore;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Produccion;

namespace FincaNovaGestionLotes.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Finca> Fincas => Set<Finca>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<PeriodoProductivo> PeriodosProductivos
    => Set<PeriodoProductivo>();

    public DbSet<Enfermedad> Enfermedades => Set<Enfermedad>();

    public DbSet<TratamientoEnfermedad> TratamientosEnfermedad
        => Set<TratamientoEnfermedad>();

    public DbSet<ProductoTratamiento> ProductosTratamiento
        => Set<ProductoTratamiento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        builder.Entity<Enfermedad>(e =>
        {
            e.HasOne(x => x.Lote)
                .WithMany()
                .HasForeignKey(x => x.LoteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.LoteId, x.Nombre, x.FechaDeteccion });
        });

        builder.Entity<TratamientoEnfermedad>(e =>
        {
            e.HasOne(x => x.Enfermedad)
                .WithMany(x => x.Tratamientos)
                .HasForeignKey(x => x.EnfermedadId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductoTratamiento>(e =>
        {
            e.HasOne(x => x.TratamientoEnfermedad)
                .WithMany(x => x.Productos)
                .HasForeignKey(x => x.TratamientoEnfermedadId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PeriodoProductivo>(e =>
        {
            e.HasOne(p => p.Lote)
                .WithMany()
                .HasForeignKey(p => p.LoteId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => new
            {
                p.LoteId,
                p.FechaInicio
            });
        });

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
