using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Inventario;

namespace FincaNovaGestionLotes.Web.Domain.Labores;

/// <summary>Insumo consumido en una labor; se rebaja del inventario (HU-24 esc. 2).</summary>
public class LaborInsumo : AuditableEntity
{
    public int LaborId { get; set; }
    public Labor Labor { get; set; } = null!;

    public int InsumoId { get; set; }
    public Insumo Insumo { get; set; } = null!;

    [Range(0.01, 100000000)]
    public decimal Cantidad { get; set; }
}
