using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Inventario;
using FincaNovaGestionLotes.Web.Domain.Labores;

namespace FincaNovaGestionLotes.Web.Services.Inventario;

public interface IInventarioService
{
    /// <summary>Suma la cantidad al stock y registra el movimiento de entrada (HU-22).</summary>
    Task RegistrarEntradaAsync(Insumo insumo, decimal cantidad, DateTime fecha,
        DateTime? fechaVencimiento, string? observaciones);

    /// <summary>
    /// Resta la cantidad del stock y registra la salida ligada a su origen (HU-24).
    /// Lanza <see cref="InventarioInsuficienteException"/> si no alcanza.
    /// El insumo debe venir con su UnidadMedida cargada para el mensaje de error.
    /// </summary>
    Task RegistrarSalidaAsync(Insumo insumo, decimal cantidad, DateTime fecha, string observaciones,
        ProductoTratamiento? productoTratamiento = null, LaborInsumo? laborInsumo = null);

    /// <summary>Abre o cierra la alerta de stock mínimo del insumo (HU-25).</summary>
    Task SincronizarAlertaAsync(Insumo insumo);
}
