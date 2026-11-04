using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;

namespace FincaNovaGestionLotes.Web.Areas.Produccion.Models;

public class PeriodoCrearViewModel
{
    [Required(ErrorMessage = "El nombre del período es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre del período")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de finalización")]
    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    /// <summary>Fincas y lotes seleccionados para el período.</summary>
    [Display(Name = "Fincas y lotes")]
    public List<int> TerrenoIds { get; set; } = new();

    public IEnumerable<TerrenoOpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<TerrenoOpcionViewModel>();
}

public class PeriodoAsignarTerrenosViewModel
{
    public int PeriodoId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [Display(Name = "Fincas y lotes")]
    public List<int> TerrenoIds { get; set; } = new();

    public IEnumerable<TerrenoOpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<TerrenoOpcionViewModel>();
}

public class TerrenoOpcionViewModel
{
    public int Id { get; set; }

    public string Texto { get; set; } = string.Empty;

    public TipoTerreno Tipo { get; set; }
}
