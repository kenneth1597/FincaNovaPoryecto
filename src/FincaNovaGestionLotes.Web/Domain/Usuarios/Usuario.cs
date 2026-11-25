using FincaNovaGestionLotes.Web.Domain.Common;

namespace FincaNovaGestionLotes.Web.Domain.Usuarios;

public class Usuario : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Trabajador";
    public bool Activo { get; set; } = true;
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
    public int IntentosFallidos { get; set; } = 0;
    public bool Bloqueado { get; set; } = false;

    /// <summary>Fin del bloqueo temporal por intentos fallidos (HU-42 esc. 4).</summary>
    public DateTime? BloqueadoHasta { get; set; }
}