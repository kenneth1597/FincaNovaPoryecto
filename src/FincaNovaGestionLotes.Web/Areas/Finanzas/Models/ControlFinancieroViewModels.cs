using System.ComponentModel.DataAnnotations;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FincaNovaGestionLotes.Web.Areas.Finanzas.Models;

public class ControlFinancieroViewModel
{
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }

    public int? PeriodoProductivoId { get; set; }

    public int Pagina { get; set; } = 1;

    public IEnumerable<SelectListItem> Periodos { get; set; } = Enumerable.Empty<SelectListItem>();

    /// <summary>Suma de las ventas que cumplen el filtro.</summary>
    public decimal TotalIngresos { get; set; }

    /// <summary>Suma de los gastos que cumplen el filtro.</summary>
    public decimal TotalEgresos { get; set; }

    public decimal Balance => TotalIngresos - TotalEgresos;

    public IReadOnlyList<MovimientoViewModel> Movimientos { get; set; } = Array.Empty<MovimientoViewModel>();
    public PaginaInfo Paginacion { get; set; } = new();

    public IReadOnlyList<EgresoCategoriaViewModel> EgresosPorCategoria { get; set; } = Array.Empty<EgresoCategoriaViewModel>();
}

public class MovimientoViewModel
{
    public DateTime Fecha { get; set; }
    public bool EsIngreso { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public decimal Monto { get; set; }
    public string? Periodo { get; set; }
}

public class EgresoCategoriaViewModel
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
