namespace FincaNovaGestionLotes.Web.Domain.Common;

public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string? ModificadoPor { get; set; }
}
