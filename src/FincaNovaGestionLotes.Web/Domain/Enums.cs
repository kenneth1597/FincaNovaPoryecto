namespace FincaNovaGestionLotes.Web.Domain;


public enum TipoLote
{
    Lote = 1,
    MicroLote = 2
}


public enum EstadoLote
{
    Activo = 1,
    Inactivo = 2,
    EnProduccion = 3,
    EnDescanso = 4
}

public enum AccionAuditoria
{
    Crear = 1,
    Modificar = 2,
    Inactivar = 3,
    Eliminar = 4
}

public enum TipoMovimientoInventario
{
    Entrada = 1,
    Salida = 2,
    Ajuste = 3
}

public enum UnidadVentaCafe
{
    Cajuela = 1,
    Fanega = 2,
    Kilogramo = 3
}

public enum TipoDetallePago
{
    Jornada = 1,
    Cajuela = 2
}
