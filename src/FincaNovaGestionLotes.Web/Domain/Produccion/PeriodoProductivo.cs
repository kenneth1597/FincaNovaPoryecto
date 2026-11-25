using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

public class PeriodoProductivo : AuditableEntity
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>Cierre explícito del período; tener fecha de fin no lo cierra por sí solo.</summary>
    public bool Cerrado { get; set; }

    /// <summary>Fincas y lotes que participan en el período.</summary>
    public ICollection<PeriodoTerreno> Terrenos { get; set; } = new List<PeriodoTerreno>();
}
