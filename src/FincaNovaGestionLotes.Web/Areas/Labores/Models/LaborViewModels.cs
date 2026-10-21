using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Labores.Models;

public class OpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
}

public class LaborFiltroViewModel
{
    public int? TerrenoId { get; set; }

    public TipoLabor? Tipo { get; set; }

    public EstadoLabor? Estado { get; set; }

    public DateTime? Desde { get; set; }

    public DateTime? Hasta { get; set; }

    public bool SoloMias { get; set; }

    public bool TieneColaborador { get; set; }

    public int Pagina { get; set; } = 1;

    public PaginaInfo Paginacion { get; set; } = new();

    public IReadOnlyList<LaborListItemViewModel> Resultados { get; set; }
        = Array.Empty<LaborListItemViewModel>();

    public IEnumerable<OpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<OpcionViewModel>();
}

public class LaborListItemViewModel
{
    public int Id { get; set; }
    public int TerrenoId { get; set; }
    public string TerrenoCodigo { get; set; } = string.Empty;
    public string TerrenoNombre { get; set; } = string.Empty;
    public TipoLabor Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoLabor Estado { get; set; }
    public decimal HorasTotales { get; set; }
    public string? RegistradoPor { get; set; }
    public List<string> Colaboradores { get; set; } = new();
    public bool EsMia { get; set; }
}

public class LaborFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el lote.")]
    [Display(Name = "Finca o lote")]
    public int? TerrenoId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el tipo de labor.")]
    [Display(Name = "Tipo de labor")]
    public TipoLabor? Tipo { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de la labor")]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Las observaciones son obligatorias.")]
    [StringLength(500)]
    [Display(Name = "Observaciones / instrucciones")]
    public string? Observaciones { get; set; }

    [Display(Name = "Colaboradores asignados")]
    public List<int> ColaboradoresIds { get; set; } = new();

    public bool EsEdicion => Id != 0;

    public bool PuedeElegirColaboradores { get; set; }

    public string? ColaboradorPropio { get; set; }

    public IEnumerable<OpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<OpcionViewModel>();

    public IEnumerable<OpcionViewModel> ColaboradoresDisponibles { get; set; }
        = Enumerable.Empty<OpcionViewModel>();
}

public class AvanceFormViewModel
{
    [Display(Name = "Colaborador")]
    public int? ColaboradorId { get; set; }

    [Required(ErrorMessage = "La fecha del avance es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Las horas trabajadas son obligatorias.")]
    [Range(0.25, 24, ErrorMessage = "Las horas deben ser mayores a cero y como máximo 24.")]
    [Display(Name = "Horas trabajadas")]
    public decimal? Horas { get; set; }

    [Required(ErrorMessage = "El comentario es obligatorio.")]
    [StringLength(500)]
    [Display(Name = "Comentario")]
    public string? Comentario { get; set; }
}

public class CierreFormViewModel
{
    [Required(ErrorMessage = "El comentario de cierre es obligatorio.")]
    [StringLength(500)]
    [Display(Name = "Comentario de cierre")]
    public string? ComentarioCierre { get; set; }

    [Display(Name = "Verifiqué que todo el trabajo asignado se realizó")]
    public bool Confirmo { get; set; }
}

public class ColaboradorLaborViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Horas { get; set; }
}

public class AvanceItemViewModel
{
    public DateTime Fecha { get; set; }
    public string Colaborador { get; set; } = string.Empty;
    public decimal Horas { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public string? RegistradoPor { get; set; }
}

public class LaborDetalleViewModel
{
    public int Id { get; set; }
    public int TerrenoId { get; set; }
    public string TerrenoCodigo { get; set; } = string.Empty;
    public string TerrenoNombre { get; set; } = string.Empty;
    public TipoLabor Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public string? RegistradoPor { get; set; }
    public EstadoLabor Estado { get; set; }
    public decimal HorasTotales { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? CerradaPor { get; set; }
    public string? ComentarioCierre { get; set; }

    public List<ColaboradorLaborViewModel> Colaboradores { get; set; } = new();
    public List<AvanceItemViewModel> Avances { get; set; } = new();

    public bool EsAdmin { get; set; }
    public bool PuedeActualizar { get; set; }
    public int? MiColaboradorId { get; set; }

    public AvanceFormViewModel NuevoAvance { get; set; } = new();
    public CierreFormViewModel Cierre { get; set; } = new();
}
