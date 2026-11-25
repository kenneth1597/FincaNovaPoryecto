using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Models;

public class VentaFiltroViewModel
{
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }

    public int Pagina { get; set; } = 1;

    public IReadOnlyList<VentaListItemViewModel> Resultados { get; set; } = Array.Empty<VentaListItemViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();

    /// <summary>Suma de todas las ventas que cumplen el filtro (no solo las de la página).</summary>
    public decimal TotalVendido { get; set; }
}

public class VentaListItemViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Comprador { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public UnidadVentaCafe Unidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
    public string? Periodo { get; set; }
}

public class VentaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha de la venta es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de la venta")]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El nombre del comprador es obligatorio.")]
    [StringLength(150, ErrorMessage = "El comprador no puede pasar de 150 caracteres.")]
    [Display(Name = "Comprador")]
    public string Comprador { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0.01, 100000000, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    [Display(Name = "Cantidad")]
    public decimal? Cantidad { get; set; } = 0.5m;

    [Display(Name = "Unidad")]
    public UnidadVentaCafe Unidad { get; set; } = UnidadVentaCafe.Cajuela;

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Range(0.01, 1000000000, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
    [Display(Name = "Precio unitario (₡)")]
    public decimal? PrecioUnitario { get; set; }

    [Display(Name = "Período productivo")]
    public int? PeriodoProductivoId { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden pasar de 500 caracteres.")]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<SelectListItem> Periodos { get; set; } = Enumerable.Empty<SelectListItem>();
}
