using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.Domain.Colaboradores;

namespace FincaNovaGestionLotes.Web.Domain.Labores;

public class LaborColaborador
{
    public int LaborId { get; set; }
    public Labor Labor { get; set; } = null!;

    public int ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    /// <summary>El colaborador confirmó que terminó su parte (solo con horas registradas).</summary>
    public bool ParteTerminada { get; set; }

    public DateTime? FechaParteTerminada { get; set; }

    /// <summary>Quién marcó la parte: el propio colaborador o un administrador en su nombre.</summary>
    [StringLength(150)]
    public string? ParteTerminadaPor { get; set; }

    [StringLength(500)]
    public string? ComentarioParte { get; set; }
}
