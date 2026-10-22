using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class JornadasYCostosLabores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Costo",
                table: "LaboresAvances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasPorJornada",
                table: "LaboresAvances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaJornada",
                table: "LaboresAvances",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoTotal",
                table: "Labores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"
UPDATE a SET
    a.HorasPorJornada = 6,
    a.TarifaJornada = c.TarifaJornada,
    a.Costo = ROUND(a.Horas / 6 * c.TarifaJornada, 2)
FROM LaboresAvances a
JOIN Colaboradores c ON c.Id = a.ColaboradorId;

UPDATE l SET l.CostoTotal = ISNULL((SELECT SUM(a.Costo) FROM LaboresAvances a WHERE a.LaborId = l.Id), 0)
FROM Labores l;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Costo",
                table: "LaboresAvances");

            migrationBuilder.DropColumn(
                name: "HorasPorJornada",
                table: "LaboresAvances");

            migrationBuilder.DropColumn(
                name: "TarifaJornada",
                table: "LaboresAvances");

            migrationBuilder.DropColumn(
                name: "CostoTotal",
                table: "Labores");
        }
    }
}
