using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Auditoria;

namespace FincaNovaGestionLotes.Web.Services.Auditoria;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RegistrarAsync(AccionAuditoria accion, string entidad, string entidadId, string descripcion)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            FechaHora = DateTime.UtcNow,
            Usuario = _httpContextAccessor.HttpContext?.User?.Identity?.Name,
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            Descripcion = descripcion
        });
        await _db.SaveChangesAsync();
    }
}
