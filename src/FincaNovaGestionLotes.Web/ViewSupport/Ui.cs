using FincaNovaGestionLotes.Web.Domain;

namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class Ui
{
    public static string EstadoLoteTexto(EstadoLote e) => e switch
    {
        EstadoLote.Activo => "Activo",
        EstadoLote.Inactivo => "Inactivo",
        EstadoLote.EnProduccion => "En producción",
        EstadoLote.EnDescanso => "En descanso",
        _ => e.ToString()
    };

    public static string EstadoLoteColor(EstadoLote e) => e switch
    {
        EstadoLote.Activo => "success",
        EstadoLote.EnProduccion => "primary",
        EstadoLote.EnDescanso => "warning",
        EstadoLote.Inactivo => "secondary",
        _ => "secondary"
    };

    public static string TipoLoteTexto(TipoLote t) => t == TipoLote.MicroLote ? "Micro lote" : "Lote";
}
