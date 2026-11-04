namespace FincaNovaGestionLotes.Web.Domain.Auditoria;

/// <summary>
/// Bitácora de acciones críticas (HU-18). La tabla tiene un trigger que impide
/// modificar o borrar filas.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public int? UsuarioId { get; set; }
    public string? Usuario { get; set; }
    public string? DireccionIp { get; set; }
    public string? Modulo { get; set; }
    public AccionAuditoria Accion { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public string? EntidadId { get; set; }
    public string? Descripcion { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
}
