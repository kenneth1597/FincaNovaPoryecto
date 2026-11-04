using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

/// <summary>Recolección diaria en cajuelas por trabajador y lote (HU-11, HU-12, HU-13).</summary>
public class Recoleccion : AuditableEntity
{
    public int ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public int TerrenoId { get; set; }
    public Terreno Terreno { get; set; } = null!;

    public int PeriodoProductivoId { get; set; }
    public PeriodoProductivo PeriodoProductivo { get; set; } = null!;

    /// <summary>Garantiza que el lote esté asignado al período.</summary>
    public PeriodoTerreno PeriodoTerreno { get; set; } = null!;

    public DateTime Fecha { get; set; }

    [Range(0.01, 100000)]
    public decimal Cajuelas { get; set; }

    /// <summary>Conversión vigente al registrar; se guarda para no alterar el histórico.</summary>
    public decimal KgPorCajuela { get; set; }

    /// <summary>Columna calculada en la base: Cajuelas * KgPorCajuela (HU-13).</summary>
    public decimal PesoEstimadoKg { get; private set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(150)]
    public string? RegistradoPor { get; set; }
}
