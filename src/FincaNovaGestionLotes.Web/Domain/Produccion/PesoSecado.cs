using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

/// <summary>
/// Peso del café después del secado (HU-14). Un secado agrupa la cosecha de un
/// lote en un período, por eso la comparación con lo recolectado se hace por
/// lote y período y no contra una recolección individual.
/// </summary>
public class PesoSecado : AuditableEntity
{
    public int TerrenoId { get; set; }
    public Terreno Terreno { get; set; } = null!;

    public int PeriodoProductivoId { get; set; }
    public PeriodoProductivo PeriodoProductivo { get; set; } = null!;

    /// <summary>Garantiza que el lote esté asignado al período.</summary>
    public PeriodoTerreno PeriodoTerreno { get; set; } = null!;

    public DateTime Fecha { get; set; }

    [Range(0.01, 100000000)]
    public decimal PesoKg { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }
}
