using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorregirColumnasPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Usuarios', 'PasswordResetToken') IS NULL
    ALTER TABLE [Usuarios] ADD [PasswordResetToken] nvarchar(max) NULL;

IF COL_LENGTH('Usuarios', 'PasswordResetTokenExpiration') IS NULL
    ALTER TABLE [Usuarios] ADD [PasswordResetTokenExpiration] datetime2 NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Usuarios', 'PasswordResetTokenExpiration') IS NOT NULL
    ALTER TABLE [Usuarios] DROP COLUMN [PasswordResetTokenExpiration];

IF COL_LENGTH('Usuarios', 'PasswordResetToken') IS NOT NULL
    ALTER TABLE [Usuarios] DROP COLUMN [PasswordResetToken];
");
        }
    }
}
