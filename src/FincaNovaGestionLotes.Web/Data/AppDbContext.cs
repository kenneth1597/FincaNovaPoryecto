using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Auditoria;
using FincaNovaGestionLotes.Web.Domain.Terrenos;
using FincaNovaGestionLotes.Web.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Domain.Labores;
using FincaNovaGestionLotes.Web.Domain.Inventario;
using FincaNovaGestionLotes.Web.Domain.Finanzas;
using FincaNovaGestionLotes.Web.Domain.Pagos;

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

    public DbSet<LaborInsumo> LaboresInsumos => Set<LaborInsumo>();

    public DbSet<PeriodoTerreno> PeriodosTerrenos => Set<PeriodoTerreno>();

    public DbSet<ParametrosFinca> ParametrosFinca => Set<ParametrosFinca>();

    // --- Inventario ---
    public DbSet<CategoriaInsumo> CategoriasInsumo => Set<CategoriaInsumo>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<AlertaStock> AlertasStock => Set<AlertaStock>();

    // --- Recolección y producción ---
    public DbSet<Recoleccion> Recolecciones => Set<Recoleccion>();
    public DbSet<PesoSecado> PesosSecado => Set<PesoSecado>();
    public DbSet<PesoProcesado> PesosProcesado => Set<PesoProcesado>();

    // --- Finanzas ---
    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Venta> Ventas => Set<Venta>();

    // --- Pagos ---
    public DbSet<PagoColaborador> PagosColaborador => Set<PagoColaborador>();
    public DbSet<PagoDetalle> PagoDetalles => Set<PagoDetalle>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Mapeo de Usuarios ---
        builder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios", t => t.HasCheckConstraint("CK_Usuarios_Rol",
                "[Rol] IN (N'Administrador', N'Dueño', N'Capataz', N'Trabajador')"));
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Nombre).HasMaxLength(150);
            e.Property(u => u.Rol).HasMaxLength(30);
            e.Property(u => u.PasswordHash).HasMaxLength(100);
            e.Property(u => u.PasswordResetToken).HasMaxLength(100);
        });

        // Quién creó o modificó: nombres de usuario, no texto libre.
        foreach (var property in builder.Model.GetEntityTypes()
                     .Where(t => typeof(Domain.Common.AuditableEntity).IsAssignableFrom(t.ClrType))
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.Name is nameof(Domain.Common.AuditableEntity.CreadoPor)
                                         or nameof(Domain.Common.AuditableEntity.ModificadoPor)))
        {
            property.SetMaxLength(150);
        }

        builder.Entity<Terreno>(e =>
        {
            // Filtrado por Eliminado = 0: un codigo de un lote eliminado queda libre para reutilizarse.
            e.HasIndex(l => new { l.EmpresaId, l.Codigo }).IsUnique().HasFilter("[Eliminado] = 0");
            e.HasOne(l => l.Empresa).WithMany(f => f.Terrenos)
                .HasForeignKey(l => l.EmpresaId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Terrenos_AreaHectareas", "[AreaHectareas] >= 0");
                // Una finca no tiene padre; un lote siempre pertenece a una finca.
                t.HasCheckConstraint("CK_Terrenos_TipoPadre",
                    "([Tipo] = 1 AND [FincaPadreId] IS NULL) OR ([Tipo] = 2 AND [FincaPadreId] IS NOT NULL)");
            });
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

            e.Property(x => x.AtendidaPor).HasMaxLength(150);

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

            e.HasOne(x => x.Insumo)
                .WithMany()
                .HasForeignKey(x => x.InsumoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PeriodoProductivo>(e =>
        {
            e.HasIndex(p => p.FechaInicio);
            e.ToTable(t => t.HasCheckConstraint("CK_PeriodosProductivos_Fechas",
                "[FechaFin] IS NULL OR [FechaFin] >= [FechaInicio]"));
        });

        builder.Entity<PeriodoTerreno>(e =>
        {
            e.ToTable("PeriodosTerrenos");
            e.HasKey(pt => new { pt.PeriodoProductivoId, pt.TerrenoId });

            e.HasOne(pt => pt.PeriodoProductivo)
                .WithMany(p => p.Terrenos)
                .HasForeignKey(pt => pt.PeriodoProductivoId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pt => pt.Terreno)
                .WithMany()
                .HasForeignKey(pt => pt.TerrenoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AuditLog>(e =>
        {
            e.Property(a => a.Usuario).HasMaxLength(150);
            e.Property(a => a.DireccionIp).HasMaxLength(45);
            e.Property(a => a.Modulo).HasMaxLength(100);
            e.Property(a => a.Entidad).HasMaxLength(100);
            e.Property(a => a.EntidadId).HasMaxLength(50);
            e.HasIndex(a => new { a.Entidad, a.EntidadId });
            e.HasIndex(a => a.FechaHora);
            // El trigger bloquea UPDATE y DELETE (HU-18); EF debe saberlo para generar los INSERT.
            e.ToTable(t => t.HasTrigger("TR_AuditLogs_SoloLectura"));
        });

        builder.Entity<ParametrosFinca>(e =>
        {
            e.HasIndex(p => p.EmpresaId).IsUnique();
            e.HasOne(p => p.Empresa)
                .WithMany()
                .HasForeignKey(p => p.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(p => p.TamanoMaximoAdjuntoMB).HasDefaultValue(5);
            e.Property(p => p.ExtensionesAdjuntoPermitidas).HasDefaultValue("pdf,jpg,jpeg,png");
        });

        ConfigurarInventario(builder);
        ConfigurarProduccion(builder);
        ConfigurarFinanzas(builder);
        ConfigurarPagos(builder);

        builder.Entity<Colaborador>(e =>
        {
            e.HasIndex(c => c.Identificacion).IsUnique();

            e.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(c => c.UsuarioId).IsUnique().HasFilter("[UsuarioId] IS NOT NULL");

            e.Property(c => c.TarifaCajuela).HasDefaultValue(0m);
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

    // Fecha fija para los datos semilla: HasData no admite valores que cambien entre migraciones.
    private static readonly DateTime FechaSemilla = new(2026, 11, 4, 0, 0, 0, DateTimeKind.Utc);

    private static void ConfigurarInventario(ModelBuilder builder)
    {
        builder.Entity<CategoriaInsumo>(e =>
        {
            e.ToTable("CategoriasInsumo");
            e.HasIndex(c => c.Nombre).IsUnique();
            e.HasData(
                new CategoriaInsumo { Id = 1, Nombre = "Fertilizante", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaInsumo { Id = 2, Nombre = "Insumo Fitosanitario / Químico", RequiereVencimiento = true, FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaInsumo { Id = 3, Nombre = "Herramienta", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaInsumo { Id = 4, Nombre = "Otro", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" });
        });

        builder.Entity<UnidadMedida>(e =>
        {
            e.ToTable("UnidadesMedida");
            e.HasIndex(u => u.Nombre).IsUnique();
            e.HasIndex(u => u.Abreviatura).IsUnique();
            e.HasData(
                new UnidadMedida { Id = 1, Nombre = "Litro", Abreviatura = "L", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 2, Nombre = "Mililitro", Abreviatura = "mL", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 3, Nombre = "Galón", Abreviatura = "gal", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 4, Nombre = "Kilogramo", Abreviatura = "kg", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 5, Nombre = "Gramo", Abreviatura = "g", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 6, Nombre = "Saco", Abreviatura = "saco", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new UnidadMedida { Id = 7, Nombre = "Unidad", Abreviatura = "und", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" });
        });

        builder.Entity<Insumo>(e =>
        {
            e.HasIndex(i => i.Nombre).IsUnique();
            // Si dos personas mueven el mismo insumo a la vez, el segundo UPDATE no
            // encuentra el stock que leyó y falla, en vez de sobrescribir el rebajo del primero.
            e.Property(i => i.StockActual).IsConcurrencyToken();
            e.HasOne(i => i.CategoriaInsumo).WithMany(c => c.Insumos)
                .HasForeignKey(i => i.CategoriaInsumoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.UnidadMedida).WithMany()
                .HasForeignKey(i => i.UnidadMedidaId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Insumos_StockActual", "[StockActual] >= 0");
                t.HasCheckConstraint("CK_Insumos_StockMinimo", "[StockMinimo] >= 0");
            });
        });

        builder.Entity<MovimientoInventario>(e =>
        {
            e.ToTable("MovimientosInventario", t =>
                t.HasCheckConstraint("CK_MovimientosInventario_Cantidad", "[Cantidad] > 0"));
            e.HasIndex(m => new { m.InsumoId, m.Fecha });
            e.HasOne(m => m.Insumo).WithMany(i => i.Movimientos)
                .HasForeignKey(m => m.InsumoId).OnDelete(DeleteBehavior.Restrict);
            // Si se borra el origen, el movimiento se conserva para no descuadrar el inventario.
            e.HasOne(m => m.ProductoTratamiento).WithMany()
                .HasForeignKey(m => m.ProductoTratamientoId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.LaborInsumo).WithMany()
                .HasForeignKey(m => m.LaborInsumoId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(m => m.Gasto).WithMany()
                .HasForeignKey(m => m.GastoId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<AlertaStock>(e =>
        {
            e.ToTable("AlertasStock");
            // Una sola alerta activa por insumo (HU-25: se mantiene hasta reabastecer).
            e.HasIndex(a => a.InsumoId).IsUnique().HasFilter("[Activa] = 1");
            e.HasOne(a => a.Insumo).WithMany(i => i.Alertas)
                .HasForeignKey(a => a.InsumoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<LaborInsumo>(e =>
        {
            e.ToTable("LaboresInsumos", t =>
                t.HasCheckConstraint("CK_LaboresInsumos_Cantidad", "[Cantidad] > 0"));
            e.HasIndex(li => new { li.LaborId, li.InsumoId }).IsUnique();
            e.HasOne(li => li.Labor).WithMany(l => l.Insumos)
                .HasForeignKey(li => li.LaborId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(li => li.Insumo).WithMany()
                .HasForeignKey(li => li.InsumoId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarProduccion(ModelBuilder builder)
    {
        builder.Entity<Recoleccion>(e =>
        {
            e.ToTable("Recolecciones", t =>
                t.HasCheckConstraint("CK_Recolecciones_Cajuelas", "[Cajuelas] > 0"));
            e.HasIndex(r => new { r.PeriodoProductivoId, r.Fecha });
            e.HasIndex(r => new { r.ColaboradorId, r.Fecha });
            e.HasIndex(r => new { r.TerrenoId, r.Fecha });
            e.Property(r => r.PesoEstimadoKg)
                .HasComputedColumnSql("[Cajuelas] * [KgPorCajuela]", stored: true);
            e.HasOne(r => r.Colaborador).WithMany()
                .HasForeignKey(r => r.ColaboradorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Terreno).WithMany()
                .HasForeignKey(r => r.TerrenoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.PeriodoProductivo).WithMany()
                .HasForeignKey(r => r.PeriodoProductivoId).OnDelete(DeleteBehavior.Restrict);
            // Solo se recolecta en lotes asignados al período.
            e.HasOne(r => r.PeriodoTerreno).WithMany()
                .HasForeignKey(r => new { r.PeriodoProductivoId, r.TerrenoId })
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PesoSecado>(e =>
        {
            e.ToTable("PesosSecado", t =>
                t.HasCheckConstraint("CK_PesosSecado_PesoKg", "[PesoKg] > 0"));
            e.HasIndex(p => new { p.TerrenoId, p.PeriodoProductivoId });
            e.HasOne(p => p.Terreno).WithMany()
                .HasForeignKey(p => p.TerrenoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.PeriodoProductivo).WithMany()
                .HasForeignKey(p => p.PeriodoProductivoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.PeriodoTerreno).WithMany()
                .HasForeignKey(p => new { p.PeriodoProductivoId, p.TerrenoId })
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PesoProcesado>(e =>
        {
            e.ToTable("PesosProcesado", t =>
                t.HasCheckConstraint("CK_PesosProcesado_PesoKg", "[PesoKg] > 0"));
            e.HasIndex(p => new { p.TerrenoId, p.PeriodoProductivoId });
            e.HasOne(p => p.Terreno).WithMany()
                .HasForeignKey(p => p.TerrenoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.PeriodoProductivo).WithMany()
                .HasForeignKey(p => p.PeriodoProductivoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.PeriodoTerreno).WithMany()
                .HasForeignKey(p => new { p.PeriodoProductivoId, p.TerrenoId })
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarFinanzas(ModelBuilder builder)
    {
        builder.Entity<CategoriaGasto>(e =>
        {
            e.ToTable("CategoriasGasto");
            e.HasIndex(c => c.Nombre).IsUnique();
            e.HasData(
                new CategoriaGasto { Id = 1, Nombre = "Insumos", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 2, Nombre = "Mano de obra", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 3, Nombre = "Planilla", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 4, Nombre = "Mantenimiento", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 5, Nombre = "Transporte", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 6, Nombre = "Servicios", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" },
                new CategoriaGasto { Id = 7, Nombre = "Otros", FechaCreacion = FechaSemilla, CreadoPor = "Sistema" });
        });

        builder.Entity<Gasto>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_Gastos_Monto", "[Monto] > 0"));
            e.HasIndex(g => g.Fecha);
            e.HasIndex(g => new { g.TerrenoId, g.Fecha });
            e.HasOne(g => g.CategoriaGasto).WithMany(c => c.Gastos)
                .HasForeignKey(g => g.CategoriaGastoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(g => g.Terreno).WithMany()
                .HasForeignKey(g => g.TerrenoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(g => g.PeriodoProductivo).WithMany()
                .HasForeignKey(g => g.PeriodoProductivoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Venta>(e =>
        {
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Ventas_Cantidad", "[Cantidad] > 0");
                t.HasCheckConstraint("CK_Ventas_PrecioUnitario", "[PrecioUnitario] > 0");
            });
            e.HasIndex(v => v.Fecha);
            e.Property(v => v.Total).HasComputedColumnSql("[Cantidad] * [PrecioUnitario]", stored: true);
            e.HasOne(v => v.PeriodoProductivo).WithMany()
                .HasForeignKey(v => v.PeriodoProductivoId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarPagos(ModelBuilder builder)
    {
        builder.Entity<PagoColaborador>(e =>
        {
            e.ToTable("PagosColaborador", t =>
                t.HasCheckConstraint("CK_PagosColaborador_Fechas", "[FechaFin] >= [FechaInicio]"));
            e.HasIndex(p => new { p.ColaboradorId, p.FechaInicio });
            e.HasOne(p => p.Colaborador).WithMany()
                .HasForeignKey(p => p.ColaboradorId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PagoDetalle>(e =>
        {
            // Cada línea viene de un avance de labor (jornada) o de una recolección (cajuela).
            e.ToTable("PagoDetalles", t => t.HasCheckConstraint("CK_PagoDetalles_Origen",
                "([Tipo] = 1 AND [LaborAvanceId] IS NOT NULL AND [RecoleccionId] IS NULL) OR " +
                "([Tipo] = 2 AND [RecoleccionId] IS NOT NULL AND [LaborAvanceId] IS NULL)"));
            e.Property(d => d.Subtotal).HasComputedColumnSql("[Cantidad] * [Tarifa]", stored: true);
            e.HasOne(d => d.PagoColaborador).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.PagoColaboradorId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.LaborAvance).WithMany()
                .HasForeignKey(d => d.LaborAvanceId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Recoleccion).WithMany()
                .HasForeignKey(d => d.RecoleccionId).OnDelete(DeleteBehavior.Restrict);
            // Evita pagar dos veces el mismo trabajo.
            e.HasIndex(d => d.LaborAvanceId).IsUnique().HasFilter("[LaborAvanceId] IS NOT NULL");
            e.HasIndex(d => d.RecoleccionId).IsUnique().HasFilter("[RecoleccionId] IS NOT NULL");
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