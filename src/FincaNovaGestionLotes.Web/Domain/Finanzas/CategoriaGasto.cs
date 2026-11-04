using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Finanzas;

public class CategoriaGasto : AuditableEntity
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
