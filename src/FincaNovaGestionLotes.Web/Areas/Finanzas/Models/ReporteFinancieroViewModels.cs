using Microsoft.AspNetCore.Mvc.Rendering;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Models;

public class ReporteFinancieroViewModel
{
    public int? PeriodoProductivoId { get; set; }

    public IEnumerable<SelectListItem> Periodos { get; set; } = Enumerable.Empty<SelectListItem>();

    public decimal TotalIngresos { get; set; }

    public decimal TotalEgresos { get; set; }

    public decimal Balance => TotalIngresos - TotalEgresos;

    public IReadOnlyList<ResumenFinancieroViewModel> PorPeriodo { get; set; } = Array.Empty<ResumenFinancieroViewModel>();

    public IReadOnlyList<ResumenFinancieroViewModel> PorMes { get; set; } = Array.Empty<ResumenFinancieroViewModel>();
}

/// <summary>Una línea del reporte: ingresos, egresos y balance de un período o de un mes.</summary>
public class ResumenFinancieroViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Ingresos { get; set; }
    public decimal Egresos { get; set; }
    public decimal Balance => Ingresos - Egresos;
}
