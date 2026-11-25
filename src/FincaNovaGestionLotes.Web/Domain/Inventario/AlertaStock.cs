using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Inventario;

/// <summary>
/// Alerta de stock mínimo (HU-25). Se mantiene activa hasta que el insumo se
/// reabastece; solo puede haber una alerta activa por insumo.
/// </summary>
public class AlertaStock : AuditableEntity
{
    public int InsumoId { get; set; }
    public Insumo Insumo { get; set; } = null!;

    public DateTime FechaAlerta { get; set; }

    public decimal StockAlMomento { get; set; }

    public decimal StockMinimo { get; set; }

    public bool Activa { get; set; } = true;

    public DateTime? FechaResolucion { get; set; }
}
