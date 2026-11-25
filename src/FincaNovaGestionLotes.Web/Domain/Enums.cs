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
