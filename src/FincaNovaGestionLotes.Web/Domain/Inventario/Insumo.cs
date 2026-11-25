using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Inventario;

public class Insumo : AuditableEntity
{
    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    public int CategoriaInsumoId { get; set; }
    public CategoriaInsumo CategoriaInsumo { get; set; } = null!;

    public int UnidadMedidaId { get; set; }
    public UnidadMedida UnidadMedida { get; set; } = null!;

    /// <summary>Existencia actual; se actualiza con cada movimiento de inventario.</summary>
    [Range(0, 100000000)]
    public decimal StockActual { get; set; }

    /// <summary>Umbral para la alerta de stock mínimo (HU-23 esc. 1, HU-25).</summary>
    [Range(0, 100000000)]
    public decimal StockMinimo { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();

    public ICollection<AlertaStock> Alertas { get; set; } = new List<AlertaStock>();
}
