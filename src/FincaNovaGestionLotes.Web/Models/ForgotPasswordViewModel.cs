using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public string Email { get; set; } = string.Empty;
}