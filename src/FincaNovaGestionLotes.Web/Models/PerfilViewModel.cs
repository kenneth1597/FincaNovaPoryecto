using System.ComponentModel.DataAnnotations;

namespace FincaNovaGestionLotes.Web.Models
{
    public class PerfilViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        public string RolNombre { get; set; } = string.Empty;

        // Campos opcionales para cambio de contraseña desde el perfil
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string? CurrentPassword { get; set; }

        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "La nueva contraseña debe incluir al menos una letra mayúscula y un carácter especial.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string? ConfirmPassword { get; set; }




    }
}