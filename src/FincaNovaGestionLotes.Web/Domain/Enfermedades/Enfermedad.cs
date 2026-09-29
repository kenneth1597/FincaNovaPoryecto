using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;
using FincaNovaGestionLotes.Web.Domain.Lotes;

namespace FincaNovaGestionLotes.Web.Domain.Enfermedades;

public class Enfermedad : AuditableEntity
{
    [Required]
    public int LoteId { get; set; }

    public Lote Lote { get; set; } = null!;

    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public DateTime FechaDeteccion { get; set; }

    [Required, StringLength(30)]
    public string Severidad { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public bool Atendida { get; set; }

    public DateTime? FechaAtendida { get; set; }

    public string? AtendidaPor { get; set; }

    public ICollection<TratamientoEnfermedad> Tratamientos { get; set; }
        = new List<TratamientoEnfermedad>();
}