using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Lotes;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

public class PeriodoProductivo : AuditableEntity
{
    [Required]
    public int LoteId { get; set; }

    public Lote Lote { get; set; } = null!;

    [Required]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public bool Cerrado { get; set; }
}