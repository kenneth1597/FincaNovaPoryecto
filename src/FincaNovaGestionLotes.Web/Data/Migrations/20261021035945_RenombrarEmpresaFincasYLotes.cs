using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarEmpresaFincasYLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Enfermedades_Lotes_LoteId", table: "Enfermedades");
            migrationBuilder.DropForeignKey(name: "FK_Labores_Lotes_LoteId", table: "Labores");
            migrationBuilder.DropForeignKey(name: "FK_PeriodosProductivos_Lotes_LoteId", table: "PeriodosProductivos");
            migrationBuilder.DropForeignKey(name: "FK_Lotes_Fincas_FincaId", table: "Lotes");
            migrationBuilder.DropForeignKey(name: "FK_Lotes_Lotes_LotePadreId", table: "Lotes");

            migrationBuilder.DropPrimaryKey(name: "PK_Lotes", table: "Lotes");
            migrationBuilder.DropPrimaryKey(name: "PK_Fincas", table: "Fincas");

            migrationBuilder.RenameTable(name: "Fincas", newName: "Empresas");
            migrationBuilder.RenameTable(name: "Lotes", newName: "Terrenos");

            migrationBuilder.RenameColumn(name: "FincaId", table: "Terrenos", newName: "EmpresaId");
            migrationBuilder.RenameColumn(name: "LotePadreId", table: "Terrenos", newName: "FincaPadreId");

            migrationBuilder.RenameIndex(name: "IX_Lotes_FincaId_Codigo", table: "Terrenos", newName: "IX_Terrenos_EmpresaId_Codigo");
            migrationBuilder.RenameIndex(name: "IX_Lotes_LotePadreId", table: "Terrenos", newName: "IX_Terrenos_FincaPadreId");

            migrationBuilder.AddPrimaryKey(name: "PK_Empresas", table: "Empresas", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Terrenos", table: "Terrenos", column: "Id");

            migrationBuilder.RenameColumn(name: "LoteId", table: "Enfermedades", newName: "TerrenoId");
            migrationBuilder.RenameIndex(
                name: "IX_Enfermedades_LoteId_Nombre_FechaDeteccion",
                table: "Enfermedades",
                newName: "IX_Enfermedades_TerrenoId_Nombre_FechaDeteccion");

            migrationBuilder.RenameColumn(name: "LoteId", table: "Labores", newName: "TerrenoId");
            migrationBuilder.RenameIndex(name: "IX_Labores_LoteId_Fecha", table: "Labores", newName: "IX_Labores_TerrenoId_Fecha");

            migrationBuilder.RenameColumn(name: "LoteId", table: "PeriodosProductivos", newName: "TerrenoId");
            migrationBuilder.RenameIndex(
                name: "IX_PeriodosProductivos_LoteId_FechaInicio",
                table: "PeriodosProductivos",
                newName: "IX_PeriodosProductivos_TerrenoId_FechaInicio");

            migrationBuilder.AddForeignKey(
                name: "FK_Terrenos_Empresas_EmpresaId",
                table: "Terrenos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terrenos_Terrenos_FincaPadreId",
                table: "Terrenos",
                column: "FincaPadreId",
                principalTable: "Terrenos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermedades_Terrenos_TerrenoId",
                table: "Enfermedades",
                column: "TerrenoId",
                principalTable: "Terrenos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labores_Terrenos_TerrenoId",
                table: "Labores",
                column: "TerrenoId",
                principalTable: "Terrenos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosProductivos_Terrenos_TerrenoId",
                table: "PeriodosProductivos",
                column: "TerrenoId",
                principalTable: "Terrenos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Enfermedades_Terrenos_TerrenoId", table: "Enfermedades");
            migrationBuilder.DropForeignKey(name: "FK_Labores_Terrenos_TerrenoId", table: "Labores");
            migrationBuilder.DropForeignKey(name: "FK_PeriodosProductivos_Terrenos_TerrenoId", table: "PeriodosProductivos");
            migrationBuilder.DropForeignKey(name: "FK_Terrenos_Empresas_EmpresaId", table: "Terrenos");
            migrationBuilder.DropForeignKey(name: "FK_Terrenos_Terrenos_FincaPadreId", table: "Terrenos");

            migrationBuilder.RenameIndex(
                name: "IX_PeriodosProductivos_TerrenoId_FechaInicio",
                table: "PeriodosProductivos",
                newName: "IX_PeriodosProductivos_LoteId_FechaInicio");
            migrationBuilder.RenameColumn(name: "TerrenoId", table: "PeriodosProductivos", newName: "LoteId");

            migrationBuilder.RenameIndex(name: "IX_Labores_TerrenoId_Fecha", table: "Labores", newName: "IX_Labores_LoteId_Fecha");
            migrationBuilder.RenameColumn(name: "TerrenoId", table: "Labores", newName: "LoteId");

            migrationBuilder.RenameIndex(
                name: "IX_Enfermedades_TerrenoId_Nombre_FechaDeteccion",
                table: "Enfermedades",
                newName: "IX_Enfermedades_LoteId_Nombre_FechaDeteccion");
            migrationBuilder.RenameColumn(name: "TerrenoId", table: "Enfermedades", newName: "LoteId");

            migrationBuilder.DropPrimaryKey(name: "PK_Terrenos", table: "Terrenos");
            migrationBuilder.DropPrimaryKey(name: "PK_Empresas", table: "Empresas");

            migrationBuilder.RenameIndex(name: "IX_Terrenos_FincaPadreId", table: "Terrenos", newName: "IX_Lotes_LotePadreId");
            migrationBuilder.RenameIndex(name: "IX_Terrenos_EmpresaId_Codigo", table: "Terrenos", newName: "IX_Lotes_FincaId_Codigo");

            migrationBuilder.RenameColumn(name: "FincaPadreId", table: "Terrenos", newName: "LotePadreId");
            migrationBuilder.RenameColumn(name: "EmpresaId", table: "Terrenos", newName: "FincaId");

            migrationBuilder.RenameTable(name: "Terrenos", newName: "Lotes");
            migrationBuilder.RenameTable(name: "Empresas", newName: "Fincas");

            migrationBuilder.AddPrimaryKey(name: "PK_Fincas", table: "Fincas", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Lotes", table: "Lotes", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Fincas_FincaId",
                table: "Lotes",
                column: "FincaId",
                principalTable: "Fincas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lotes_Lotes_LotePadreId",
                table: "Lotes",
                column: "LotePadreId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enfermedades_Lotes_LoteId",
                table: "Enfermedades",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labores_Lotes_LoteId",
                table: "Labores",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosProductivos_Lotes_LoteId",
                table: "PeriodosProductivos",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
