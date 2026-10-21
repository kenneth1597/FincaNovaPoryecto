using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Enfermedades.Models;

public class EnfermedadCrearViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Finca o lote")]
    public int TerrenoId { get; set; }

    [Required(ErrorMessage = "El nombre de la enfermedad es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Enfermedad")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de detección es obligatoria.")]
    [Display(Name = "Fecha de detección")]
    public DateTime FechaDeteccion { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Debe indicar la severidad.")]
    [Display(Name = "Severidad")]
    public string Severidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

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

public class EnfermedadFiltroViewModel
{
    public int? TerrenoId { get; set; }

    public string? Nombre { get; set; }

    public string? Severidad { get; set; }

    public string? Estado { get; set; }

    [Display(Name = "Desde")]
    public DateTime? Desde { get; set; }

    [Display(Name = "Hasta")]
    public DateTime? Hasta { get; set; }

    public int Pagina { get; set; } = 1;

    public PaginaInfo Paginacion { get; set; } = new();

    public IReadOnlyList<EnfermedadListaViewModel> Resultados { get; set; }
        = Array.Empty<EnfermedadListaViewModel>();

    public IEnumerable<TerrenoOpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<TerrenoOpcionViewModel>();
}

public class EnfermedadListaViewModel
{
    public int Id { get; set; }

    public int TerrenoId { get; set; }

    public string TerrenoCodigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public DateTime FechaDeteccion { get; set; }

    public string Severidad { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public bool Atendida { get; set; }

    public bool EsRecurrencia { get; set; }

    public EstadoEnfermedad Estado { get; set; }
}

public class CierreEnfermedadViewModel
{
    [Required(ErrorMessage = "Seleccione el resultado final.")]
    [Display(Name = "Resultado final")]
    public ResultadoCierreEnfermedad? ResultadoCierre { get; set; }

    [Display(Name = "Tratamiento que resolvió el caso")]
    public int? TratamientoEfectivoId { get; set; }

    [Required(ErrorMessage = "El comentario de cierre es obligatorio.")]
    [StringLength(500)]
    [Display(Name = "Comentario de cierre")]
    public string? ComentarioCierre { get; set; }

    [Display(Name = "Verifiqué en campo que la enfermedad está controlada")]
    public bool Confirmo { get; set; }
}

public class TratamientoCrearViewModel
{
    public int EnfermedadId { get; set; }

    [Required(ErrorMessage = "El tratamiento es obligatorio.")]
    [StringLength(250)]
    [Display(Name = "Tratamiento aplicado")]
    public string Tratamiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de aplicación es obligatoria.")]
    [Display(Name = "Fecha de aplicación")]
    public DateTime FechaAplicacion { get; set; } = DateTime.Today;

    [StringLength(150)]
    [Display(Name = "Responsable")]
    public string? Responsable { get; set; }

    [StringLength(500)]
    [Display(Name = "Resultado")]
    public string? Resultado { get; set; }

    [Required(ErrorMessage = "Indique la efectividad del tratamiento.")]
    [Display(Name = "Efectividad")]
    public EfectividadTratamiento? Efectividad { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public List<ProductoViewModel> Productos { get; set; }
        = new();
}

public class ProductoViewModel
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Producto")]
    public string NombreProducto { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public decimal Cantidad { get; set; }

    [Required(ErrorMessage = "La unidad es obligatoria.")]
    [StringLength(30)]
    public string Unidad { get; set; } = string.Empty;
}

public class EnfermedadEditarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Finca o lote")]
    public int TerrenoId { get; set; }

    [Required(ErrorMessage = "El nombre de la enfermedad es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Enfermedad")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de detección es obligatoria.")]
    [Display(Name = "Fecha de detección")]
    public DateTime FechaDeteccion { get; set; }

    [Required(ErrorMessage = "Debe indicar la severidad.")]
    [Display(Name = "Severidad")]
    public string Severidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<TerrenoOpcionViewModel> Terrenos { get; set; }
        = Enumerable.Empty<TerrenoOpcionViewModel>();
}