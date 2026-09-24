using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class FiltrarIndiceCodigoPorEliminado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lotes_FincaId_Codigo",
                table: "Lotes");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_FincaId_Codigo",
                table: "Lotes",
                columns: new[] { "FincaId", "Codigo" },
                unique: true,
                filter: "[Eliminado] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lotes_FincaId_Codigo",
                table: "Lotes");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_FincaId_Codigo",
                table: "Lotes",
                columns: new[] { "FincaId", "Codigo" },
                unique: true);
        }
    }
}
