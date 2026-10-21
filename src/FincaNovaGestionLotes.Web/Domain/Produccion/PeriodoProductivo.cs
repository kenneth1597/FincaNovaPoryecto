using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

public class PeriodoProductivo : AuditableEntity
{
    [Required]
    public int TerrenoId { get; set; }

    public Terreno Terreno { get; set; } = null!;

    [Required]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public bool Cerrado { get; set; }
}