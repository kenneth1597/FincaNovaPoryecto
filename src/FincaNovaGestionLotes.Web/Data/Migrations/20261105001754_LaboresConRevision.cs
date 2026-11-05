using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class LaboresConRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComentarioParte",
                table: "LaboresColaboradores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaParteTerminada",
                table: "LaboresColaboradores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ParteTerminada",
                table: "LaboresColaboradores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParteTerminadaPor",
                table: "LaboresColaboradores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevueltaPor",
                table: "Labores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion",
                table: "Labores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEnvioRevision",
                table: "Labores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoDevolucion",
                table: "Labores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Labores cerradas antes de existir la revisión: sus partes se dan por terminadas.
            // Va en EXEC para que el script idempotente compile aunque la columna aún no exista.
            migrationBuilder.Sql(@"
EXEC(N'UPDATE lc SET lc.[ParteTerminada] = 1, lc.[FechaParteTerminada] = l.[FechaCierre], lc.[ParteTerminadaPor] = l.[CerradaPor]
FROM [LaboresColaboradores] lc
JOIN [Labores] l ON l.[Id] = lc.[LaborId]
WHERE l.[Estado] = 3;');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComentarioParte",
                table: "LaboresColaboradores");

            migrationBuilder.DropColumn(
                name: "FechaParteTerminada",
                table: "LaboresColaboradores");

            migrationBuilder.DropColumn(
                name: "ParteTerminada",
                table: "LaboresColaboradores");

            migrationBuilder.DropColumn(
                name: "ParteTerminadaPor",
                table: "LaboresColaboradores");

            migrationBuilder.DropColumn(
                name: "DevueltaPor",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "FechaEnvioRevision",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "MotivoDevolucion",
                table: "Labores");
        }
    }
}
