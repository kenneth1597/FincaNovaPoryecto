using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Enfermedades;
using FincaNovaGestionLotes.Web.Domain.Inventario;
using FincaNovaGestionLotes.Web.Domain.Labores;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.EntityFrameworkCore;

namespace FincaNovaGestionLotes.Web.Services.Inventario;

/// <summary>
/// Movimientos del inventario de insumos. Los métodos solo preparan los cambios
/// en el contexto: quien los llama hace SaveChanges, así el movimiento y el
/// registro que lo origina (tratamiento, labor) se guardan juntos o no se guarda nada.
/// </summary>
public class InventarioService : IInventarioService
{
    /// <summary>Se muestra cuando otra persona movió el mismo insumo mientras se registraba.</summary>
    public const string MensajeConcurrencia =
        "Otra persona actualizó el inventario de este insumo al mismo tiempo. No se guardó nada; revise las existencias e intente de nuevo.";

    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _http;

    public InventarioService(AppDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    private string? NombreUsuario => _http.HttpContext?.Session.GetString("UsuarioNombre");

    public async Task RegistrarEntradaAsync(Insumo insumo, decimal cantidad, DateTime fecha,
        DateTime? fechaVencimiento, string? observaciones)
    {
        insumo.StockActual += cantidad;

        _db.MovimientosInventario.Add(new MovimientoInventario
        {
            Insumo = insumo,
            Tipo = TipoMovimientoInventario.Entrada,
            Cantidad = cantidad,
            StockResultante = insumo.StockActual,
            Fecha = fecha,
            FechaVencimiento = fechaVencimiento,
            Observaciones = observaciones,
            RegistradoPor = NombreUsuario,
            CreadoPor = NombreUsuario
        });

        await SincronizarAlertaAsync(insumo);
    }

    public async Task RegistrarSalidaAsync(Insumo insumo, decimal cantidad, DateTime fecha, string observaciones,
        ProductoTratamiento? productoTratamiento = null, LaborInsumo? laborInsumo = null)
    {
        // HU-24 esc. 2: nunca se rebaja más de lo que hay.
        if (cantidad > insumo.StockActual)
            throw new InventarioInsuficienteException(insumo);

        insumo.StockActual -= cantidad;

        _db.MovimientosInventario.Add(new MovimientoInventario
        {
            Insumo = insumo,
            Tipo = TipoMovimientoInventario.Salida,
            Cantidad = cantidad,
            StockResultante = insumo.StockActual,
            Fecha = fecha,
            Observaciones = observaciones,
            RegistradoPor = NombreUsuario,
            CreadoPor = NombreUsuario,
            ProductoTratamiento = productoTratamiento,
            LaborInsumo = laborInsumo
        });

        await SincronizarAlertaAsync(insumo);
    }

    /// <summary>
    /// HU-25: abre una alerta cuando el stock llega al mínimo y la cierra cuando
    /// se reabastece. Un mínimo de 0 significa que no se configuró.
    /// </summary>
    public async Task SincronizarAlertaAsync(Insumo insumo)
    {
        var bajoMinimo = insumo.Activo && insumo.StockMinimo > 0 && insumo.StockActual <= insumo.StockMinimo;

        var activa = _db.AlertasStock.Local.FirstOrDefault(a => a.Insumo == insumo && a.Activa)
                     ?? (insumo.Id == 0
                         ? null
                         : await _db.AlertasStock.FirstOrDefaultAsync(a => a.InsumoId == insumo.Id && a.Activa));

        if (bajoMinimo && activa is null)
        {
            _db.AlertasStock.Add(new AlertaStock
            {
                Insumo = insumo,
                FechaAlerta = DateTime.UtcNow,
                StockAlMomento = insumo.StockActual,
                StockMinimo = insumo.StockMinimo,
                Activa = true,
                CreadoPor = NombreUsuario ?? "Sistema"
            });
        }
        else if (bajoMinimo && activa is not null)
        {
            activa.StockAlMomento = insumo.StockActual;
            activa.StockMinimo = insumo.StockMinimo;
        }
        else if (!bajoMinimo && activa is not null)
        {
            activa.Activa = false;
            activa.FechaResolucion = DateTime.UtcNow;
            activa.ModificadoPor = NombreUsuario;
        }
    }
}

public class InventarioInsuficienteException : Exception
{
    public InventarioInsuficienteException(Insumo insumo)
        : base($"Inventario insuficiente de {insumo.Nombre}. Existencias actuales: " +
               Ui.CantidadConUnidad(insumo.StockActual, insumo.UnidadMedida?.Nombre))
    {
        Insumo = insumo;
    }

    public Insumo Insumo { get; }

    /// <summary>Texto de las existencias, ej. «3 sacos».</summary>
    public string Existencias => Ui.CantidadConUnidad(Insumo.StockActual, Insumo.UnidadMedida?.Nombre);
}
