using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Terrenos.Models;


public class TerrenoListItemViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TipoTerreno Tipo { get; set; }
    public string? FincaPadre { get; set; }
    public decimal AreaHectareas { get; set; }
    public decimal AreaAsignada { get; set; }
    public string? VariedadCafe { get; set; }
    public int? AnioSiembra { get; set; }
    public string? Ubicacion { get; set; }
    public EstadoTerreno Estado { get; set; }
    public int Lotes { get; set; }
}

/// <summary>Filtros y búsqueda del listado (HU-32).</summary>
public class TerrenoFiltroViewModel
{
    public string? Q { get; set; }
    public string? Ubicacion { get; set; }
    public TipoTerreno? Tipo { get; set; }
    public EstadoTerreno? Estado { get; set; }
    public int Pagina { get; set; } = 1;
    public IReadOnlyList<TerrenoListItemViewModel> Resultados { get; set; } = Array.Empty<TerrenoListItemViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();
    public Dictionary<EstadoTerreno, int> Conteos { get; set; } = new();
}

/// <summary>Formulario de alta y edición de finca / lote (HU-30, HU-33).</summary>
public class TerrenoFormViewModel
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
    public TipoTerreno Tipo { get; set; } = TipoTerreno.Finca;

    [Display(Name = "Finca a la que pertenece")]
    public int? FincaPadreId { get; set; }

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

    public IEnumerable<FincaOpcionViewModel> FincasPadre { get; set; } = Enumerable.Empty<FincaOpcionViewModel>();
}

public class FincaOpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public decimal AreaDisponible { get; set; }
    public string SiguienteCodigoLote { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
}


public class TerrenoDetalleViewModel
{
    public Domain.Terrenos.Terreno Terreno { get; set; } = null!;
    public IReadOnlyList<TerrenoListItemViewModel> Lotes { get; set; } = Array.Empty<TerrenoListItemViewModel>();
    public decimal AreaAsignada { get; set; }
    public decimal AreaDisponible => Math.Max(0, Terreno.AreaHectareas - AreaAsignada);
    public int PorcentajeAsignado => Terreno.AreaHectareas <= 0
        ? 0
        : (int)Math.Min(100, Math.Round(AreaAsignada * 100 / Terreno.AreaHectareas));
    public IReadOnlyList<LaborBitacoraViewModel> Labores { get; set; } = Array.Empty<LaborBitacoraViewModel>();
    public IReadOnlyList<IndicadorMantenimientoViewModel> Indicadores { get; set; } = Array.Empty<IndicadorMantenimientoViewModel>();
    public decimal HorasPorJornada { get; set; }
    public bool IncluyeLotes { get; set; }
}

public class IndicadorMantenimientoViewModel
{
    public TipoLabor Tipo { get; set; }
    public string? TipoOtro { get; set; }
    public string NombreLabor => Ui.NombreLabor(Tipo, TipoOtro);
    public int Labores { get; set; }
    public decimal Horas { get; set; }
    public decimal Jornadas { get; set; }
    public decimal Costo { get; set; }
}

public class LaborBitacoraViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoLabor Tipo { get; set; }
    public string? TipoOtro { get; set; }
    public string NombreLabor => Ui.NombreLabor(Tipo, TipoOtro);
    public decimal HorasTotales { get; set; }
    public EstadoLabor Estado { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public List<string> Colaboradores { get; set; } = new();
}
