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

    /// <summary>Insumos del inventario que se pueden aplicar (HU-24).</summary>
    public IEnumerable<InsumoTratamientoOpcion> Insumos { get; set; }
        = Enumerable.Empty<InsumoTratamientoOpcion>();
}

public class ProductoViewModel
{
    /// <summary>
    /// Insumo del inventario. Si se elige, el nombre y la unidad salen del insumo
    /// y la cantidad se rebaja del stock (HU-24). Si no, se escriben a mano.
    /// </summary>
    [Display(Name = "Insumo del inventario")]
    public int? InsumoId { get; set; }

    [StringLength(150)]
    [Display(Name = "Producto")]
    public string? NombreProducto { get; set; }

    [Range(0.01, 100000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public decimal Cantidad { get; set; }

    [StringLength(30)]
    public string? Unidad { get; set; }

    public bool EstaVacio => InsumoId is null && string.IsNullOrWhiteSpace(NombreProducto) && Cantidad == 0;
}

public class InsumoTratamientoOpcion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
}

public class EnfermedadEditarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un lote.")]
    [Display(Name = "Lote")]
    public int LoteId { get; set; }

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

    public IEnumerable<LoteOpcionViewModel> Lotes { get; set; }
        = Enumerable.Empty<LoteOpcionViewModel>();
}