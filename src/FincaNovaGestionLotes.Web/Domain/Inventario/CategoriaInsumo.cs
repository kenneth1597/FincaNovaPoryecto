using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Inventario;

public class CategoriaInsumo : AuditableEntity
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Insumos químicos: la fecha de vencimiento es obligatoria al ingresar (HU-22 esc. 2).</summary>
    public bool RequiereVencimiento { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();
}
