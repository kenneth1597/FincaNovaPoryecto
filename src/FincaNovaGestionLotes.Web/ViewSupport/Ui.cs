using FincaNovaGestionLotes.Web.Domain;

namespace FincaNovaGestionLotes.Web.ViewSupport;

public static class Ui
{
    public static string EstadoTerrenoTexto(EstadoTerreno e) => e switch
    {
        EstadoTerreno.Activo => "Activo",
        EstadoTerreno.Inactivo => "Inactivo",
        EstadoTerreno.EnProduccion => "En producción",
        EstadoTerreno.EnDescanso => "En descanso",
        _ => e.ToString()
    };

    public static string EstadoTerrenoColor(EstadoTerreno e) => e switch
    {
        EstadoTerreno.Activo => "success",
        EstadoTerreno.EnProduccion => "primary",
        EstadoTerreno.EnDescanso => "warning",
        EstadoTerreno.Inactivo => "secondary",
        _ => "secondary"
    };

    public static string TipoTerrenoTexto(TipoTerreno t) => t == TipoTerreno.Lote ? "Lote" : "Finca";

    public static string TipoTerrenoConArticulo(TipoTerreno t) => t == TipoTerreno.Lote ? "el lote" : "la finca";

    public static string TerminacionGenero(TipoTerreno t) => t == TipoTerreno.Lote ? "o" : "a";

    public static string TipoLaborTexto(TipoLabor t) => t switch
    {
        TipoLabor.Poda => "Poda",
        TipoLabor.Atomizacion => "Atomización",
        TipoLabor.Limpia => "Limpia",
        TipoLabor.Fertilizacion => "Fertilización",
        TipoLabor.Deshija => "Deshija",
        TipoLabor.Otra => "Otra",
        _ => t.ToString()
    };

    public static string NombreLabor(TipoLabor tipo, string? tipoOtro) =>
        tipo == TipoLabor.Otra && !string.IsNullOrWhiteSpace(tipoOtro)
            ? tipoOtro.Trim()
            : TipoLaborTexto(tipo);

    public static string EstadoEnfermedadTexto(EstadoEnfermedad e) => e switch
    {
        EstadoEnfermedad.Activa => "Activa",
        EstadoEnfermedad.EnTratamiento => "En tratamiento",
        EstadoEnfermedad.Cerrada => "Cerrada",
        _ => e.ToString()
    };

    public static string EstadoEnfermedadBadge(EstadoEnfermedad e) => e switch
    {
        EstadoEnfermedad.Activa => "fn-badge-danger",
        EstadoEnfermedad.EnTratamiento => "fn-badge-warning",
        EstadoEnfermedad.Cerrada => "fn-badge-success",
        _ => "fn-badge-neutral"
    };

    public static string ResultadoCierreTexto(ResultadoCierreEnfermedad r) => r switch
    {
        ResultadoCierreEnfermedad.Controlada => "Controlada",
        ResultadoCierreEnfermedad.Erradicada => "Erradicada",
        ResultadoCierreEnfermedad.PerdidaDePlantas => "Pérdida de plantas",
        _ => r.ToString()
    };

    public static string EfectividadTexto(EfectividadTratamiento e) => e switch
    {
        EfectividadTratamiento.SinMejoria => "Sin mejoría",
        EfectividadTratamiento.MejoriaParcial => "Mejoría parcial",
        EfectividadTratamiento.Efectivo => "Efectivo",
        _ => e.ToString()
    };

    public static string EfectividadBadge(EfectividadTratamiento e) => e switch
    {
        EfectividadTratamiento.SinMejoria => "fn-badge-danger",
        EfectividadTratamiento.MejoriaParcial => "fn-badge-warning",
        EfectividadTratamiento.Efectivo => "fn-badge-success",
        _ => "fn-badge-neutral"
    };

    public static string EstadoLaborTexto(EstadoLabor e) => e switch
    {
        EstadoLabor.Pendiente => "Pendiente",
        EstadoLabor.EnProceso => "En proceso",
        EstadoLabor.Finalizada => "Finalizada",
        _ => e.ToString()
    };

    public static string EstadoLaborBadge(EstadoLabor e) => e switch
    {
        EstadoLabor.Pendiente => "fn-badge-warning",
        EstadoLabor.EnProceso => "fn-badge-info",
        EstadoLabor.Finalizada => "fn-badge-success",
        _ => "fn-badge-neutral"
    };
}
