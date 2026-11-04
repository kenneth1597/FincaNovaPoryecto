using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

/// <summary>
/// Peso final del café seco procesado, sin cáscara (HU-15). La merma entre
/// etapas se calcula por lote y período.
/// </summary>
public class PesoProcesado : AuditableEntity
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
