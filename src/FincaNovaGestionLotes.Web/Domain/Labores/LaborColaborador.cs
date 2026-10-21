using FincaNovaGestionLotes.Web.Domain.Colaboradores;

namespace FincaNovaGestionLotes.Web.Domain.Labores;

public class LaborColaborador
{
    public int LaborId { get; set; }
    public Labor Labor { get; set; } = null!;

    public int ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;
}
