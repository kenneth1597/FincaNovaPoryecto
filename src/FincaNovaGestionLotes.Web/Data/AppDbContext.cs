using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Auditoria;
using FincaNovaGestionLotes.Web.Domain.Terrenos;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Domain.Labores;

namespace FincaNovaGestionLotes.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Terreno> Terrenos => Set<Terreno>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<PeriodoProductivo> PeriodosProductivos
    => Set<PeriodoProductivo>();

    public DbSet<Enfermedad> Enfermedades => Set<Enfermedad>();

    public DbSet<TratamientoEnfermedad> TratamientosEnfermedad
        => Set<TratamientoEnfermedad>();

    public DbSet<ProductoTratamiento> ProductosTratamiento
        => Set<ProductoTratamiento>();

    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();

    public DbSet<Labor> Labores => Set<Labor>();

    public DbSet<LaborColaborador> LaboresColaboradores => Set<LaborColaborador>();

    public DbSet<LaborAvance> LaboresAvances => Set<LaborAvance>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Mapeo de Usuarios ---
        builder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios"); 
            e.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<Terreno>(e =>
        {
            // Filtrado por Eliminado = 0: un codigo de un lote eliminado queda libre para reutilizarse.
            e.HasIndex(l => new { l.EmpresaId, l.Codigo }).IsUnique().HasFilter("[Eliminado] = 0");
            e.HasOne(l => l.Empresa).WithMany(f => f.Terrenos)
                .HasForeignKey(l => l.EmpresaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.FincaPadre).WithMany(l => l.Lotes)
                .HasForeignKey(l => l.FincaPadreId).OnDelete(DeleteBehavior.Restrict);
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
            e.HasOne(x => x.Terreno)
                .WithMany()
                .HasForeignKey(x => x.TerrenoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.TerrenoId, x.Nombre, x.FechaDeteccion });

            e.HasOne(x => x.TratamientoEfectivo)
                .WithMany()
                .HasForeignKey(x => x.TratamientoEfectivoId)
                .OnDelete(DeleteBehavior.NoAction);
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
            e.HasOne(p => p.Terreno)
                .WithMany()
                .HasForeignKey(p => p.TerrenoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => new
            {
                p.TerrenoId,
                p.FechaInicio
            });
        });

        builder.Entity<Colaborador>(e =>
        {
            e.HasIndex(c => c.Identificacion).IsUnique();

            e.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(c => c.UsuarioId).IsUnique().HasFilter("[UsuarioId] IS NOT NULL");
        });

        builder.Entity<LaborAvance>(e =>
        {
            e.HasOne(a => a.Labor)
                .WithMany(l => l.Avances)
                .HasForeignKey(a => a.LaborId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(a => a.Colaborador)
                .WithMany()
                .HasForeignKey(a => a.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(a => new { a.LaborId, a.Fecha });
        });

        builder.Entity<Labor>(e =>
        {
            e.HasOne(l => l.Terreno)
                .WithMany()
                .HasForeignKey(l => l.TerrenoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(l => new { l.TerrenoId, l.Fecha });
        });

        builder.Entity<LaborColaborador>(e =>
        {
            e.HasKey(lc => new { lc.LaborId, lc.ColaboradorId });

            e.HasOne(lc => lc.Labor)
                .WithMany(l => l.Colaboradores)
                .HasForeignKey(lc => lc.LaborId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(lc => lc.Colaborador)
                .WithMany(c => c.Labores)
                .HasForeignKey(lc => lc.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);
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