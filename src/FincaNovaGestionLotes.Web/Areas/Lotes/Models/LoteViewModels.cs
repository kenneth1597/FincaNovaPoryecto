using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Lotes.Models;


public class LoteListItemViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TipoLote Tipo { get; set; }
    public string? LotePadre { get; set; }
    public decimal AreaHectareas { get; set; }
    public string? VariedadCafe { get; set; }
    public int? AnioSiembra { get; set; }
    public EstadoLote Estado { get; set; }
    public int MicroLotes { get; set; }
}

/// <summary>Filtros y búsqueda del listado (HU-32).</summary>
public class LoteFiltroViewModel
{
    public string? Q { get; set; }
    public TipoLote? Tipo { get; set; }
    public EstadoLote? Estado { get; set; }
    public IReadOnlyList<LoteListItemViewModel> Resultados { get; set; } = Array.Empty<LoteListItemViewModel>();
    public Dictionary<EstadoLote, int> Conteos { get; set; } = new();
}

/// <summary>Formulario de alta y edición de lote / micro lote (HU-30, HU-33).</summary>
public class LoteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(30)]
    [RegularExpression(Validaciones.Codigo, ErrorMessage = Validaciones.CodigoMsg)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Tipo")]
    public TipoLote Tipo { get; set; } = TipoLote.Lote;

    [Display(Name = "Lote al que pertenece")]
    public int? LotePadreId { get; set; }

    [Range(0.01, 100000, ErrorMessage = "El área debe ser mayor a cero.")]
    [Display(Name = "Área (hectáreas)")]
    public decimal AreaHectareas { get; set; }

    [Required(ErrorMessage = "La variedad de café es obligatoria.")]
    [StringLength(100)]
    [RegularExpression(Validaciones.Texto, ErrorMessage = Validaciones.TextoMsg)]
    [Display(Name = "Variedad de café")]
    public string? VariedadCafe { get; set; }

    [Required(ErrorMessage = "El año de siembra es obligatorio.")]
    [Range(1900, 2100, ErrorMessage = "El año de siembra no es válido.")]
    [Display(Name = "Año de siembra")]
    public int? AnioSiembra { get; set; }

    [Required(ErrorMessage = "La ubicación es obligatoria.")]
    [StringLength(200)]
    [Display(Name = "Ubicación / referencia")]
    public string? Ubicacion { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public bool EsEdicion => Id != 0;

    public IEnumerable<(int Id, string Texto)> LotesPadre { get; set; } = Enumerable.Empty<(int, string)>();
}


public class LoteDetalleViewModel
{
    public Domain.Lotes.Lote Lote { get; set; } = null!;
    public IReadOnlyList<LoteListItemViewModel> MicroLotes { get; set; } = Array.Empty<LoteListItemViewModel>();
}
