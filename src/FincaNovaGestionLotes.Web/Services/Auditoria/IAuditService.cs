using FincaNovaGestionLotes.Web.Domain;

namespace FincaNovaGestionLotes.Web.Services.Auditoria;

public interface IAuditService
{
    Task RegistrarAsync(AccionAuditoria accion, string entidad, string entidadId, string descripcion);
}
