using FincaNovaGestionLotes.Web.Domain.Labores;
using FincaNovaGestionLotes.Web.Domain.Produccion;

namespace FincaNovaGestionLotes.Web.Domain.Pagos;

/// <summary>
/// Línea del cálculo de un pago. Cada avance de labor o recolección solo puede
/// pagarse una vez.
/// </summary>
public class PagoDetalle
{
    public int Id { get; set; }

    public int PagoColaboradorId { get; set; }
    public PagoColaborador PagoColaborador { get; set; } = null!;

    public TipoDetallePago Tipo { get; set; }

    public int? LaborAvanceId { get; set; }
    public LaborAvance? LaborAvance { get; set; }

    public int? RecoleccionId { get; set; }
    public Recoleccion? Recoleccion { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>Jornadas o cajuelas, según el tipo.</summary>
    public decimal Cantidad { get; set; }

    public decimal Tarifa { get; set; }

    /// <summary>Columna calculada en la base: Cantidad * Tarifa.</summary>
    public decimal Subtotal { get; private set; }
}
