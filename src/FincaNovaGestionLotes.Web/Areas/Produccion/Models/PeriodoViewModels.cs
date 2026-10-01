using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Models;

public class PeriodoCrearViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Lote")]
    public int LoteId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de finalización")]
    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<LoteOpcionViewModel> Lotes { get; set; }
        = Enumerable.Empty<LoteOpcionViewModel>();
}

public class LoteOpcionViewModel
{
    public int Id { get; set; }

    public string Texto { get; set; } = string.Empty;
}