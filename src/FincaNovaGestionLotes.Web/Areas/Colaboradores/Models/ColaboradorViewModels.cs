using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.ViewSupport;

namespace FincaNovaGestionLotes.Web.Areas.Colaboradores.Models;

public class ColaboradorFiltroViewModel
{
    public string? Q { get; set; }

    public string Estado { get; set; } = "Activos";

    public string Orden { get; set; } = "nombre";

    public int Pagina { get; set; } = 1;

    public PaginaInfo Paginacion { get; set; } = new();

    public IReadOnlyList<ColaboradorListItemViewModel> Resultados { get; set; }
        = Array.Empty<ColaboradorListItemViewModel>();

    public static readonly IReadOnlyList<(string Valor, string Texto)> Ordenes = new[]
    {
        ("nombre", "Nombre (A-Z)"),
        ("nombre_desc", "Nombre (Z-A)"),
        ("cargo", "Cargo"),
        ("labores", "Más labores"),
        ("horas", "Más horas trabajadas"),
        ("reciente", "Participación más reciente")
    };
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
    public decimal Horas { get; set; }
    public DateTime? UltimaLabor { get; set; }
    public string? UsuarioEmail { get; set; }
}

public class ColaboradorHistorialViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal TarifaJornada { get; set; }
    public bool Activo { get; set; }
    public string? UsuarioEmail { get; set; }

    public int? TerrenoId { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int Pagina { get; set; } = 1;

    public int TotalLabores { get; set; }
    public int TotalDias { get; set; }
    public decimal TotalHoras { get; set; }
    public decimal TotalJornadas { get; set; }
    public decimal TotalMonto { get; set; }
    public decimal HorasPorJornada { get; set; }

    public PaginaInfo Paginacion { get; set; } = new();
    public IReadOnlyList<HistorialLaborViewModel> Historial { get; set; } = Array.Empty<HistorialLaborViewModel>();
    public IEnumerable<(int Id, string Texto)> Terrenos { get; set; } = Enumerable.Empty<(int, string)>();

    public bool HayFiltros => TerrenoId != null || Desde != null || Hasta != null;
}

public class HistorialLaborViewModel
{
    public int LaborId { get; set; }
    public DateTime Fecha { get; set; }
    public string TerrenoCodigo { get; set; } = string.Empty;
    public string TerrenoNombre { get; set; } = string.Empty;
    public TipoLabor Tipo { get; set; }
    public string? TipoOtro { get; set; }
    public string NombreLabor => Ui.NombreLabor(Tipo, TipoOtro);
    public EstadoLabor Estado { get; set; }
    public int Dias { get; set; }
    public decimal Horas { get; set; }
    public decimal Jornadas { get; set; }
    public decimal Monto { get; set; }
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
    [StringLength(150, ErrorMessage = Validaciones.LongitudMsg)]
    [RegularExpression(Validaciones.NombrePersona, ErrorMessage = Validaciones.NombrePersonaMsg)]
    [Display(Name = "Nombre completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [StringLength(30, ErrorMessage = Validaciones.LongitudMsg)]
    [RegularExpression(@"^[A-Za-z0-9\-]{5,30}$", ErrorMessage = "La identificación solo admite letras, números y guion (mínimo 5 caracteres).")]
    [Display(Name = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(30, ErrorMessage = Validaciones.LongitudMsg)]
    [RegularExpression(Validaciones.Telefono, ErrorMessage = Validaciones.TelefonoMsg)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio.")]
    [StringLength(100, ErrorMessage = Validaciones.LongitudMsg)]
    [RegularExpression(Validaciones.Texto, ErrorMessage = Validaciones.TextoMsg)]
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
