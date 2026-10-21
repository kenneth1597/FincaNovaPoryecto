using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Labores;
using FincaNovaGestionLotes.Web.Domain.Usuarios;

namespace FincaNovaGestionLotes.Web.Domain.Colaboradores;

public class Colaborador : AuditableEntity
{
    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Identificacion { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string Telefono { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Cargo { get; set; } = string.Empty;

    [Range(0, 10000000)]
    public decimal TarifaJornada { get; set; }

    public bool Activo { get; set; } = true;

    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<LaborColaborador> Labores { get; set; } = new List<LaborColaborador>();
}
