using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Pagos;

/// <summary>Pago a un trabajador por jornadas y/o cajuelas de un período (HU-06).</summary>
public class PagoColaborador : AuditableEntity
{
    public int ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public DateTime FechaPago { get; set; }

    public decimal TotalJornadas { get; set; }

    public decimal TotalCajuelas { get; set; }

    public decimal MontoTotal { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }

    public ICollection<PagoDetalle> Detalles { get; set; } = new List<PagoDetalle>();
}
