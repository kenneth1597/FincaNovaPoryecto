namespace FincaNovaGestionLotes.Web.Domain.Auditoria;


public class AuditLog
{
    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Usuario { get; set; }
    public AccionAuditoria Accion { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public string? EntidadId { get; set; }
    public string? Descripcion { get; set; }
}
