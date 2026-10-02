using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Enfermedades;

public class TratamientoEnfermedad : AuditableEntity
{
    [Required]
    public int EnfermedadId { get; set; }

    public Enfermedad Enfermedad { get; set; } = null!;

    [Required, StringLength(250)]
    public string Tratamiento { get; set; } = string.Empty;

    [Required]
    public DateTime FechaAplicacion { get; set; }

    [StringLength(150)]
    public string? Responsable { get; set; }

    [StringLength(500)]
    public string? Resultado { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public ICollection<ProductoTratamiento> Productos { get; set; }
        = new List<ProductoTratamiento>();
}