using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBloqueoUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Usuarios', 'Bloqueado') IS NULL
    ALTER TABLE [Usuarios] ADD [Bloqueado] bit NOT NULL CONSTRAINT [DF_Usuarios_Bloqueado] DEFAULT CAST(0 AS bit);

IF COL_LENGTH('Usuarios', 'IntentosFallidos') IS NULL
    ALTER TABLE [Usuarios] ADD [IntentosFallidos] int NOT NULL CONSTRAINT [DF_Usuarios_IntentosFallidos] DEFAULT 0;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('DF_Usuarios_Bloqueado', 'D') IS NOT NULL
    ALTER TABLE [Usuarios] DROP CONSTRAINT [DF_Usuarios_Bloqueado];
IF COL_LENGTH('Usuarios', 'Bloqueado') IS NOT NULL
    ALTER TABLE [Usuarios] DROP COLUMN [Bloqueado];

IF OBJECT_ID('DF_Usuarios_IntentosFallidos', 'D') IS NOT NULL
    ALTER TABLE [Usuarios] DROP CONSTRAINT [DF_Usuarios_IntentosFallidos];
IF COL_LENGTH('Usuarios', 'IntentosFallidos') IS NOT NULL
    ALTER TABLE [Usuarios] DROP COLUMN [IntentosFallidos];
");
        }
    }
}
