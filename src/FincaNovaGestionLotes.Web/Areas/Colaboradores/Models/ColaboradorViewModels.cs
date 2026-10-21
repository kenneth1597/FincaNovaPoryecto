using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Colaboradores.Models;

public class ColaboradorFiltroViewModel
{
    public string? Q { get; set; }

    public bool? Activo { get; set; }

    public int Pagina { get; set; } = 1;

    public PaginaInfo Paginacion { get; set; } = new();

    public IReadOnlyList<ColaboradorListItemViewModel> Resultados { get; set; }
        = Array.Empty<ColaboradorListItemViewModel>();
}

public class ColaboradorListItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal TarifaJornada { get; set; }
    public bool Activo { get; set; }
    public int Labores { get; set; }
    public string? UsuarioEmail { get; set; }
}

public class UsuarioOpcionViewModel
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
}

public class ColaboradorFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [StringLength(30)]
    [RegularExpression(@"^[A-Za-z0-9\-]{5,30}$", ErrorMessage = "La identificación solo admite letras, números y guion (mínimo 5 caracteres).")]
    [Display(Name = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30)]
    [RegularExpression(@"^[0-9\-\+ ]{8,30}$", ErrorMessage = "El teléfono solo admite números, espacios, + y guion (mínimo 8).")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Cargo")]
    public string Cargo { get; set; } = string.Empty;

    [Range(0.01, 10000000, ErrorMessage = "La tarifa por jornada debe ser mayor a cero.")]
    [Display(Name = "Tarifa por jornada (₡)")]
    public decimal TarifaJornada { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Usuario del sistema")]
    public int? UsuarioId { get; set; }

    public IEnumerable<UsuarioOpcionViewModel> Usuarios { get; set; }
        = Enumerable.Empty<UsuarioOpcionViewModel>();

    public bool EsEdicion => Id != 0;
}
