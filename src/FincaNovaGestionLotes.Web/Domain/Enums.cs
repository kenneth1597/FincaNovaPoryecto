namespace FincaNovaGestionLotes.Web.Domain;


public enum TipoTerreno
{
    Finca = 1,
    Lote = 2
}


public enum EstadoTerreno
{
    Activo = 1,
    Inactivo = 2,
    EnProduccion = 3,
    EnDescanso = 4
}

public enum TipoLabor
{
    Poda = 1,
    Atomizacion = 2,
    Limpia = 3,
    Fertilizacion = 4,
    Deshija = 5,
    Otra = 6
}

public enum EstadoLabor
{
    Pendiente = 1,
    EnProceso = 2,
    Finalizada = 3
}

public enum EstadoEnfermedad
{
    Activa = 1,
    EnTratamiento = 2,
    Cerrada = 3
}

public enum ResultadoCierreEnfermedad
{
    Controlada = 1,
    Erradicada = 2,
    PerdidaDePlantas = 3
}

public enum EfectividadTratamiento
{
    SinMejoria = 1,
    MejoriaParcial = 2,
    Efectivo = 3
}

public enum AccionAuditoria
{
    Crear = 1,
    Modificar = 2,
    Inactivar = 3,
    Eliminar = 4
}
