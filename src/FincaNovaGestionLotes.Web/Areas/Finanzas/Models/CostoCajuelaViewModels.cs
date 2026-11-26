using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Models;

public class CostoCajuelaViewModel
{
    public int? PeriodoProductivoId { get; set; }

    public int Pagina { get; set; } = 1;

    public IEnumerable<SelectListItem> Periodos { get; set; } = Enumerable.Empty<SelectListItem>();

    /// <summary>Gastos de los períodos mostrados.</summary>
    public decimal TotalGastos { get; set; }

    /// <summary>Cajuelas recolectadas en los períodos mostrados.</summary>
    public decimal TotalCajuelas { get; set; }

    /// <summary>Gastos entre cajuelas; vacío si todavía no hay cajuelas recolectadas.</summary>
    public decimal? CostoPorCajuela => TotalCajuelas > 0 ? TotalGastos / TotalCajuelas : null;

    /// <summary>Gastos que no tienen período y por eso no entran en el cálculo.</summary>
    public decimal GastosSinPeriodo { get; set; }

    public IReadOnlyList<CostoCajuelaPeriodoViewModel> Filas { get; set; } = Array.Empty<CostoCajuelaPeriodoViewModel>();

    public PaginaInfo Paginacion { get; set; } = new();
}

public class CostoCajuelaPeriodoViewModel
{
    public string Periodo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Cerrado { get; set; }
    public decimal Gastos { get; set; }
    public decimal Cajuelas { get; set; }

    public decimal? CostoPorCajuela => Cajuelas > 0 ? Gastos / Cajuelas : null;
}
