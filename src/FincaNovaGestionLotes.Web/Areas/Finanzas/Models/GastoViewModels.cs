using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Models;

public class GastoFiltroViewModel
{
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }

    public int? CategoriaId { get; set; }

    public int Pagina { get; set; } = 1;

    public IReadOnlyList<GastoListItemViewModel> Resultados { get; set; } = Array.Empty<GastoListItemViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();
    public IEnumerable<SelectListItem> Categorias { get; set; } = Enumerable.Empty<SelectListItem>();

    /// <summary>Suma de todos los gastos que cumplen el filtro (no solo los de la página).</summary>
    public decimal TotalGastado { get; set; }
}

public class GastoListItemViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Proveedor { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? Terreno { get; set; }
    public decimal Monto { get; set; }
    public bool TieneComprobante { get; set; }
}

public class GastoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha del gasto es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha del gasto")]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 1000000000, ErrorMessage = "El monto debe ser mayor que cero.")]
    [Display(Name = "Monto (₡)")]
    public decimal? Monto { get; set; }

    [Required(ErrorMessage = "Seleccione la categoría del gasto.")]
    [Display(Name = "Categoría")]
    public int? CategoriaGastoId { get; set; }

    [Display(Name = "Finca o lote")]
    public int? TerrenoId { get; set; }

    [Display(Name = "Período productivo")]
    public int? PeriodoProductivoId { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede pasar de 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "El proveedor no puede pasar de 150 caracteres.")]
    [Display(Name = "Proveedor")]
    public string? Proveedor { get; set; }

    [Display(Name = "Comprobante (PDF, JPG o PNG, máx. 5 MB)")]
    public IFormFile? Comprobante { get; set; }

    /// <summary>Solo al editar: borra el comprobante que ya tenía el gasto.</summary>
    [Display(Name = "Quitar el comprobante actual")]
    public bool QuitarComprobante { get; set; }

    /// <summary>Solo al editar: nombre del comprobante que ya está guardado.</summary>
    public string? ComprobanteActual { get; set; }

    public IEnumerable<SelectListItem> Categorias { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Terrenos { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Periodos { get; set; } = Enumerable.Empty<SelectListItem>();
}
