using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CierreEnfermedadesYEfectividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Efectividad",
                table: "TratamientosEnfermedad",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerradaPor",
                table: "Enfermedades",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioCierre",
                table: "Enfermedades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Enfermedades",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "Enfermedades",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResultadoCierre",
                table: "Enfermedades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TratamientoEfectivoId",
                table: "Enfermedades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enfermedades_TratamientoEfectivoId",
                table: "Enfermedades",
                column: "TratamientoEfectivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermedades_TratamientosEnfermedad_TratamientoEfectivoId",
                table: "Enfermedades",
                column: "TratamientoEfectivoId",
                principalTable: "TratamientosEnfermedad",
                principalColumn: "Id");

            migrationBuilder.Sql(@"
UPDATE e SET e.Estado = 2
FROM Enfermedades e
WHERE EXISTS (SELECT 1 FROM TratamientosEnfermedad t WHERE t.EnfermedadId = e.Id);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enfermedades_TratamientosEnfermedad_TratamientoEfectivoId",
                table: "Enfermedades");

            migrationBuilder.DropIndex(
                name: "IX_Enfermedades_TratamientoEfectivoId",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "Efectividad",
                table: "TratamientosEnfermedad");

            migrationBuilder.DropColumn(
                name: "CerradaPor",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "ComentarioCierre",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "ResultadoCierre",
                table: "Enfermedades");

            migrationBuilder.DropColumn(
                name: "TratamientoEfectivoId",
                table: "Enfermedades");
        }
    }
}
