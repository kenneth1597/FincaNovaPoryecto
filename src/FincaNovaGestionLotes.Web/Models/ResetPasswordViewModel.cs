using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).*$",
        ErrorMessage = "La contraseña debe incluir al menos una letra mayúscula y un carácter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar Nueva Contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}