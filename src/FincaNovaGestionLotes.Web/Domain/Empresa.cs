using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain;


public class Empresa : AuditableEntity
{
    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Ubicacion { get; set; }

    [StringLength(150)]
    public string? Propietario { get; set; }

    [StringLength(30)]
    public string? Telefono { get; set; }

    public ICollection<Terreno> Terrenos { get; set; } = new List<Terreno>();
}
