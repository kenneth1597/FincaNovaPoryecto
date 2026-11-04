using FincaNovaGestionLotes.Web.Domain.Terrenos;

namespace FincaNovaGestionLotes.Web.Domain.Produccion;

/// <summary>Finca o lote asignado a un período productivo.</summary>
public class PeriodoTerreno
{
    public int PeriodoProductivoId { get; set; }
    public PeriodoProductivo PeriodoProductivo { get; set; } = null!;

    public int TerrenoId { get; set; }
    public Terreno Terreno { get; set; } = null!;
}
