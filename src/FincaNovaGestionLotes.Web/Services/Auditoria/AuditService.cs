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

    public async Task RegistrarAsync(AccionAuditoria accion, string entidad, string entidadId, string descripcion,
        string? valorAnterior = null, string? valorNuevo = null)
    {
        var http = _httpContextAccessor.HttpContext;
        _db.AuditLogs.Add(new AuditLog
        {
            FechaHora = DateTime.UtcNow,
            UsuarioId = http?.Session.GetInt32("UsuarioId"),
            Usuario = http?.Session.GetString("UsuarioNombre")
                      ?? http?.User?.Identity?.Name,
            DireccionIp = http?.Connection.RemoteIpAddress?.ToString(),
            Modulo = http?.GetRouteValue("area")?.ToString() ?? http?.GetRouteValue("controller")?.ToString(),
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            Descripcion = descripcion,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo
        });
        await _db.SaveChangesAsync();
    }
}
