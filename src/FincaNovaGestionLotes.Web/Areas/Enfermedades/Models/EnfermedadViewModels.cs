using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Areas.Enfermedades.Models;

public class EnfermedadCrearViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Lote")]
    public int LoteId { get; set; }

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

    public IEnumerable<LoteOpcionViewModel> Lotes { get; set; }
        = Enumerable.Empty<LoteOpcionViewModel>();
}

public class LoteOpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
}

public class EnfermedadFiltroViewModel
{
    public int? LoteId { get; set; }

    public string? Nombre { get; set; }

    public string? Severidad { get; set; }

    [Display(Name = "Desde")]
    public DateTime? Desde { get; set; }

    [Display(Name = "Hasta")]
    public DateTime? Hasta { get; set; }

    public IReadOnlyList<EnfermedadListaViewModel> Resultados { get; set; }
        = Array.Empty<EnfermedadListaViewModel>();

    public IEnumerable<LoteOpcionViewModel> Lotes { get; set; }
        = Enumerable.Empty<LoteOpcionViewModel>();
}

public class EnfermedadListaViewModel
{
    public int Id { get; set; }

    public int LoteId { get; set; }

    public string LoteCodigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public DateTime FechaDeteccion { get; set; }

    public string Severidad { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public bool Atendida { get; set; }

    public bool EsRecurrencia { get; set; }
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