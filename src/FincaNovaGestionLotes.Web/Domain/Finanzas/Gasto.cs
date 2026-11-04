using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Produccion;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Finanzas;

/// <summary>Egreso de la finca con comprobante adjunto opcional (HU-19, HU-52).</summary>
public class Gasto : AuditableEntity
{
    public DateTime Fecha { get; set; }

    [Range(0.01, 1000000000)]
    public decimal Monto { get; set; }

    public int CategoriaGastoId { get; set; }
    public CategoriaGasto CategoriaGasto { get; set; } = null!;

    /// <summary>Lote o finca asociado; vacío si es un gasto general.</summary>
    public int? TerrenoId { get; set; }
    public Terreno? Terreno { get; set; }

    public int? PeriodoProductivoId { get; set; }
    public PeriodoProductivo? PeriodoProductivo { get; set; }

    [Required, StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Proveedor { get; set; }

    [StringLength(255)]
    public string? AdjuntoNombreOriginal { get; set; }

    /// <summary>Ruta relativa donde se guardó el archivo en el servidor.</summary>
    [StringLength(500)]
    public string? AdjuntoRuta { get; set; }

    [StringLength(100)]
    public string? AdjuntoTipoContenido { get; set; }

    public long? AdjuntoTamanoBytes { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }
}
