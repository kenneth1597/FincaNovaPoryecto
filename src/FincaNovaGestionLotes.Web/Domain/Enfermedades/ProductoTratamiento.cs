using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Enfermedades;

public class ProductoTratamiento : AuditableEntity
{
    [Required]
    public int TratamientoEnfermedadId { get; set; }

    public TratamientoEnfermedad TratamientoEnfermedad { get; set; } = null!;

    [Required, StringLength(150)]
    public string NombreProducto { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Cantidad { get; set; }

    [Required, StringLength(30)]
    public string Unidad { get; set; } = string.Empty;
}