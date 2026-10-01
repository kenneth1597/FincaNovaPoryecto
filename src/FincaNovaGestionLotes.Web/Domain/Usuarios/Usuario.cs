using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Usuarios;

public class Usuario : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Trabajador";
    public bool Activo { get; set; } = true;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiration { get; set; }
}