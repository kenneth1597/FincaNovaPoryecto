/* =====================================================================
   FincaNova · Script completo de la base de datos FincaUSER
   ---------------------------------------------------------------------
   Generado con: dotnet ef migrations script --idempotent
   Incluye las 19 migraciones (hasta LaboresConRevision).

   Es idempotente: se puede ejecutar sobre una base vacía o sobre una
   FincaUSER existente; solo aplica las migraciones que falten.

   Cómo usarlo
   1. Abrir en SQL Server Management Studio y ejecutar (F5).
   2. La aplicación (appsettings.json) apunta a Database=FincaUSER.

   Fuente de verdad: las migraciones EF en
   src/FincaNovaGestionLotes.Web/Data/Migrations. Si se agrega una
   migración, regenerar este archivo con el mismo comando.
   ===================================================================== */

USE master;
GO

IF DB_ID(N'FincaUSER') IS NULL
    CREATE DATABASE FincaUSER;
GO

USE FincaUSER;
GO

-- Requeridos por los índices filtrados y columnas calculadas (sqlcmd los trae apagados).
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [FechaHora] datetime2 NOT NULL,
        [Usuario] nvarchar(max) NULL,
        [Accion] int NOT NULL,
        [Entidad] nvarchar(max) NOT NULL,
        [EntidadId] nvarchar(max) NULL,
        [Descripcion] nvarchar(max) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [Fincas] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [Ubicacion] nvarchar(200) NULL,
        [Propietario] nvarchar(150) NULL,
        [Telefono] nvarchar(30) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Fincas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [Lotes] (
        [Id] int NOT NULL IDENTITY,
        [FincaId] int NOT NULL,
        [Codigo] nvarchar(30) NOT NULL,
        [Nombre] nvarchar(120) NOT NULL,
        [Tipo] int NOT NULL,
        [LotePadreId] int NULL,
        [AreaHectareas] decimal(18,2) NOT NULL,
        [VariedadCafe] nvarchar(100) NULL,
        [AnioSiembra] int NULL,
        [Ubicacion] nvarchar(200) NULL,
        [Estado] int NOT NULL,
        [FechaUltimoCambioEstado] datetime2 NULL,
        [EstadoCambiadoPor] nvarchar(256) NULL,
        [Observaciones] nvarchar(500) NULL,
        [Eliminado] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Lotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Lotes_Fincas_FincaId] FOREIGN KEY ([FincaId]) REFERENCES [Fincas] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Lotes_Lotes_LotePadreId] FOREIGN KEY ([LotePadreId]) REFERENCES [Lotes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [RolClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RolClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RolClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [UsuarioClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UsuarioClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UsuarioClaims_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [UsuarioLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UsuarioLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UsuarioLogins_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [UsuarioRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_UsuarioRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UsuarioRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsuarioRoles_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE TABLE [UsuarioTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UsuarioTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UsuarioTokens_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lotes_FincaId_Codigo] ON [Lotes] ([FincaId], [Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lotes_LotePadreId] ON [Lotes] ([LotePadreId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RolClaims_RoleId] ON [RolClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UsuarioClaims_UserId] ON [UsuarioClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UsuarioLogins_UserId] ON [UsuarioLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UsuarioRoles_RoleId] ON [UsuarioRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Usuarios] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Usuarios] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921013634_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921013634_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921024457_FiltrarIndiceCodigoPorEliminado'
)
BEGIN
    DROP INDEX [IX_Lotes_FincaId_Codigo] ON [Lotes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921024457_FiltrarIndiceCodigoPorEliminado'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Lotes_FincaId_Codigo] ON [Lotes] ([FincaId], [Codigo]) WHERE [Eliminado] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921024457_FiltrarIndiceCodigoPorEliminado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921024457_FiltrarIndiceCodigoPorEliminado', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [RolClaims];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [UsuarioClaims];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [UsuarioLogins];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [UsuarioRoles];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [UsuarioTokens];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [Roles];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    DROP TABLE [Usuarios];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921033230_QuitarIdentityYRoles'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921033230_QuitarIdentityYRoles', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924063228_AgregarTablaUsuario'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Rol] nvarchar(max) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924063228_AgregarTablaUsuario'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [Usuarios] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924063228_AgregarTablaUsuario'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924063228_AgregarTablaUsuario', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE TABLE [Enfermedades] (
        [Id] int NOT NULL IDENTITY,
        [LoteId] int NOT NULL,
        [Nombre] nvarchar(150) NOT NULL,
        [FechaDeteccion] datetime2 NOT NULL,
        [Severidad] nvarchar(30) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        [Atendida] bit NOT NULL,
        [FechaAtendida] datetime2 NULL,
        [AtendidaPor] nvarchar(max) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Enfermedades] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Enfermedades_Lotes_LoteId] FOREIGN KEY ([LoteId]) REFERENCES [Lotes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE TABLE [PeriodosProductivos] (
        [Id] int NOT NULL IDENTITY,
        [LoteId] int NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NULL,
        [Observaciones] nvarchar(500) NULL,
        [Cerrado] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_PeriodosProductivos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PeriodosProductivos_Lotes_LoteId] FOREIGN KEY ([LoteId]) REFERENCES [Lotes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE TABLE [TratamientosEnfermedad] (
        [Id] int NOT NULL IDENTITY,
        [EnfermedadId] int NOT NULL,
        [Tratamiento] nvarchar(250) NOT NULL,
        [FechaAplicacion] datetime2 NOT NULL,
        [Responsable] nvarchar(150) NULL,
        [Resultado] nvarchar(500) NULL,
        [Observaciones] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_TratamientosEnfermedad] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TratamientosEnfermedad_Enfermedades_EnfermedadId] FOREIGN KEY ([EnfermedadId]) REFERENCES [Enfermedades] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE TABLE [ProductosTratamiento] (
        [Id] int NOT NULL IDENTITY,
        [TratamientoEnfermedadId] int NOT NULL,
        [NombreProducto] nvarchar(150) NOT NULL,
        [Cantidad] decimal(18,2) NOT NULL,
        [Unidad] nvarchar(30) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductosTratamiento] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductosTratamiento_TratamientosEnfermedad_TratamientoEnfermedadId] FOREIGN KEY ([TratamientoEnfermedadId]) REFERENCES [TratamientosEnfermedad] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE INDEX [IX_Enfermedades_LoteId_Nombre_FechaDeteccion] ON [Enfermedades] ([LoteId], [Nombre], [FechaDeteccion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE INDEX [IX_PeriodosProductivos_LoteId_FechaInicio] ON [PeriodosProductivos] ([LoteId], [FechaInicio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE INDEX [IX_ProductosTratamiento_TratamientoEnfermedadId] ON [ProductosTratamiento] ([TratamientoEnfermedadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    CREATE INDEX [IX_TratamientosEnfermedad_EnfermedadId] ON [TratamientosEnfermedad] ([EnfermedadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001022342_AgregarEnfermedadesYPeriodosProductivos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001022342_AgregarEnfermedadesYPeriodosProductivos', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001071712_AgregarTokensRecuperacion'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [ResetPasswordToken] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001071712_AgregarTokensRecuperacion'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [ResetPasswordTokenExpiration] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001071712_AgregarTokensRecuperacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001071712_AgregarTokensRecuperacion', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261005025452_AddTokensNuevos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261005025452_AddTokensNuevos', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE TABLE [Colaboradores] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [Identificacion] nvarchar(30) NOT NULL,
        [Telefono] nvarchar(30) NOT NULL,
        [Cargo] nvarchar(100) NOT NULL,
        [TarifaJornada] decimal(18,2) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Colaboradores] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE TABLE [Labores] (
        [Id] int NOT NULL IDENTITY,
        [LoteId] int NOT NULL,
        [Tipo] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [DuracionHoras] decimal(18,2) NOT NULL,
        [Observaciones] nvarchar(500) NOT NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Labores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Labores_Lotes_LoteId] FOREIGN KEY ([LoteId]) REFERENCES [Lotes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE TABLE [LaboresColaboradores] (
        [LaborId] int NOT NULL,
        [ColaboradorId] int NOT NULL,
        CONSTRAINT [PK_LaboresColaboradores] PRIMARY KEY ([LaborId], [ColaboradorId]),
        CONSTRAINT [FK_LaboresColaboradores_Colaboradores_ColaboradorId] FOREIGN KEY ([ColaboradorId]) REFERENCES [Colaboradores] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LaboresColaboradores_Labores_LaborId] FOREIGN KEY ([LaborId]) REFERENCES [Labores] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Colaboradores_Identificacion] ON [Colaboradores] ([Identificacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE INDEX [IX_Labores_LoteId_Fecha] ON [Labores] ([LoteId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    CREATE INDEX [IX_LaboresColaboradores_ColaboradorId] ON [LaboresColaboradores] ([ColaboradorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021011446_AgregarLaboresYColaboradores'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021011446_AgregarLaboresYColaboradores', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021013000_CorregirColumnasPasswordReset'
)
BEGIN

    IF COL_LENGTH('Usuarios', 'PasswordResetToken') IS NULL
        ALTER TABLE [Usuarios] ADD [PasswordResetToken] nvarchar(max) NULL;

    IF COL_LENGTH('Usuarios', 'PasswordResetTokenExpiration') IS NULL
        ALTER TABLE [Usuarios] ADD [PasswordResetTokenExpiration] datetime2 NULL;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021013000_CorregirColumnasPasswordReset'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021013000_CorregirColumnasPasswordReset', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    EXEC sp_rename N'[Labores].[DuracionHoras]', N'HorasTotales', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Labores] ADD [CerradaPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Labores] ADD [ComentarioCierre] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Labores] ADD [Estado] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Labores] ADD [FechaCierre] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Colaboradores] ADD [UsuarioId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    CREATE TABLE [LaboresAvances] (
        [Id] int NOT NULL IDENTITY,
        [LaborId] int NOT NULL,
        [ColaboradorId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Horas] decimal(18,2) NOT NULL,
        [Comentario] nvarchar(500) NOT NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_LaboresAvances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LaboresAvances_Colaboradores_ColaboradorId] FOREIGN KEY ([ColaboradorId]) REFERENCES [Colaboradores] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LaboresAvances_Labores_LaborId] FOREIGN KEY ([LaborId]) REFERENCES [Labores] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Colaboradores_UsuarioId] ON [Colaboradores] ([UsuarioId]) WHERE [UsuarioId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    CREATE INDEX [IX_LaboresAvances_ColaboradorId] ON [LaboresAvances] ([ColaboradorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    CREATE INDEX [IX_LaboresAvances_LaborId_Fecha] ON [LaboresAvances] ([LaborId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    ALTER TABLE [Colaboradores] ADD CONSTRAINT [FK_Colaboradores_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021033953_LaboresConSeguimiento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021033953_LaboresConSeguimiento', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Enfermedades] DROP CONSTRAINT [FK_Enfermedades_Lotes_LoteId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Labores] DROP CONSTRAINT [FK_Labores_Lotes_LoteId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [PeriodosProductivos] DROP CONSTRAINT [FK_PeriodosProductivos_Lotes_LoteId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Lotes] DROP CONSTRAINT [FK_Lotes_Fincas_FincaId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Lotes] DROP CONSTRAINT [FK_Lotes_Lotes_LotePadreId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Lotes] DROP CONSTRAINT [PK_Lotes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Fincas] DROP CONSTRAINT [PK_Fincas];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Fincas]', N'Empresas';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Lotes]', N'Terrenos';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Terrenos].[FincaId]', N'EmpresaId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Terrenos].[LotePadreId]', N'FincaPadreId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Terrenos].[IX_Lotes_FincaId_Codigo]', N'IX_Terrenos_EmpresaId_Codigo', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Terrenos].[IX_Lotes_LotePadreId]', N'IX_Terrenos_FincaPadreId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Empresas] ADD CONSTRAINT [PK_Empresas] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Terrenos] ADD CONSTRAINT [PK_Terrenos] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Enfermedades].[LoteId]', N'TerrenoId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Enfermedades].[IX_Enfermedades_LoteId_Nombre_FechaDeteccion]', N'IX_Enfermedades_TerrenoId_Nombre_FechaDeteccion', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Labores].[LoteId]', N'TerrenoId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[Labores].[IX_Labores_LoteId_Fecha]', N'IX_Labores_TerrenoId_Fecha', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[PeriodosProductivos].[LoteId]', N'TerrenoId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    EXEC sp_rename N'[PeriodosProductivos].[IX_PeriodosProductivos_LoteId_FechaInicio]', N'IX_PeriodosProductivos_TerrenoId_FechaInicio', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Terrenos] ADD CONSTRAINT [FK_Terrenos_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Terrenos] ADD CONSTRAINT [FK_Terrenos_Terrenos_FincaPadreId] FOREIGN KEY ([FincaPadreId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD CONSTRAINT [FK_Enfermedades_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [Labores] ADD CONSTRAINT [FK_Labores_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    ALTER TABLE [PeriodosProductivos] ADD CONSTRAINT [FK_PeriodosProductivos_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021035945_RenombrarEmpresaFincasYLotes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021035945_RenombrarEmpresaFincasYLotes', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [TratamientosEnfermedad] ADD [Efectividad] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [CerradaPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [ComentarioCierre] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [Estado] int NOT NULL DEFAULT 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [FechaCierre] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [ResultadoCierre] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD [TratamientoEfectivoId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    CREATE INDEX [IX_Enfermedades_TratamientoEfectivoId] ON [Enfermedades] ([TratamientoEfectivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    ALTER TABLE [Enfermedades] ADD CONSTRAINT [FK_Enfermedades_TratamientosEnfermedad_TratamientoEfectivoId] FOREIGN KEY ([TratamientoEfectivoId]) REFERENCES [TratamientosEnfermedad] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN

    UPDATE e SET e.Estado = 2
    FROM Enfermedades e
    WHERE EXISTS (SELECT 1 FROM TratamientosEnfermedad t WHERE t.EnfermedadId = e.Id);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261021042115_CierreEnfermedadesYEfectividad'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261021042115_CierreEnfermedadesYEfectividad', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN
    ALTER TABLE [LaboresAvances] ADD [Costo] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN
    ALTER TABLE [LaboresAvances] ADD [HorasPorJornada] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN
    ALTER TABLE [LaboresAvances] ADD [TarifaJornada] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN
    ALTER TABLE [Labores] ADD [CostoTotal] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN

    UPDATE a SET
        a.HorasPorJornada = 6,
        a.TarifaJornada = c.TarifaJornada,
        a.Costo = ROUND(a.Horas / 6 * c.TarifaJornada, 2)
    FROM LaboresAvances a
    JOIN Colaboradores c ON c.Id = a.ColaboradorId;

    UPDATE l SET l.CostoTotal = ISNULL((SELECT SUM(a.Costo) FROM LaboresAvances a WHERE a.LaborId = l.Id), 0)
    FROM Labores l;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022022345_JornadasYCostosLabores'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022022345_JornadasYCostosLabores', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022023932_AgregarBloqueoUsuarios'
)
BEGIN

    IF COL_LENGTH('Usuarios', 'Bloqueado') IS NULL
        ALTER TABLE [Usuarios] ADD [Bloqueado] bit NOT NULL CONSTRAINT [DF_Usuarios_Bloqueado] DEFAULT CAST(0 AS bit);

    IF COL_LENGTH('Usuarios', 'IntentosFallidos') IS NULL
        ALTER TABLE [Usuarios] ADD [IntentosFallidos] int NOT NULL CONSTRAINT [DF_Usuarios_IntentosFallidos] DEFAULT 0;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022023932_AgregarBloqueoUsuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022023932_AgregarBloqueoUsuarios', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022030256_TipoOtroEnLabores'
)
BEGIN
    ALTER TABLE [Labores] ADD [TipoOtro] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022030256_TipoOtroEnLabores'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022030256_TipoOtroEnLabores', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022033412_RenombrarCodigosDeLotes'
)
BEGIN

    UPDATE Terrenos
    SET Codigo = CONCAT('TMP-', Id)
    WHERE Tipo = 2;

    WITH Numerados AS (
        SELECT t.Id,
               LEFT(f.Codigo, 25) AS CodigoFinca,
               ROW_NUMBER() OVER (PARTITION BY t.FincaPadreId ORDER BY t.Id) AS Numero
        FROM Terrenos t
        JOIN Terrenos f ON f.Id = t.FincaPadreId
        WHERE t.Tipo = 2
    )
    UPDATE t
    SET t.Codigo = n.CodigoFinca + '-L' + RIGHT('0' + CAST(n.Numero AS varchar(4)), CASE WHEN n.Numero < 100 THEN 2 ELSE 3 END)
    FROM Terrenos t
    JOIN Numerados n ON n.Id = t.Id;

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022033412_RenombrarCodigosDeLotes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022033412_RenombrarCodigosDeLotes', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [PeriodosProductivos] DROP CONSTRAINT [FK_PeriodosProductivos_Terrenos_TerrenoId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [Terrenos] DROP CONSTRAINT [FK_Terrenos_Empresas_EmpresaId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DROP INDEX [IX_PeriodosProductivos_TerrenoId_FechaInicio] ON [PeriodosProductivos];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'ResetPasswordToken');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Usuarios] DROP COLUMN [ResetPasswordToken];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'ResetPasswordTokenExpiration');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Usuarios] DROP COLUMN [ResetPasswordTokenExpiration];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [ProductosTratamiento] ADD [InsumoId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [PeriodosProductivos] ADD [Nombre] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [Colaboradores] ADD [TarifaCajuela] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuditLogs]') AND [c].[name] = N'Usuario');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AuditLogs] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [AuditLogs] ALTER COLUMN [Usuario] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuditLogs]') AND [c].[name] = N'EntidadId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AuditLogs] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [AuditLogs] ALTER COLUMN [EntidadId] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AuditLogs]') AND [c].[name] = N'Entidad');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AuditLogs] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [AuditLogs] ALTER COLUMN [Entidad] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [DireccionIp] nvarchar(45) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [Modulo] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [UsuarioId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [ValorAnterior] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [ValorNuevo] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [CategoriasGasto] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_CategoriasGasto] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [CategoriasInsumo] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [RequiereVencimiento] bit NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_CategoriasInsumo] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [PagosColaborador] (
        [Id] int NOT NULL IDENTITY,
        [ColaboradorId] int NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [TotalJornadas] decimal(18,2) NOT NULL,
        [TotalCajuelas] decimal(18,2) NOT NULL,
        [MontoTotal] decimal(18,2) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_PagosColaborador] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PagosColaborador_Fechas] CHECK ([FechaFin] >= [FechaInicio]),
        CONSTRAINT [FK_PagosColaborador_Colaboradores_ColaboradorId] FOREIGN KEY ([ColaboradorId]) REFERENCES [Colaboradores] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [ParametrosFinca] (
        [Id] int NOT NULL IDENTITY,
        [EmpresaId] int NOT NULL,
        [KgPorCajuela] decimal(18,2) NOT NULL,
        [TamanoMaximoAdjuntoMB] int NOT NULL DEFAULT 5,
        [ExtensionesAdjuntoPermitidas] nvarchar(100) NOT NULL DEFAULT N'pdf,jpg,jpeg,png',
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_ParametrosFinca] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ParametrosFinca_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [PeriodosTerrenos] (
        [PeriodoProductivoId] int NOT NULL,
        [TerrenoId] int NOT NULL,
        CONSTRAINT [PK_PeriodosTerrenos] PRIMARY KEY ([PeriodoProductivoId], [TerrenoId]),
        CONSTRAINT [FK_PeriodosTerrenos_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PeriodosTerrenos_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [Recolecciones] (
        [Id] int NOT NULL IDENTITY,
        [ColaboradorId] int NOT NULL,
        [TerrenoId] int NOT NULL,
        [PeriodoProductivoId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Cajuelas] decimal(18,2) NOT NULL,
        [KgPorCajuela] decimal(18,2) NOT NULL,
        [PesoEstimadoKg] decimal(18,2) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Recolecciones] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Recolecciones_Cajuelas] CHECK ([Cajuelas] > 0),
        CONSTRAINT [FK_Recolecciones_Colaboradores_ColaboradorId] FOREIGN KEY ([ColaboradorId]) REFERENCES [Colaboradores] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Recolecciones_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Recolecciones_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [UnidadesMedida] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        [Abreviatura] nvarchar(10) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_UnidadesMedida] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [Ventas] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Comprador] nvarchar(150) NOT NULL,
        [Cantidad] decimal(18,2) NOT NULL,
        [Unidad] int NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [Total] AS [Cantidad] * [PrecioUnitario] PERSISTED,
        [PeriodoProductivoId] int NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Ventas] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Ventas_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [CK_Ventas_PrecioUnitario] CHECK ([PrecioUnitario] > 0),
        CONSTRAINT [FK_Ventas_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [Gastos] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [CategoriaGastoId] int NOT NULL,
        [TerrenoId] int NULL,
        [PeriodoProductivoId] int NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Proveedor] nvarchar(150) NULL,
        [AdjuntoNombreOriginal] nvarchar(255) NULL,
        [AdjuntoRuta] nvarchar(500) NULL,
        [AdjuntoTipoContenido] nvarchar(100) NULL,
        [AdjuntoTamanoBytes] bigint NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Gastos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Gastos_Monto] CHECK ([Monto] > 0),
        CONSTRAINT [FK_Gastos_CategoriasGasto_CategoriaGastoId] FOREIGN KEY ([CategoriaGastoId]) REFERENCES [CategoriasGasto] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Gastos_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Gastos_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [PagoDetalles] (
        [Id] int NOT NULL IDENTITY,
        [PagoColaboradorId] int NOT NULL,
        [Tipo] int NOT NULL,
        [LaborAvanceId] int NULL,
        [RecoleccionId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [Cantidad] decimal(18,2) NOT NULL,
        [Tarifa] decimal(18,2) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PagoDetalles] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PagoDetalles_Origen] CHECK (([Tipo] = 1 AND [LaborAvanceId] IS NOT NULL AND [RecoleccionId] IS NULL) OR ([Tipo] = 2 AND [RecoleccionId] IS NOT NULL AND [LaborAvanceId] IS NULL)),
        CONSTRAINT [FK_PagoDetalles_LaboresAvances_LaborAvanceId] FOREIGN KEY ([LaborAvanceId]) REFERENCES [LaboresAvances] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PagoDetalles_PagosColaborador_PagoColaboradorId] FOREIGN KEY ([PagoColaboradorId]) REFERENCES [PagosColaborador] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PagoDetalles_Recolecciones_RecoleccionId] FOREIGN KEY ([RecoleccionId]) REFERENCES [Recolecciones] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [PesosSecado] (
        [Id] int NOT NULL IDENTITY,
        [TerrenoId] int NOT NULL,
        [PeriodoProductivoId] int NOT NULL,
        [RecoleccionId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [PesoKg] decimal(18,2) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_PesosSecado] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PesosSecado_PesoKg] CHECK ([PesoKg] > 0),
        CONSTRAINT [FK_PesosSecado_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PesosSecado_Recolecciones_RecoleccionId] FOREIGN KEY ([RecoleccionId]) REFERENCES [Recolecciones] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PesosSecado_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [Insumos] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [CategoriaInsumoId] int NOT NULL,
        [UnidadMedidaId] int NOT NULL,
        [StockActual] decimal(18,2) NOT NULL,
        [StockMinimo] decimal(18,2) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Activo] bit NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_Insumos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Insumos_StockActual] CHECK ([StockActual] >= 0),
        CONSTRAINT [CK_Insumos_StockMinimo] CHECK ([StockMinimo] >= 0),
        CONSTRAINT [FK_Insumos_CategoriasInsumo_CategoriaInsumoId] FOREIGN KEY ([CategoriaInsumoId]) REFERENCES [CategoriasInsumo] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Insumos_UnidadesMedida_UnidadMedidaId] FOREIGN KEY ([UnidadMedidaId]) REFERENCES [UnidadesMedida] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [PesosProcesado] (
        [Id] int NOT NULL IDENTITY,
        [TerrenoId] int NOT NULL,
        [PeriodoProductivoId] int NOT NULL,
        [PesoSecadoId] int NULL,
        [Fecha] datetime2 NOT NULL,
        [PesoKg] decimal(18,2) NOT NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_PesosProcesado] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PesosProcesado_PesoKg] CHECK ([PesoKg] > 0),
        CONSTRAINT [FK_PesosProcesado_PeriodosProductivos_PeriodoProductivoId] FOREIGN KEY ([PeriodoProductivoId]) REFERENCES [PeriodosProductivos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PesosProcesado_PesosSecado_PesoSecadoId] FOREIGN KEY ([PesoSecadoId]) REFERENCES [PesosSecado] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PesosProcesado_Terrenos_TerrenoId] FOREIGN KEY ([TerrenoId]) REFERENCES [Terrenos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [AlertasStock] (
        [Id] int NOT NULL IDENTITY,
        [InsumoId] int NOT NULL,
        [FechaAlerta] datetime2 NOT NULL,
        [StockAlMomento] decimal(18,2) NOT NULL,
        [StockMinimo] decimal(18,2) NOT NULL,
        [Activa] bit NOT NULL,
        [FechaResolucion] datetime2 NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_AlertasStock] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AlertasStock_Insumos_InsumoId] FOREIGN KEY ([InsumoId]) REFERENCES [Insumos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [LaboresInsumos] (
        [Id] int NOT NULL IDENTITY,
        [LaborId] int NOT NULL,
        [InsumoId] int NOT NULL,
        [Cantidad] decimal(18,2) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_LaboresInsumos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_LaboresInsumos_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_LaboresInsumos_Insumos_InsumoId] FOREIGN KEY ([InsumoId]) REFERENCES [Insumos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LaboresInsumos_Labores_LaborId] FOREIGN KEY ([LaborId]) REFERENCES [Labores] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE TABLE [MovimientosInventario] (
        [Id] int NOT NULL IDENTITY,
        [InsumoId] int NOT NULL,
        [Tipo] int NOT NULL,
        [Cantidad] decimal(18,2) NOT NULL,
        [StockResultante] decimal(18,2) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [FechaVencimiento] datetime2 NULL,
        [Observaciones] nvarchar(500) NULL,
        [RegistradoPor] nvarchar(150) NULL,
        [ProductoTratamientoId] int NULL,
        [LaborInsumoId] int NULL,
        [GastoId] int NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [CreadoPor] nvarchar(max) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(max) NULL,
        CONSTRAINT [PK_MovimientosInventario] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_MovimientosInventario_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_MovimientosInventario_Gastos_GastoId] FOREIGN KEY ([GastoId]) REFERENCES [Gastos] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_MovimientosInventario_Insumos_InsumoId] FOREIGN KEY ([InsumoId]) REFERENCES [Insumos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MovimientosInventario_LaboresInsumos_LaborInsumoId] FOREIGN KEY ([LaborInsumoId]) REFERENCES [LaboresInsumos] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_MovimientosInventario_ProductosTratamiento_ProductoTratamientoId] FOREIGN KEY ([ProductoTratamientoId]) REFERENCES [ProductosTratamiento] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasGasto]'))
        SET IDENTITY_INSERT [CategoriasGasto] ON;
    EXEC(N'INSERT INTO [CategoriasGasto] ([Id], [Activo], [CreadoPor], [FechaCreacion], [FechaModificacion], [ModificadoPor], [Nombre])
    VALUES (1, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Insumos''),
    (2, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Mano de obra''),
    (3, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Planilla''),
    (4, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Mantenimiento''),
    (5, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Transporte''),
    (6, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Servicios''),
    (7, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Otros'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasGasto]'))
        SET IDENTITY_INSERT [CategoriasGasto] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre', N'RequiereVencimiento') AND [object_id] = OBJECT_ID(N'[CategoriasInsumo]'))
        SET IDENTITY_INSERT [CategoriasInsumo] ON;
    EXEC(N'INSERT INTO [CategoriasInsumo] ([Id], [Activo], [CreadoPor], [FechaCreacion], [FechaModificacion], [ModificadoPor], [Nombre], [RequiereVencimiento])
    VALUES (1, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Fertilizante'', CAST(0 AS bit)),
    (2, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Insumo Fitosanitario / Químico'', CAST(1 AS bit)),
    (3, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Herramienta'', CAST(0 AS bit)),
    (4, CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Otro'', CAST(0 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre', N'RequiereVencimiento') AND [object_id] = OBJECT_ID(N'[CategoriasInsumo]'))
        SET IDENTITY_INSERT [CategoriasInsumo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Abreviatura', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[UnidadesMedida]'))
        SET IDENTITY_INSERT [UnidadesMedida] ON;
    EXEC(N'INSERT INTO [UnidadesMedida] ([Id], [Abreviatura], [Activo], [CreadoPor], [FechaCreacion], [FechaModificacion], [ModificadoPor], [Nombre])
    VALUES (1, N''L'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Litro''),
    (2, N''mL'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Mililitro''),
    (3, N''gal'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Galón''),
    (4, N''kg'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Kilogramo''),
    (5, N''g'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Gramo''),
    (6, N''saco'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Saco''),
    (7, N''und'', CAST(1 AS bit), N''Sistema'', ''2026-11-04T00:00:00.0000000Z'', NULL, NULL, N''Unidad'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Abreviatura', N'Activo', N'CreadoPor', N'FechaCreacion', N'FechaModificacion', N'ModificadoPor', N'Nombre') AND [object_id] = OBJECT_ID(N'[UnidadesMedida]'))
        SET IDENTITY_INSERT [UnidadesMedida] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'ALTER TABLE [Terrenos] ADD CONSTRAINT [CK_Terrenos_AreaHectareas] CHECK ([AreaHectareas] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'ALTER TABLE [Terrenos] ADD CONSTRAINT [CK_Terrenos_TipoPadre] CHECK (([Tipo] = 1 AND [FincaPadreId] IS NULL) OR ([Tipo] = 2 AND [FincaPadreId] IS NOT NULL))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_ProductosTratamiento_InsumoId] ON [ProductosTratamiento] ([InsumoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PeriodosProductivos_FechaInicio] ON [PeriodosProductivos] ([FechaInicio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'ALTER TABLE [PeriodosProductivos] ADD CONSTRAINT [CK_PeriodosProductivos_Fechas] CHECK ([FechaFin] IS NULL OR [FechaFin] >= [FechaInicio])');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Entidad_EntidadId] ON [AuditLogs] ([Entidad], [EntidadId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_FechaHora] ON [AuditLogs] ([FechaHora]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AlertasStock_InsumoId] ON [AlertasStock] ([InsumoId]) WHERE [Activa] = 1');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriasGasto_Nombre] ON [CategoriasGasto] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriasInsumo_Nombre] ON [CategoriasInsumo] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Gastos_CategoriaGastoId] ON [Gastos] ([CategoriaGastoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Gastos_Fecha] ON [Gastos] ([Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Gastos_PeriodoProductivoId] ON [Gastos] ([PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Gastos_TerrenoId_Fecha] ON [Gastos] ([TerrenoId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Insumos_CategoriaInsumoId] ON [Insumos] ([CategoriaInsumoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Insumos_Nombre] ON [Insumos] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Insumos_UnidadMedidaId] ON [Insumos] ([UnidadMedidaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_LaboresInsumos_InsumoId] ON [LaboresInsumos] ([InsumoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LaboresInsumos_LaborId_InsumoId] ON [LaboresInsumos] ([LaborId], [InsumoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_MovimientosInventario_GastoId] ON [MovimientosInventario] ([GastoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_MovimientosInventario_InsumoId_Fecha] ON [MovimientosInventario] ([InsumoId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_MovimientosInventario_LaborInsumoId] ON [MovimientosInventario] ([LaborInsumoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_MovimientosInventario_ProductoTratamientoId] ON [MovimientosInventario] ([ProductoTratamientoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PagoDetalles_LaborAvanceId] ON [PagoDetalles] ([LaborAvanceId]) WHERE [LaborAvanceId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PagoDetalles_PagoColaboradorId] ON [PagoDetalles] ([PagoColaboradorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PagoDetalles_RecoleccionId] ON [PagoDetalles] ([RecoleccionId]) WHERE [RecoleccionId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PagosColaborador_ColaboradorId_FechaInicio] ON [PagosColaborador] ([ColaboradorId], [FechaInicio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ParametrosFinca_EmpresaId] ON [ParametrosFinca] ([EmpresaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PeriodosTerrenos_TerrenoId] ON [PeriodosTerrenos] ([TerrenoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosProcesado_PeriodoProductivoId] ON [PesosProcesado] ([PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosProcesado_PesoSecadoId] ON [PesosProcesado] ([PesoSecadoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosProcesado_TerrenoId_PeriodoProductivoId] ON [PesosProcesado] ([TerrenoId], [PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosSecado_PeriodoProductivoId] ON [PesosSecado] ([PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosSecado_RecoleccionId] ON [PesosSecado] ([RecoleccionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_PesosSecado_TerrenoId_PeriodoProductivoId] ON [PesosSecado] ([TerrenoId], [PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Recolecciones_ColaboradorId_Fecha] ON [Recolecciones] ([ColaboradorId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Recolecciones_PeriodoProductivoId_Fecha] ON [Recolecciones] ([PeriodoProductivoId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Recolecciones_TerrenoId_Fecha] ON [Recolecciones] ([TerrenoId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UnidadesMedida_Abreviatura] ON [UnidadesMedida] ([Abreviatura]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UnidadesMedida_Nombre] ON [UnidadesMedida] ([Nombre]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Ventas_Fecha] ON [Ventas] ([Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    CREATE INDEX [IX_Ventas_PeriodoProductivoId] ON [Ventas] ([PeriodoProductivoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [ProductosTratamiento] ADD CONSTRAINT [FK_ProductosTratamiento_Insumos_InsumoId] FOREIGN KEY ([InsumoId]) REFERENCES [Insumos] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    ALTER TABLE [Terrenos] ADD CONSTRAINT [FK_Terrenos_Empresas_EmpresaId] FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN

    EXEC(N'INSERT INTO [PeriodosTerrenos] ([PeriodoProductivoId], [TerrenoId])
    SELECT [Id], [TerrenoId] FROM [PeriodosProductivos];

    UPDATE [PeriodosProductivos]
    SET [Nombre] = N''Período '' + CONVERT(nvarchar(10), [FechaInicio], 103)
    WHERE [Nombre] = N'''';');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PeriodosProductivos]') AND [c].[name] = N'TerrenoId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PeriodosProductivos] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [PeriodosProductivos] DROP COLUMN [TerrenoId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN

    EXEC(N'CREATE TRIGGER [dbo].[TR_AuditLogs_SoloLectura]
    ON [dbo].[AuditLogs]
    INSTEAD OF UPDATE, DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        THROW 50001, N''La bitácora de auditoría es de solo lectura: no se permite modificar ni eliminar registros.'', 1;
    END;');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233020_TablasSprint3y4YCorrecciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104233020_TablasSprint3y4YCorrecciones', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [PesosProcesado] DROP CONSTRAINT [FK_PesosProcesado_PesosSecado_PesoSecadoId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [PesosSecado] DROP CONSTRAINT [FK_PesosSecado_Recolecciones_RecoleccionId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DROP INDEX [IX_PesosSecado_PeriodoProductivoId] ON [PesosSecado];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DROP INDEX [IX_PesosSecado_RecoleccionId] ON [PesosSecado];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DROP INDEX [IX_PesosProcesado_PeriodoProductivoId] ON [PesosProcesado];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DROP INDEX [IX_PesosProcesado_PesoSecadoId] ON [PesosProcesado];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosSecado]') AND [c].[name] = N'RecoleccionId');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [PesosSecado] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [PesosSecado] DROP COLUMN [RecoleccionId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosProcesado]') AND [c].[name] = N'PesoSecadoId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [PesosProcesado] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [PesosProcesado] DROP COLUMN [PesoSecadoId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ventas]') AND [c].[name] = N'ModificadoPor');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Ventas] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Ventas] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ventas]') AND [c].[name] = N'CreadoPor');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Ventas] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Ventas] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Rol');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [Rol] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'PasswordResetToken');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [PasswordResetToken] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'PasswordHash');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [PasswordHash] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Nombre');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [Nombre] nvarchar(150) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'ModificadoPor');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'CreadoPor');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [Usuarios] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [BloqueadoHasta] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UnidadesMedida]') AND [c].[name] = N'ModificadoPor');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [UnidadesMedida] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [UnidadesMedida] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UnidadesMedida]') AND [c].[name] = N'CreadoPor');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [UnidadesMedida] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [UnidadesMedida] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TratamientosEnfermedad]') AND [c].[name] = N'ModificadoPor');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [TratamientosEnfermedad] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [TratamientosEnfermedad] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TratamientosEnfermedad]') AND [c].[name] = N'CreadoPor');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [TratamientosEnfermedad] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [TratamientosEnfermedad] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Terrenos]') AND [c].[name] = N'ModificadoPor');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Terrenos] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [Terrenos] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Terrenos]') AND [c].[name] = N'CreadoPor');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Terrenos] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [Terrenos] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Recolecciones]') AND [c].[name] = N'ModificadoPor');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Recolecciones] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [Recolecciones] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Recolecciones]') AND [c].[name] = N'CreadoPor');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Recolecciones] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Recolecciones] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductosTratamiento]') AND [c].[name] = N'ModificadoPor');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [ProductosTratamiento] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [ProductosTratamiento] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductosTratamiento]') AND [c].[name] = N'CreadoPor');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [ProductosTratamiento] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [ProductosTratamiento] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosSecado]') AND [c].[name] = N'ModificadoPor');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [PesosSecado] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [PesosSecado] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosSecado]') AND [c].[name] = N'CreadoPor');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [PesosSecado] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [PesosSecado] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosProcesado]') AND [c].[name] = N'ModificadoPor');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [PesosProcesado] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [PesosProcesado] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var29 sysname;
    SELECT @var29 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PesosProcesado]') AND [c].[name] = N'CreadoPor');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [PesosProcesado] DROP CONSTRAINT [' + @var29 + '];');
    ALTER TABLE [PesosProcesado] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var30 sysname;
    SELECT @var30 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PeriodosProductivos]') AND [c].[name] = N'ModificadoPor');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [PeriodosProductivos] DROP CONSTRAINT [' + @var30 + '];');
    ALTER TABLE [PeriodosProductivos] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var31 sysname;
    SELECT @var31 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PeriodosProductivos]') AND [c].[name] = N'CreadoPor');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [PeriodosProductivos] DROP CONSTRAINT [' + @var31 + '];');
    ALTER TABLE [PeriodosProductivos] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var32 sysname;
    SELECT @var32 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ParametrosFinca]') AND [c].[name] = N'ModificadoPor');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [ParametrosFinca] DROP CONSTRAINT [' + @var32 + '];');
    ALTER TABLE [ParametrosFinca] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var33 sysname;
    SELECT @var33 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ParametrosFinca]') AND [c].[name] = N'CreadoPor');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [ParametrosFinca] DROP CONSTRAINT [' + @var33 + '];');
    ALTER TABLE [ParametrosFinca] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var34 sysname;
    SELECT @var34 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PagosColaborador]') AND [c].[name] = N'ModificadoPor');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [PagosColaborador] DROP CONSTRAINT [' + @var34 + '];');
    ALTER TABLE [PagosColaborador] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var35 sysname;
    SELECT @var35 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PagosColaborador]') AND [c].[name] = N'CreadoPor');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [PagosColaborador] DROP CONSTRAINT [' + @var35 + '];');
    ALTER TABLE [PagosColaborador] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var36 sysname;
    SELECT @var36 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MovimientosInventario]') AND [c].[name] = N'ModificadoPor');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [MovimientosInventario] DROP CONSTRAINT [' + @var36 + '];');
    ALTER TABLE [MovimientosInventario] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var37 sysname;
    SELECT @var37 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MovimientosInventario]') AND [c].[name] = N'CreadoPor');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [MovimientosInventario] DROP CONSTRAINT [' + @var37 + '];');
    ALTER TABLE [MovimientosInventario] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var38 sysname;
    SELECT @var38 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LaboresInsumos]') AND [c].[name] = N'ModificadoPor');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [LaboresInsumos] DROP CONSTRAINT [' + @var38 + '];');
    ALTER TABLE [LaboresInsumos] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var39 sysname;
    SELECT @var39 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LaboresInsumos]') AND [c].[name] = N'CreadoPor');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [LaboresInsumos] DROP CONSTRAINT [' + @var39 + '];');
    ALTER TABLE [LaboresInsumos] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var40 sysname;
    SELECT @var40 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LaboresAvances]') AND [c].[name] = N'ModificadoPor');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [LaboresAvances] DROP CONSTRAINT [' + @var40 + '];');
    ALTER TABLE [LaboresAvances] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var41 sysname;
    SELECT @var41 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LaboresAvances]') AND [c].[name] = N'CreadoPor');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [LaboresAvances] DROP CONSTRAINT [' + @var41 + '];');
    ALTER TABLE [LaboresAvances] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var42 sysname;
    SELECT @var42 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Labores]') AND [c].[name] = N'ModificadoPor');
    IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Labores] DROP CONSTRAINT [' + @var42 + '];');
    ALTER TABLE [Labores] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var43 sysname;
    SELECT @var43 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Labores]') AND [c].[name] = N'CreadoPor');
    IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Labores] DROP CONSTRAINT [' + @var43 + '];');
    ALTER TABLE [Labores] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var44 sysname;
    SELECT @var44 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Insumos]') AND [c].[name] = N'ModificadoPor');
    IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Insumos] DROP CONSTRAINT [' + @var44 + '];');
    ALTER TABLE [Insumos] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var45 sysname;
    SELECT @var45 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Insumos]') AND [c].[name] = N'CreadoPor');
    IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Insumos] DROP CONSTRAINT [' + @var45 + '];');
    ALTER TABLE [Insumos] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var46 sysname;
    SELECT @var46 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gastos]') AND [c].[name] = N'ModificadoPor');
    IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Gastos] DROP CONSTRAINT [' + @var46 + '];');
    ALTER TABLE [Gastos] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var47 sysname;
    SELECT @var47 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Gastos]') AND [c].[name] = N'CreadoPor');
    IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Gastos] DROP CONSTRAINT [' + @var47 + '];');
    ALTER TABLE [Gastos] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var48 sysname;
    SELECT @var48 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enfermedades]') AND [c].[name] = N'ModificadoPor');
    IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [Enfermedades] DROP CONSTRAINT [' + @var48 + '];');
    ALTER TABLE [Enfermedades] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var49 sysname;
    SELECT @var49 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enfermedades]') AND [c].[name] = N'CreadoPor');
    IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [Enfermedades] DROP CONSTRAINT [' + @var49 + '];');
    ALTER TABLE [Enfermedades] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var50 sysname;
    SELECT @var50 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enfermedades]') AND [c].[name] = N'AtendidaPor');
    IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [Enfermedades] DROP CONSTRAINT [' + @var50 + '];');
    ALTER TABLE [Enfermedades] ALTER COLUMN [AtendidaPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var51 sysname;
    SELECT @var51 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empresas]') AND [c].[name] = N'ModificadoPor');
    IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [Empresas] DROP CONSTRAINT [' + @var51 + '];');
    ALTER TABLE [Empresas] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var52 sysname;
    SELECT @var52 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empresas]') AND [c].[name] = N'CreadoPor');
    IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [Empresas] DROP CONSTRAINT [' + @var52 + '];');
    ALTER TABLE [Empresas] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var53 sysname;
    SELECT @var53 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Colaboradores]') AND [c].[name] = N'ModificadoPor');
    IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [Colaboradores] DROP CONSTRAINT [' + @var53 + '];');
    ALTER TABLE [Colaboradores] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var54 sysname;
    SELECT @var54 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Colaboradores]') AND [c].[name] = N'CreadoPor');
    IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [Colaboradores] DROP CONSTRAINT [' + @var54 + '];');
    ALTER TABLE [Colaboradores] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var55 sysname;
    SELECT @var55 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CategoriasInsumo]') AND [c].[name] = N'ModificadoPor');
    IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [CategoriasInsumo] DROP CONSTRAINT [' + @var55 + '];');
    ALTER TABLE [CategoriasInsumo] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var56 sysname;
    SELECT @var56 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CategoriasInsumo]') AND [c].[name] = N'CreadoPor');
    IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [CategoriasInsumo] DROP CONSTRAINT [' + @var56 + '];');
    ALTER TABLE [CategoriasInsumo] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var57 sysname;
    SELECT @var57 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CategoriasGasto]') AND [c].[name] = N'ModificadoPor');
    IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [CategoriasGasto] DROP CONSTRAINT [' + @var57 + '];');
    ALTER TABLE [CategoriasGasto] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var58 sysname;
    SELECT @var58 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CategoriasGasto]') AND [c].[name] = N'CreadoPor');
    IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [CategoriasGasto] DROP CONSTRAINT [' + @var58 + '];');
    ALTER TABLE [CategoriasGasto] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var59 sysname;
    SELECT @var59 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AlertasStock]') AND [c].[name] = N'ModificadoPor');
    IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [AlertasStock] DROP CONSTRAINT [' + @var59 + '];');
    ALTER TABLE [AlertasStock] ALTER COLUMN [ModificadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var60 sysname;
    SELECT @var60 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AlertasStock]') AND [c].[name] = N'CreadoPor');
    IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [AlertasStock] DROP CONSTRAINT [' + @var60 + '];');
    ALTER TABLE [AlertasStock] ALTER COLUMN [CreadoPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var61 sysname;
    SELECT @var61 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Recolecciones]') AND [c].[name] = N'PesoEstimadoKg');
    IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [Recolecciones] DROP CONSTRAINT [' + @var61 + '];');
    ALTER TABLE [Recolecciones] DROP COLUMN [PesoEstimadoKg];
    EXEC(N'ALTER TABLE [Recolecciones] ADD [PesoEstimadoKg] AS [Cajuelas] * [KgPorCajuela] PERSISTED');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    DECLARE @var62 sysname;
    SELECT @var62 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PagoDetalles]') AND [c].[name] = N'Subtotal');
    IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [PagoDetalles] DROP CONSTRAINT [' + @var62 + '];');
    ALTER TABLE [PagoDetalles] DROP COLUMN [Subtotal];
    EXEC(N'ALTER TABLE [PagoDetalles] ADD [Subtotal] AS [Cantidad] * [Tarifa] PERSISTED');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    EXEC(N'ALTER TABLE [Usuarios] ADD CONSTRAINT [CK_Usuarios_Rol] CHECK ([Rol] IN (N''Administrador'', N''Dueño'', N''Capataz'', N''Trabajador''))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    CREATE INDEX [IX_Recolecciones_PeriodoProductivoId_TerrenoId] ON [Recolecciones] ([PeriodoProductivoId], [TerrenoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    CREATE INDEX [IX_PesosSecado_PeriodoProductivoId_TerrenoId] ON [PesosSecado] ([PeriodoProductivoId], [TerrenoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    CREATE INDEX [IX_PesosProcesado_PeriodoProductivoId_TerrenoId] ON [PesosProcesado] ([PeriodoProductivoId], [TerrenoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [PesosProcesado] ADD CONSTRAINT [FK_PesosProcesado_PeriodosTerrenos_PeriodoProductivoId_TerrenoId] FOREIGN KEY ([PeriodoProductivoId], [TerrenoId]) REFERENCES [PeriodosTerrenos] ([PeriodoProductivoId], [TerrenoId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [PesosSecado] ADD CONSTRAINT [FK_PesosSecado_PeriodosTerrenos_PeriodoProductivoId_TerrenoId] FOREIGN KEY ([PeriodoProductivoId], [TerrenoId]) REFERENCES [PeriodosTerrenos] ([PeriodoProductivoId], [TerrenoId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    ALTER TABLE [Recolecciones] ADD CONSTRAINT [FK_Recolecciones_PeriodosTerrenos_PeriodoProductivoId_TerrenoId] FOREIGN KEY ([PeriodoProductivoId], [TerrenoId]) REFERENCES [PeriodosTerrenos] ([PeriodoProductivoId], [TerrenoId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104233945_RevisionEsquemaRelaciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104233945_RevisionEsquemaRelaciones', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [LaboresColaboradores] ADD [ComentarioParte] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [LaboresColaboradores] ADD [FechaParteTerminada] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [LaboresColaboradores] ADD [ParteTerminada] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [LaboresColaboradores] ADD [ParteTerminadaPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [Labores] ADD [DevueltaPor] nvarchar(150) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [Labores] ADD [FechaDevolucion] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [Labores] ADD [FechaEnvioRevision] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    ALTER TABLE [Labores] ADD [MotivoDevolucion] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN

    EXEC(N'UPDATE lc SET lc.[ParteTerminada] = 1, lc.[FechaParteTerminada] = l.[FechaCierre], lc.[ParteTerminadaPor] = l.[CerradaPor]
    FROM [LaboresColaboradores] lc
    JOIN [Labores] l ON l.[Id] = lc.[LaborId]
    WHERE l.[Estado] = 3;');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261105001754_LaboresConRevision'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261105001754_LaboresConRevision', N'8.0.11');
END;
GO

COMMIT;
GO

