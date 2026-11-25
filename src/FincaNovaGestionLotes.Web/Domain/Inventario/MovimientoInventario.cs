using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Finanzas;
using FincaNovaGestionLotes.Web.Domain.Labores;

namespace FincaNovaGestionLotes.Web.Domain.Inventario;

/// <summary>
/// Entrada, salida o ajuste del inventario. Las salidas automáticas (HU-24)
/// guardan el tratamiento o la labor que las originó.
/// </summary>
public class MovimientoInventario : AuditableEntity
{
    public int InsumoId { get; set; }
    public Insumo Insumo { get; set; } = null!;

    public TipoMovimientoInventario Tipo { get; set; }

    [Range(0.01, 100000000)]
    public decimal Cantidad { get; set; }

    /// <summary>Existencia del insumo inmediatamente después del movimiento.</summary>
    public decimal StockResultante { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>Obligatoria en entradas de categorías que requieren vencimiento.</summary>
    public DateTime? FechaVencimiento { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }

    public int? ProductoTratamientoId { get; set; }
    public ProductoTratamiento? ProductoTratamiento { get; set; }

    public int? LaborInsumoId { get; set; }
    public LaborInsumo? LaborInsumo { get; set; }

    /// <summary>Gasto de compra asociado a una entrada (HU-19).</summary>
    public int? GastoId { get; set; }
    public Gasto? Gasto { get; set; }
}
