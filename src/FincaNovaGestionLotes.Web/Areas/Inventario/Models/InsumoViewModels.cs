using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Inventario.Models;

public class InsumoFiltroViewModel
{
    public string? Q { get; set; }
    public int? CategoriaId { get; set; }
    public string Estado { get; set; } = "Activos";
    public bool SoloBajoMinimo { get; set; }
    public int Pagina { get; set; } = 1;

    public IReadOnlyList<InsumoListItemViewModel> Resultados { get; set; } = Array.Empty<InsumoListItemViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();
    public IEnumerable<OpcionViewModel> Categorias { get; set; } = Enumerable.Empty<OpcionViewModel>();
    public int AlertasActivas { get; set; }
    public bool EsAdmin { get; set; }
}

public class InsumoListItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public bool Activo { get; set; }
    public DateTime? ProximoVencimiento { get; set; }

    /// <summary>HU-23 esc. 1: la fila se resalta en rojo.</summary>
    public bool BajoMinimo => Activo && StockMinimo > 0 && StockActual <= StockMinimo;
}

public class IngresoFormViewModel
{
    /// <summary>true = se registra un insumo nuevo en el mismo formulario.</summary>
    public bool EsNuevo { get; set; }

    [Display(Name = "Insumo")]
    public int? InsumoId { get; set; }

    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    [Display(Name = "Nombre del insumo")]
    public string? Nombre { get; set; }

    [Display(Name = "Tipo (categoría)")]
    public int? CategoriaInsumoId { get; set; }

    [Display(Name = "Unidad de medida")]
    public int? UnidadMedidaId { get; set; }

    [Range(0, 100000000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [Display(Name = "Stock mínimo")]
    public decimal? StockMinimo { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0.01, 100000000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    [Display(Name = "Cantidad que ingresa")]
    public decimal? Cantidad { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
    [Display(Name = "Fecha de ingreso")]
    public DateTime? Fecha { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de vencimiento")]
    public DateTime? FechaVencimiento { get; set; }

    [StringLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<InsumoOpcionViewModel> Insumos { get; set; } = Enumerable.Empty<InsumoOpcionViewModel>();
    public IEnumerable<CategoriaOpcionViewModel> Categorias { get; set; } = Enumerable.Empty<CategoriaOpcionViewModel>();
    public IEnumerable<OpcionViewModel> Unidades { get; set; } = Enumerable.Empty<OpcionViewModel>();
}

public class InsumoEditarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del insumo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    [Display(Name = "Nombre del insumo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione el tipo de insumo.")]
    [Display(Name = "Tipo (categoría)")]
    public int? CategoriaInsumoId { get; set; }

    [Required(ErrorMessage = "Seleccione la unidad de medida.")]
    [Display(Name = "Unidad de medida")]
    public int? UnidadMedidaId { get; set; }

    [Required(ErrorMessage = "El stock mínimo es obligatorio (0 = sin alerta).")]
    [Range(0, 100000000, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    [Display(Name = "Stock mínimo")]
    public decimal? StockMinimo { get; set; }

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Insumo activo")]
    public bool Activo { get; set; } = true;

    public decimal StockActual { get; set; }

    /// <summary>Con movimientos ya no se cambia la unidad: alteraría el sentido del histórico.</summary>
    public bool TieneMovimientos { get; set; }

    public IEnumerable<CategoriaOpcionViewModel> Categorias { get; set; } = Enumerable.Empty<CategoriaOpcionViewModel>();
    public IEnumerable<OpcionViewModel> Unidades { get; set; } = Enumerable.Empty<OpcionViewModel>();
}

public class InsumoDetalleViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool RequiereVencimiento { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public bool EsAdmin { get; set; }

    public bool BajoMinimo => Activo && StockMinimo > 0 && StockActual <= StockMinimo;

    public IReadOnlyList<MovimientoItemViewModel> Movimientos { get; set; } = Array.Empty<MovimientoItemViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();
}

public class MovimientoItemViewModel
{
    public DateTime Fecha { get; set; }
    public TipoMovimientoInventario Tipo { get; set; }
    public decimal Cantidad { get; set; }
    public decimal StockResultante { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? Origen { get; set; }
    public string? Observaciones { get; set; }
    public string? RegistradoPor { get; set; }
}

public class AlertasStockViewModel
{
    public IReadOnlyList<AlertaItemViewModel> Activas { get; set; } = Array.Empty<AlertaItemViewModel>();
    public IReadOnlyList<AlertaItemViewModel> Resueltas { get; set; } = Array.Empty<AlertaItemViewModel>();
}

public class AlertaItemViewModel
{
    public int InsumoId { get; set; }
    public string Insumo { get; set; } = string.Empty;
    public string Unidad { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public DateTime FechaAlerta { get; set; }
    public DateTime? FechaResolucion { get; set; }
}

public class OpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
}

public class CategoriaOpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public bool RequiereVencimiento { get; set; }
}

public class InsumoOpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public bool RequiereVencimiento { get; set; }
}
