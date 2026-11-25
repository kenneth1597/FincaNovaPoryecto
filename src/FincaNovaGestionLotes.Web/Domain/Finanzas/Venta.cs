using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Produccion;

namespace FincaNovaGestionLotes.Web.Domain.Finanzas;

/// <summary>Venta de café (HU-20).</summary>
public class Venta : AuditableEntity
{
    public DateTime Fecha { get; set; }

    [Required, StringLength(150)]
    public string Comprador { get; set; } = string.Empty;

    [Range(0.01, 100000000)]
    public decimal Cantidad { get; set; }

    public UnidadVentaCafe Unidad { get; set; } = UnidadVentaCafe.Cajuela;

    [Range(0.01, 1000000000)]
    public decimal PrecioUnitario { get; set; }

    /// <summary>Columna calculada en la base: Cantidad * PrecioUnitario.</summary>
    public decimal Total { get; private set; }

    public int? PeriodoProductivoId { get; set; }
    public PeriodoProductivo? PeriodoProductivo { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }
}
