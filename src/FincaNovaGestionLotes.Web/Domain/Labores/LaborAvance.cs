using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;
using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Labores;

public class LaborAvance : AuditableEntity
{
    public int LaborId { get; set; }
    public Labor Labor { get; set; } = null!;

    public int ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public DateTime Fecha { get; set; }

    [Range(0.25, 24)]
    public decimal Horas { get; set; }

    [Required, StringLength(500)]
    public string Comentario { get; set; } = string.Empty;

    [StringLength(150)]
    public string? RegistradoPor { get; set; }
}
