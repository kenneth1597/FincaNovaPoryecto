using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Inventario;

public class UnidadMedida : AuditableEntity
{
    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Abreviatura { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
