using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Lotes;


public class Lote : AuditableEntity
{
    public int FincaId { get; set; }
    public Finca Finca { get; set; } = null!;


    [Required, StringLength(30)]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    public TipoLote Tipo { get; set; } = TipoLote.Lote;

    public int? LotePadreId { get; set; }
    public Lote? LotePadre { get; set; }
    public ICollection<Lote> MicroLotes { get; set; } = new List<Lote>();

    [Range(0, 100000)]
    public decimal AreaHectareas { get; set; }

    [StringLength(100)]
    public string? VariedadCafe { get; set; }

    [Range(1900, 2100)]
    public int? AnioSiembra { get; set; }

    [StringLength(200)]
    public string? Ubicacion { get; set; }

    public EstadoLote Estado { get; set; } = EstadoLote.Activo;

    public DateTime? FechaUltimoCambioEstado { get; set; }

    /// <summary>Usuario que realizó el último cambio de estado (HU-34 esc. 3).</summary>
    [StringLength(256)]
    public string? EstadoCambiadoPor { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>Borrado lógico: un lote con historial nunca se elimina físicamente .</summary>
    public bool Eliminado { get; set; }
}
