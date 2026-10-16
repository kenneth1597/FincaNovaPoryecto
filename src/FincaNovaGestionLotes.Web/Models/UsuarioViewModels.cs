using System.ComponentModel.DataAnnotations;


namespace FincaNovaGestionLotes.Web.Models;

public class UsuarioCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public string Rol { get; set; } = "Trabajador";

    public bool Activo { get; set; } = true;
}

public class UsuarioEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public string Rol { get; set; } = string.Empty;

    public bool Activo { get; set; }

    [Display(Name = "Nueva contraseña (opcional)")]
    public string? NewPassword { get; set; }
}