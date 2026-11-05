using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Labores;

public class Labor : AuditableEntity
{
    public int TerrenoId { get; set; }
    public Terreno Terreno { get; set; } = null!;

    public TipoLabor Tipo { get; set; }

    [StringLength(100)]
    public string? TipoOtro { get; set; }

    public DateTime Fecha { get; set; }

    [Required, StringLength(500)]
    public string Observaciones { get; set; } = string.Empty;

    [StringLength(150)]
    public string? RegistradoPor { get; set; }

    public EstadoLabor Estado { get; set; } = EstadoLabor.Pendiente;

    public decimal HorasTotales { get; set; }

    public decimal CostoTotal { get; set; }

    /// <summary>Cuándo todos los asignados terminaron su parte y la labor pasó a revisión.</summary>
    public DateTime? FechaEnvioRevision { get; set; }

    /// <summary>Último rechazo del revisor: la labor volvió a «En proceso».</summary>
    public DateTime? FechaDevolucion { get; set; }

    [StringLength(150)]
    public string? DevueltaPor { get; set; }

    [StringLength(500)]
    public string? MotivoDevolucion { get; set; }

    /// <summary>Aprobación final del administrador o dueño.</summary>
    public DateTime? FechaCierre { get; set; }

    [StringLength(150)]
    public string? CerradaPor { get; set; }

    [StringLength(500)]
    public string? ComentarioCierre { get; set; }

    public ICollection<LaborColaborador> Colaboradores { get; set; } = new List<LaborColaborador>();

    public ICollection<LaborAvance> Avances { get; set; } = new List<LaborAvance>();

    public ICollection<LaborInsumo> Insumos { get; set; } = new List<LaborInsumo>();
}
