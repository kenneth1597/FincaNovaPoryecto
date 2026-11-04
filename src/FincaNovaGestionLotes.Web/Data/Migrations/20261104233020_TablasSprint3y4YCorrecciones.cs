using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TablasSprint3y4YCorrecciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodosProductivos_Terrenos_TerrenoId",
                table: "PeriodosProductivos");

            migrationBuilder.DropForeignKey(
                name: "FK_Terrenos_Empresas_EmpresaId",
                table: "Terrenos");

            migrationBuilder.DropIndex(
                name: "IX_PeriodosProductivos_TerrenoId_FechaInicio",
                table: "PeriodosProductivos");

            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiration",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "InsumoId",
                table: "ProductosTratamiento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "PeriodosProductivos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaCajuela",
                table: "Colaboradores",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "AuditLogs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntidadId",
                table: "AuditLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Entidad",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DireccionIp",
                table: "AuditLogs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modulo",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorAnterior",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorNuevo",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoriasGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasGasto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasInsumo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequiereVencimiento = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasInsumo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagosColaborador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalJornadas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCajuelas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosColaborador", x => x.Id);
                    table.CheckConstraint("CK_PagosColaborador_Fechas", "[FechaFin] >= [FechaInicio]");
                    table.ForeignKey(
                        name: "FK_PagosColaborador_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosFinca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    KgPorCajuela = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TamanoMaximoAdjuntoMB = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    ExtensionesAdjuntoPermitidas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "pdf,jpg,jpeg,png"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosFinca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosFinca_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosTerrenos",
                columns: table => new
                {
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: false),
                    TerrenoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosTerrenos", x => new { x.PeriodoProductivoId, x.TerrenoId });
                    table.ForeignKey(
                        name: "FK_PeriodosTerrenos_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeriodosTerrenos_Terrenos_TerrenoId",
                        column: x => x.TerrenoId,
                        principalTable: "Terrenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recolecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColaboradorId = table.Column<int>(type: "int", nullable: false),
                    TerrenoId = table.Column<int>(type: "int", nullable: false),
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cajuelas = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KgPorCajuela = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PesoEstimadoKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recolecciones", x => x.Id);
                    table.CheckConstraint("CK_Recolecciones_Cajuelas", "[Cajuelas] > 0");
                    table.ForeignKey(
                        name: "FK_Recolecciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recolecciones_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recolecciones_Terrenos_TerrenoId",
                        column: x => x.TerrenoId,
                        principalTable: "Terrenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Abreviatura = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comprador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Unidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "[Cantidad] * [PrecioUnitario]", stored: true),
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.CheckConstraint("CK_Ventas_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_Ventas_PrecioUnitario", "[PrecioUnitario] > 0");
                    table.ForeignKey(
                        name: "FK_Ventas_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoriaGastoId = table.Column<int>(type: "int", nullable: false),
                    TerrenoId = table.Column<int>(type: "int", nullable: true),
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AdjuntoNombreOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AdjuntoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdjuntoTipoContenido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdjuntoTamanoBytes = table.Column<long>(type: "bigint", nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.CheckConstraint("CK_Gastos_Monto", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_Gastos_CategoriasGasto_CategoriaGastoId",
                        column: x => x.CategoriaGastoId,
                        principalTable: "CategoriasGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gastos_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gastos_Terrenos_TerrenoId",
                        column: x => x.TerrenoId,
                        principalTable: "Terrenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagoDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PagoColaboradorId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    LaborAvanceId = table.Column<int>(type: "int", nullable: true),
                    RecoleccionId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tarifa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoDetalles", x => x.Id);
                    table.CheckConstraint("CK_PagoDetalles_Origen", "([Tipo] = 1 AND [LaborAvanceId] IS NOT NULL AND [RecoleccionId] IS NULL) OR ([Tipo] = 2 AND [RecoleccionId] IS NOT NULL AND [LaborAvanceId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_PagoDetalles_LaboresAvances_LaborAvanceId",
                        column: x => x.LaborAvanceId,
                        principalTable: "LaboresAvances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagoDetalles_PagosColaborador_PagoColaboradorId",
                        column: x => x.PagoColaboradorId,
                        principalTable: "PagosColaborador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagoDetalles_Recolecciones_RecoleccionId",
                        column: x => x.RecoleccionId,
                        principalTable: "Recolecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PesosSecado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TerrenoId = table.Column<int>(type: "int", nullable: false),
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: false),
                    RecoleccionId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PesosSecado", x => x.Id);
                    table.CheckConstraint("CK_PesosSecado_PesoKg", "[PesoKg] > 0");
                    table.ForeignKey(
                        name: "FK_PesosSecado_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PesosSecado_Recolecciones_RecoleccionId",
                        column: x => x.RecoleccionId,
                        principalTable: "Recolecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PesosSecado_Terrenos_TerrenoId",
                        column: x => x.TerrenoId,
                        principalTable: "Terrenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CategoriaInsumoId = table.Column<int>(type: "int", nullable: false),
                    UnidadMedidaId = table.Column<int>(type: "int", nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.CheckConstraint("CK_Insumos_StockActual", "[StockActual] >= 0");
                    table.CheckConstraint("CK_Insumos_StockMinimo", "[StockMinimo] >= 0");
                    table.ForeignKey(
                        name: "FK_Insumos_CategoriasInsumo_CategoriaInsumoId",
                        column: x => x.CategoriaInsumoId,
                        principalTable: "CategoriasInsumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Insumos_UnidadesMedida_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PesosProcesado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TerrenoId = table.Column<int>(type: "int", nullable: false),
                    PeriodoProductivoId = table.Column<int>(type: "int", nullable: false),
                    PesoSecadoId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PesosProcesado", x => x.Id);
                    table.CheckConstraint("CK_PesosProcesado_PesoKg", "[PesoKg] > 0");
                    table.ForeignKey(
                        name: "FK_PesosProcesado_PeriodosProductivos_PeriodoProductivoId",
                        column: x => x.PeriodoProductivoId,
                        principalTable: "PeriodosProductivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PesosProcesado_PesosSecado_PesoSecadoId",
                        column: x => x.PesoSecadoId,
                        principalTable: "PesosSecado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PesosProcesado_Terrenos_TerrenoId",
                        column: x => x.TerrenoId,
                        principalTable: "Terrenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlertasStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsumoId = table.Column<int>(type: "int", nullable: false),
                    FechaAlerta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StockAlMomento = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasStock_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboresInsumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaborId = table.Column<int>(type: "int", nullable: false),
                    InsumoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboresInsumos", x => x.Id);
                    table.CheckConstraint("CK_LaboresInsumos_Cantidad", "[Cantidad] > 0");
                    table.ForeignKey(
                        name: "FK_LaboresInsumos_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboresInsumos_Labores_LaborId",
                        column: x => x.LaborId,
                        principalTable: "Labores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsumoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StockResultante = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProductoTratamientoId = table.Column<int>(type: "int", nullable: true),
                    LaborInsumoId = table.Column<int>(type: "int", nullable: true),
                    GastoId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.CheckConstraint("CK_MovimientosInventario_Cantidad", "[Cantidad] > 0");
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Gastos_GastoId",
                        column: x => x.GastoId,
                        principalTable: "Gastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_LaboresInsumos_LaborInsumoId",
                        column: x => x.LaborInsumoId,
                        principalTable: "LaboresInsumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_ProductosTratamiento_ProductoTratamientoId",
                        column: x => x.ProductoTratamientoId,
                        principalTable: "ProductosTratamiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "CategoriasGasto",
                columns: new[] { "Id", "Activo", "CreadoPor", "FechaCreacion", "FechaModificacion", "ModificadoPor", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Insumos" },
                    { 2, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Mano de obra" },
                    { 3, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Planilla" },
                    { 4, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Mantenimiento" },
                    { 5, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Transporte" },
                    { 6, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Servicios" },
                    { 7, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Otros" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasInsumo",
                columns: new[] { "Id", "Activo", "CreadoPor", "FechaCreacion", "FechaModificacion", "ModificadoPor", "Nombre", "RequiereVencimiento" },
                values: new object[,]
                {
                    { 1, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Fertilizante", false },
                    { 2, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Insumo Fitosanitario / Químico", true },
                    { 3, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Herramienta", false },
                    { 4, true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Otro", false }
                });

            migrationBuilder.InsertData(
                table: "UnidadesMedida",
                columns: new[] { "Id", "Abreviatura", "Activo", "CreadoPor", "FechaCreacion", "FechaModificacion", "ModificadoPor", "Nombre" },
                values: new object[,]
                {
                    { 1, "L", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Litro" },
                    { 2, "mL", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Mililitro" },
                    { 3, "gal", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Galón" },
                    { 4, "kg", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Kilogramo" },
                    { 5, "g", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Gramo" },
                    { 6, "saco", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Saco" },
                    { 7, "und", true, "Sistema", new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Unidad" }
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Terrenos_AreaHectareas",
                table: "Terrenos",
                sql: "[AreaHectareas] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Terrenos_TipoPadre",
                table: "Terrenos",
                sql: "([Tipo] = 1 AND [FincaPadreId] IS NULL) OR ([Tipo] = 2 AND [FincaPadreId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosTratamiento_InsumoId",
                table: "ProductosTratamiento",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosProductivos_FechaInicio",
                table: "PeriodosProductivos",
                column: "FechaInicio");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PeriodosProductivos_Fechas",
                table: "PeriodosProductivos",
                sql: "[FechaFin] IS NULL OR [FechaFin] >= [FechaInicio]");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entidad_EntidadId",
                table: "AuditLogs",
                columns: new[] { "Entidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_FechaHora",
                table: "AuditLogs",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_AlertasStock_InsumoId",
                table: "AlertasStock",
                column: "InsumoId",
                unique: true,
                filter: "[Activa] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasGasto_Nombre",
                table: "CategoriasGasto",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasInsumo_Nombre",
                table: "CategoriasInsumo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_CategoriaGastoId",
                table: "Gastos",
                column: "CategoriaGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_Fecha",
                table: "Gastos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_PeriodoProductivoId",
                table: "Gastos",
                column: "PeriodoProductivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_TerrenoId_Fecha",
                table: "Gastos",
                columns: new[] { "TerrenoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_CategoriaInsumoId",
                table: "Insumos",
                column: "CategoriaInsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_Nombre",
                table: "Insumos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_UnidadMedidaId",
                table: "Insumos",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboresInsumos_InsumoId",
                table: "LaboresInsumos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboresInsumos_LaborId_InsumoId",
                table: "LaboresInsumos",
                columns: new[] { "LaborId", "InsumoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_GastoId",
                table: "MovimientosInventario",
                column: "GastoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_InsumoId_Fecha",
                table: "MovimientosInventario",
                columns: new[] { "InsumoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_LaborInsumoId",
                table: "MovimientosInventario",
                column: "LaborInsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_ProductoTratamientoId",
                table: "MovimientosInventario",
                column: "ProductoTratamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoDetalles_LaborAvanceId",
                table: "PagoDetalles",
                column: "LaborAvanceId",
                unique: true,
                filter: "[LaborAvanceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PagoDetalles_PagoColaboradorId",
                table: "PagoDetalles",
                column: "PagoColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_PagoDetalles_RecoleccionId",
                table: "PagoDetalles",
                column: "RecoleccionId",
                unique: true,
                filter: "[RecoleccionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PagosColaborador_ColaboradorId_FechaInicio",
                table: "PagosColaborador",
                columns: new[] { "ColaboradorId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosFinca_EmpresaId",
                table: "ParametrosFinca",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosTerrenos_TerrenoId",
                table: "PeriodosTerrenos",
                column: "TerrenoId");

            migrationBuilder.CreateIndex(
                name: "IX_PesosProcesado_PeriodoProductivoId",
                table: "PesosProcesado",
                column: "PeriodoProductivoId");

            migrationBuilder.CreateIndex(
                name: "IX_PesosProcesado_PesoSecadoId",
                table: "PesosProcesado",
                column: "PesoSecadoId");

            migrationBuilder.CreateIndex(
                name: "IX_PesosProcesado_TerrenoId_PeriodoProductivoId",
                table: "PesosProcesado",
                columns: new[] { "TerrenoId", "PeriodoProductivoId" });

            migrationBuilder.CreateIndex(
                name: "IX_PesosSecado_PeriodoProductivoId",
                table: "PesosSecado",
                column: "PeriodoProductivoId");

            migrationBuilder.CreateIndex(
                name: "IX_PesosSecado_RecoleccionId",
                table: "PesosSecado",
                column: "RecoleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_PesosSecado_TerrenoId_PeriodoProductivoId",
                table: "PesosSecado",
                columns: new[] { "TerrenoId", "PeriodoProductivoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Recolecciones_ColaboradorId_Fecha",
                table: "Recolecciones",
                columns: new[] { "ColaboradorId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Recolecciones_PeriodoProductivoId_Fecha",
                table: "Recolecciones",
                columns: new[] { "PeriodoProductivoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Recolecciones_TerrenoId_Fecha",
                table: "Recolecciones",
                columns: new[] { "TerrenoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Abreviatura",
                table: "UnidadesMedida",
                column: "Abreviatura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Nombre",
                table: "UnidadesMedida",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_PeriodoProductivoId",
                table: "Ventas",
                column: "PeriodoProductivoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductosTratamiento_Insumos_InsumoId",
                table: "ProductosTratamiento",
                column: "InsumoId",
                principalTable: "Insumos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terrenos_Empresas_EmpresaId",
                table: "Terrenos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Períodos existentes: pasan su terreno a PeriodosTerrenos y reciben un nombre
            // antes de eliminar la columna TerrenoId. Va dentro de EXEC para que el script
            // idempotente compile en bases donde TerrenoId ya no existe.
            migrationBuilder.Sql(@"
EXEC(N'INSERT INTO [PeriodosTerrenos] ([PeriodoProductivoId], [TerrenoId])
SELECT [Id], [TerrenoId] FROM [PeriodosProductivos];

UPDATE [PeriodosProductivos]
SET [Nombre] = N''Período '' + CONVERT(nvarchar(10), [FechaInicio], 103)
WHERE [Nombre] = N'''';');");

            migrationBuilder.DropColumn(
                name: "TerrenoId",
                table: "PeriodosProductivos");

            // HU-18: la bitácora de auditoría no se puede modificar ni borrar.
            // Va dentro de EXEC porque CREATE TRIGGER debe ser la única instrucción
            // de su lote, y el script idempotente la envuelve en un IF.
            migrationBuilder.Sql(@"
EXEC(N'CREATE TRIGGER [dbo].[TR_AuditLogs_SoloLectura]
ON [dbo].[AuditLogs]
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    THROW 50001, N''La bitácora de auditoría es de solo lectura: no se permite modificar ni eliminar registros.'', 1;
END;');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS [dbo].[TR_AuditLogs_SoloLectura];");

            // Guarda un terreno por período para restaurar la columna TerrenoId.
            migrationBuilder.Sql(@"
SELECT [PeriodoProductivoId], MIN([TerrenoId]) AS [TerrenoId]
INTO [__PeriodosTerrenosRespaldo]
FROM [PeriodosTerrenos]
GROUP BY [PeriodoProductivoId];");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductosTratamiento_Insumos_InsumoId",
                table: "ProductosTratamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_Terrenos_Empresas_EmpresaId",
                table: "Terrenos");

            migrationBuilder.DropTable(
                name: "AlertasStock");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "PagoDetalles");

            migrationBuilder.DropTable(
                name: "ParametrosFinca");

            migrationBuilder.DropTable(
                name: "PeriodosTerrenos");

            migrationBuilder.DropTable(
                name: "PesosProcesado");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "LaboresInsumos");

            migrationBuilder.DropTable(
                name: "PagosColaborador");

            migrationBuilder.DropTable(
                name: "PesosSecado");

            migrationBuilder.DropTable(
                name: "CategoriasGasto");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Recolecciones");

            migrationBuilder.DropTable(
                name: "CategoriasInsumo");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Terrenos_AreaHectareas",
                table: "Terrenos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Terrenos_TipoPadre",
                table: "Terrenos");

            migrationBuilder.DropIndex(
                name: "IX_ProductosTratamiento_InsumoId",
                table: "ProductosTratamiento");

            migrationBuilder.DropIndex(
                name: "IX_PeriodosProductivos_FechaInicio",
                table: "PeriodosProductivos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PeriodosProductivos_Fechas",
                table: "PeriodosProductivos");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Entidad_EntidadId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_FechaHora",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "InsumoId",
                table: "ProductosTratamiento");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "PeriodosProductivos");

            migrationBuilder.DropColumn(
                name: "TarifaCajuela",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "DireccionIp",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Modulo",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ValorAnterior",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ValorNuevo",
                table: "AuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordTokenExpiration",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerrenoId",
                table: "PeriodosProductivos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntidadId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Entidad",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.Sql(@"
UPDATE p SET p.[TerrenoId] = r.[TerrenoId]
FROM [PeriodosProductivos] p
JOIN [__PeriodosTerrenosRespaldo] r ON r.[PeriodoProductivoId] = p.[Id];
DROP TABLE [__PeriodosTerrenosRespaldo];");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosProductivos_TerrenoId_FechaInicio",
                table: "PeriodosProductivos",
                columns: new[] { "TerrenoId", "FechaInicio" });

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosProductivos_Terrenos_TerrenoId",
                table: "PeriodosProductivos",
                column: "TerrenoId",
                principalTable: "Terrenos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terrenos_Empresas_EmpresaId",
                table: "Terrenos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
