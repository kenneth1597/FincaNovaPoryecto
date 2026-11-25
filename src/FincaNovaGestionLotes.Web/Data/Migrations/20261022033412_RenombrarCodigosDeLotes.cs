using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincaNovaGestionLotes.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarCodigosDeLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Terrenos
SET Codigo = CONCAT('TMP-', Id)
WHERE Tipo = 2;

WITH Numerados AS (
    SELECT t.Id,
           LEFT(f.Codigo, 25) AS CodigoFinca,
           ROW_NUMBER() OVER (PARTITION BY t.FincaPadreId ORDER BY t.Id) AS Numero
    FROM Terrenos t
    JOIN Terrenos f ON f.Id = t.FincaPadreId
    WHERE t.Tipo = 2
)
UPDATE t
SET t.Codigo = n.CodigoFinca + '-L' + RIGHT('0' + CAST(n.Numero AS varchar(4)), CASE WHEN n.Numero < 100 THEN 2 ELSE 3 END)
FROM Terrenos t
JOIN Numerados n ON n.Id = t.Id;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
