using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Terrenos;


public class Terreno : AuditableEntity
{
    public int EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;


    [Required, StringLength(30)]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    public TipoTerreno Tipo { get; set; } = TipoTerreno.Finca;

    public int? FincaPadreId { get; set; }
    public Terreno? FincaPadre { get; set; }
    public ICollection<Terreno> Lotes { get; set; } = new List<Terreno>();

    [Range(0, 100000)]
    public decimal AreaHectareas { get; set; }

    [StringLength(100)]
    public string? VariedadCafe { get; set; }

    [Range(1900, 2100)]
    public int? AnioSiembra { get; set; }

    [StringLength(200)]
    public string? Ubicacion { get; set; }

    public EstadoTerreno Estado { get; set; } = EstadoTerreno.Activo;

    public DateTime? FechaUltimoCambioEstado { get; set; }

    /// <summary>Usuario que realizó el último cambio de estado (HU-34 esc. 3).</summary>
    [StringLength(256)]
    public string? EstadoCambiadoPor { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>Borrado lógico: un lote con historial nunca se elimina físicamente .</summary>
    public bool Eliminado { get; set; }
}
