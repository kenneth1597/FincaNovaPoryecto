using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class LaboresConSeguimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DuracionHoras",
                table: "Labores",
                newName: "HorasTotales");

            migrationBuilder.AddColumn<string>(
                name: "CerradaPor",
                table: "Labores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioCierre",
                table: "Labores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Labores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "Labores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Colaboradores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboresAvances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaborId = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboresAvances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboresAvances_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboresAvances_Labores_LaborId",
                        column: x => x.LaborId,
                        principalTable: "Labores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_UsuarioId",
                table: "Colaboradores",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LaboresAvances_ColaboradorId",
                table: "LaboresAvances",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboresAvances_LaborId_Fecha",
                table: "LaboresAvances",
                columns: new[] { "LaborId", "Fecha" });

            migrationBuilder.AddForeignKey(
                name: "FK_Colaboradores_Usuarios_UsuarioId",
                table: "Colaboradores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colaboradores_Usuarios_UsuarioId",
                table: "Colaboradores");

            migrationBuilder.DropTable(
                name: "LaboresAvances");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_UsuarioId",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "CerradaPor",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "ComentarioCierre",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "Labores");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Colaboradores");

            migrationBuilder.RenameColumn(
                name: "HorasTotales",
                table: "Labores",
                newName: "DuracionHoras");
        }
    }
}
