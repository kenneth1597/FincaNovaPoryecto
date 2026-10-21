using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Models;

public class PeriodoCrearViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Finca o lote")]
    public int TerrenoId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de finalización")]
    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<TerrenoOpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<TerrenoOpcionViewModel>();
}

public class TerrenoOpcionViewModel
{
    public int Id { get; set; }

    public string Texto { get; set; } = string.Empty;
}