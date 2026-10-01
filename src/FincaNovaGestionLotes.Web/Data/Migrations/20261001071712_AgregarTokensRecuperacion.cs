using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTokensRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ResetPasswordTokenExpiration",
                table: "Usuarios");
        }
    }
}
