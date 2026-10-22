using FincaNovaGestionLotes.Web.Areas.Labores.Models;
using FincaNovaGestionLotes.Web.Data;
using FincaNovaGestionLotes.Web.Domain;
using FincaNovaGestionLotes.Web.Domain.Labores;
using FincaNovaGestionLotes.Web.Filters;
using FincaNovaGestionLotes.Web.Services.Auditoria;
using FincaNovaGestionLotes.Web.Services.Labores;
using FincaNovaGestionLotes.Web.ViewSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FincaNovaGestionLotes.Web.Areas.Labores.Controllers;

[Area("Labores")]
[RequiereRol(Roles.Trabajador, Roles.Capataz, Roles.Administrador, Roles.Duenio)]
public class LaboresController : Controller
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ParametrosJornada _jornada;

    public LaboresController(AppDbContext db, IAuditService audit, IOptions<ParametrosJornada> jornada)
    {
        _db = db;
        _audit = audit;
        _jornada = jornada.Value;
    }

    private bool EsAdmin => Roles.EsAdmin(HttpContext);

    private string? NombreUsuario => HttpContext.Session.GetString("UsuarioNombre");

    [HttpGet]
    public async Task<IActionResult> Index(LaborFiltroViewModel filtro)
    {
        var miColaboradorId = await MiColaboradorIdAsync();
        filtro.TieneColaborador = miColaboradorId is not null;
        filtro.EsAdmin = EsAdmin;

        var query = _db.Labores.AsNoTracking();

        if (filtro.TerrenoId is { } terrenoId)
            query = query.Where(l => l.TerrenoId == terrenoId);

        if (filtro.Tipo is { } tipo)
            query = query.Where(l => l.Tipo == tipo);

        if (filtro.Estado is { } estado)
            query = query.Where(l => l.Estado == estado);

        if (filtro.Desde is { } desde)
            query = query.Where(l => l.Fecha >= desde.Date);

        if (filtro.Hasta is { } hasta)
        {
            var limite = hasta.Date.AddDays(1);
            query = query.Where(l => l.Fecha < limite);
        }

        if (filtro.SoloMias && miColaboradorId is { } mio)
            query = query.Where(l => l.Colaboradores.Any(c => c.ColaboradorId == mio));

        var (resultados, paginacion) = await query
            .OrderByDescending(l => l.Fecha)
            .ThenByDescending(l => l.Id)
            .Select(l => new LaborListItemViewModel
            {
                Id = l.Id,
                TerrenoId = l.TerrenoId,
                TerrenoCodigo = l.Terreno.Codigo,
                TerrenoNombre = l.Terreno.Nombre,
                Tipo = l.Tipo,
                TipoOtro = l.TipoOtro,
                Fecha = l.Fecha,
                Estado = l.Estado,
                HorasTotales = l.HorasTotales,
                CostoTotal = l.CostoTotal,
                MiMonto = l.Avances.Where(a => a.ColaboradorId == miColaboradorId).Sum(a => (decimal?)a.Costo) ?? 0,
                RegistradoPor = l.RegistradoPor,
                Colaboradores = l.Colaboradores
                    .OrderBy(c => c.Colaborador.Nombre)
                    .Select(c => c.Colaborador.Nombre)
                    .ToList(),
                EsMia = miColaboradorId != null && l.Colaboradores.Any(c => c.ColaboradorId == miColaboradorId)
            })
            .PaginarAsync(filtro.Pagina);

        filtro.Resultados = resultados;
        filtro.Paginacion = paginacion;

        filtro.Terrenos = await TerrenosAsync(soloDisponibles: false);

        return View(filtro);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var vm = await ConstruirDetalleAsync(id);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(int? terrenoId = null)
    {
        var model = new LaborFormViewModel { TerrenoId = terrenoId };
        if (!await PrepararFormularioAsync(model))
            return RedirectToAction(nameof(Index));

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(LaborFormViewModel model)
    {
        if (!await PrepararFormularioAsync(model))
            return RedirectToAction(nameof(Index));

        var idsColaboradores = await ResolverColaboradoresAsync(model);
        await ValidarFormularioAsync(model, idsColaboradores);

        decimal? horasTrabajo = null;
        if (model.YaRealizado)
        {
            if (model.Fecha is { } fechaTrabajo && fechaTrabajo.Date > DateTime.Today)
                ModelState.AddModelError(nameof(model.Fecha), "Si el trabajo ya se realizó, la fecha no puede ser futura.");

            if (model.ModoTrabajo == ModoRegistroAvance.JornadaCompleta)
                horasTrabajo = _jornada.HorasPorJornada;
            else if (model.ModoTrabajo == ModoRegistroAvance.PorHoras)
            {
                if (model.HorasTrabajo is null)
                    ModelState.AddModelError(nameof(model.HorasTrabajo), "Indique las horas trabajadas.");
                else
                    horasTrabajo = model.HorasTrabajo;
            }
            else
                ModelState.AddModelError(nameof(model.ModoTrabajo), "Seleccione cómo se registra el trabajo.");

            if (horasTrabajo is { } horasNuevas && model.Fecha is { } dia)
            {
                foreach (var colaboradorId in idsColaboradores)
                {
                    if (await HorasDelDiaAsync(colaboradorId, dia) + horasNuevas > 24)
                    {
                        ModelState.AddModelError(nameof(model.HorasTrabajo),
                            $"Un colaborador asignado ya supera las 24 h registradas el {dia:dd/MM/yyyy}.");
                        break;
                    }
                }
            }
        }

        if (!ModelState.IsValid)
            return View(model);

        var labor = new Labor
        {
            TerrenoId = model.TerrenoId!.Value,
            Tipo = model.Tipo!.Value,
            TipoOtro = model.Tipo == TipoLabor.Otra ? model.TipoOtro?.Trim() : null,
            Fecha = model.Fecha!.Value.Date,
            Observaciones = model.Observaciones!.Trim(),
            Estado = EstadoLabor.Pendiente,
            RegistradoPor = NombreUsuario,
            CreadoPor = NombreUsuario
        };

        foreach (var colaboradorId in idsColaboradores)
            labor.Colaboradores.Add(new LaborColaborador { ColaboradorId = colaboradorId });

        if (horasTrabajo is { } horasIniciales)
        {
            foreach (var colaboradorId in idsColaboradores)
                await AgregarAvanceAsync(labor, colaboradorId, labor.Fecha, horasIniciales, labor.Observaciones);
        }

        _db.Labores.Add(labor);
        await _db.SaveChangesAsync();

        var codigoLote = await CodigoTerrenoAsync(labor.TerrenoId);
        await _audit.RegistrarAsync(AccionAuditoria.Crear, nameof(Labor), labor.Id.ToString(),
            $"Registro de labor {Ui.NombreLabor(labor.Tipo, labor.TipoOtro)} en el lote {codigoLote} ({idsColaboradores.Count} colaborador(es))");

        TempData["Ok"] = horasTrabajo is { } h
            ? $"Labor de {Ui.NombreLabor(labor.Tipo, labor.TipoOtro).ToLower()} registrada en «{codigoLote}» con {h:N2} h por colaborador (₡{labor.CostoTotal:N2})."
            : $"Labor de {Ui.NombreLabor(labor.Tipo, labor.TipoOtro).ToLower()} registrada en el lote «{codigoLote}».";
        return RedirectToAction(nameof(Detalle), new { id = labor.Id });
    }

    [HttpGet]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Editar(int id)
    {
        var labor = await _db.Labores.AsNoTracking()
            .Include(l => l.Colaboradores)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (labor is null) return NotFound();

        if (labor.Estado == EstadoLabor.Finalizada)
        {
            TempData["Error"] = "La labor ya está finalizada y no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var model = new LaborFormViewModel
        {
            Id = labor.Id,
            TerrenoId = labor.TerrenoId,
            Tipo = labor.Tipo,
            TipoOtro = labor.TipoOtro,
            Fecha = labor.Fecha,
            Observaciones = labor.Observaciones,
            ColaboradoresIds = labor.Colaboradores.Select(c => c.ColaboradorId).ToList()
        };
        await PrepararFormularioAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol(Roles.Administrador, Roles.Duenio)]
    public async Task<IActionResult> Editar(LaborFormViewModel model)
    {
        var labor = await _db.Labores
            .Include(l => l.Colaboradores)
            .FirstOrDefaultAsync(l => l.Id == model.Id);
        if (labor is null) return NotFound();

        if (labor.Estado == EstadoLabor.Finalizada)
        {
            TempData["Error"] = "La labor ya está finalizada y no se puede editar.";
            return RedirectToAction(nameof(Detalle), new { id = labor.Id });
        }

        await PrepararFormularioAsync(model);
        var idsColaboradores = await ResolverColaboradoresAsync(model);
        await ValidarFormularioAsync(model, idsColaboradores, labor.Colaboradores.Select(c => c.ColaboradorId).ToList());

        var conAvances = await _db.LaboresAvances
            .Where(a => a.LaborId == labor.Id)
            .Select(a => a.ColaboradorId)
            .Distinct()
            .ToListAsync();
        var quitados = conAvances.Except(idsColaboradores).ToList();
        if (quitados.Count > 0)
            ModelState.AddModelError(nameof(model.ColaboradoresIds),
                "No se puede quitar a un colaborador que ya registró avances en esta labor.");

        if (!ModelState.IsValid)
            return View(model);

        labor.TerrenoId = model.TerrenoId!.Value;
        labor.Tipo = model.Tipo!.Value;
        labor.TipoOtro = model.Tipo == TipoLabor.Otra ? model.TipoOtro?.Trim() : null;
        labor.Fecha = model.Fecha!.Value.Date;
        labor.Observaciones = model.Observaciones!.Trim();
        labor.ModificadoPor = NombreUsuario;

        var actuales = labor.Colaboradores.Select(c => c.ColaboradorId).ToList();
        foreach (var quitar in labor.Colaboradores.Where(c => !idsColaboradores.Contains(c.ColaboradorId)).ToList())
            labor.Colaboradores.Remove(quitar);
        foreach (var agregar in idsColaboradores.Except(actuales))
            labor.Colaboradores.Add(new LaborColaborador { ColaboradorId = agregar });

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Labor), labor.Id.ToString(),
            $"Edición de la labor {Ui.NombreLabor(labor.Tipo, labor.TipoOtro)} del lote {await CodigoTerrenoAsync(labor.TerrenoId)}");

        TempData["Ok"] = "Labor actualizada.";
        return RedirectToAction(nameof(Detalle), new { id = labor.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarAvance(int id, [Bind(Prefix = "NuevoAvance")] AvanceFormViewModel avance)
    {
        var labor = await _db.Labores
            .Include(l => l.Colaboradores)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (labor is null) return NotFound();

        var miColaboradorId = await MiColaboradorIdAsync();
        var asignados = labor.Colaboradores.Select(c => c.ColaboradorId).ToList();

        if (!PuedeActualizar(asignados, miColaboradorId))
            return RedirectToAction("AccessDenied", "Account", new { area = "" });

        if (labor.Estado == EstadoLabor.Finalizada)
        {
            TempData["Error"] = "La labor ya está finalizada; no admite más avances.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var colaboradorId = EsAdmin ? avance.ColaboradorId : miColaboradorId;
        if (colaboradorId is null || !asignados.Contains(colaboradorId.Value))
            ModelState.AddModelError("NuevoAvance.ColaboradorId", "Debe indicar un colaborador asignado a esta labor.");

        if (avance.Fecha is { } fecha)
        {
            if (fecha.Date > DateTime.Today)
                ModelState.AddModelError("NuevoAvance.Fecha", "La fecha del avance no puede ser futura.");
            else if (fecha.Date < labor.Fecha.Date)
                ModelState.AddModelError("NuevoAvance.Fecha",
                    $"La fecha del avance no puede ser anterior al inicio de la labor ({labor.Fecha:dd/MM/yyyy}).");
        }

        decimal? horasRegistro = null;
        if (avance.Modo == ModoRegistroAvance.JornadaCompleta)
            horasRegistro = _jornada.HorasPorJornada;
        else if (avance.Modo == ModoRegistroAvance.PorHoras)
        {
            if (avance.Horas is null)
                ModelState.AddModelError("NuevoAvance.Horas", "Indique las horas trabajadas.");
            else
                horasRegistro = avance.Horas;
        }
        else
            ModelState.AddModelError("NuevoAvance.Modo", "Seleccione cómo se registra el trabajo.");

        if (colaboradorId is { } idColaborador && avance.Fecha is { } dia && horasRegistro is { } horasNuevas)
        {
            var horasDelDia = await HorasDelDiaAsync(idColaborador, dia);
            if (horasDelDia + horasNuevas > 24)
                ModelState.AddModelError("NuevoAvance.Horas",
                    $"El colaborador ya tiene {horasDelDia:N2} h registradas el {dia:dd/MM/yyyy}; el día no puede superar 24 h.");
        }

        if (!ModelState.IsValid)
        {
            var vm = await ConstruirDetalleAsync(id);
            if (vm is null) return NotFound();
            vm.NuevoAvance = avance;
            return View(nameof(Detalle), vm);
        }

        var horas = horasRegistro!.Value;
        var horasPorJornada = _jornada.HorasPorJornada;
        var costo = await AgregarAvanceAsync(labor, colaboradorId!.Value, avance.Fecha!.Value, horas, avance.Comentario!);

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Labor), labor.Id.ToString(),
            $"Avance de {horas:N2} h registrado en la labor {Ui.NombreLabor(labor.Tipo, labor.TipoOtro)}");

        TempData["Ok"] = $"Avance de {horas:N2} h registrado ({horas / horasPorJornada:N2} jornada(s), ₡{costo:N2}).";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int id, [Bind(Prefix = "Cierre")] CierreFormViewModel cierre)
    {
        var labor = await _db.Labores
            .Include(l => l.Colaboradores)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (labor is null) return NotFound();

        var miColaboradorId = await MiColaboradorIdAsync();
        var asignados = labor.Colaboradores.Select(c => c.ColaboradorId).ToList();

        if (!PuedeActualizar(asignados, miColaboradorId))
            return RedirectToAction("AccessDenied", "Account", new { area = "" });

        if (labor.Estado == EstadoLabor.Finalizada)
        {
            TempData["Error"] = "La labor ya estaba finalizada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (labor.HorasTotales <= 0)
            ModelState.AddModelError("Cierre", "Antes de cerrar la labor debe registrar al menos un avance con las horas trabajadas.");

        if (!cierre.Confirmo)
            ModelState.AddModelError("Cierre.Confirmo", "Debe confirmar que verificó que todo el trabajo asignado se realizó.");

        if (!ModelState.IsValid)
        {
            var vm = await ConstruirDetalleAsync(id);
            if (vm is null) return NotFound();
            vm.Cierre = cierre;
            return View(nameof(Detalle), vm);
        }

        labor.Estado = EstadoLabor.Finalizada;
        labor.FechaCierre = DateTime.UtcNow;
        labor.CerradaPor = NombreUsuario;
        labor.ComentarioCierre = cierre.ComentarioCierre!.Trim();
        labor.ModificadoPor = NombreUsuario;

        await _db.SaveChangesAsync();

        await _audit.RegistrarAsync(AccionAuditoria.Modificar, nameof(Labor), labor.Id.ToString(),
            $"Cierre de la labor {Ui.NombreLabor(labor.Tipo, labor.TipoOtro)} con {labor.HorasTotales:N2} h acumuladas");

        TempData["Ok"] = $"Labor finalizada con {labor.HorasTotales:N2} horas acumuladas.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    private async Task<decimal> HorasDelDiaAsync(int colaboradorId, DateTime fecha) =>
        await _db.LaboresAvances
            .Where(a => a.ColaboradorId == colaboradorId && a.Fecha == fecha.Date)
            .SumAsync(a => (decimal?)a.Horas) ?? 0;

    private async Task<decimal> AgregarAvanceAsync(Labor labor, int colaboradorId, DateTime fecha, decimal horas, string comentario)
    {
        var tarifa = await _db.Colaboradores
            .Where(c => c.Id == colaboradorId)
            .Select(c => c.TarifaJornada)
            .FirstAsync();
        var horasPorJornada = _jornada.HorasPorJornada;
        var costo = Math.Round(horas / horasPorJornada * tarifa, 2);

        labor.Avances.Add(new LaborAvance
        {
            ColaboradorId = colaboradorId,
            Fecha = fecha.Date,
            Horas = horas,
            HorasPorJornada = horasPorJornada,
            TarifaJornada = tarifa,
            Costo = costo,
            Comentario = comentario.Trim(),
            RegistradoPor = NombreUsuario,
            CreadoPor = NombreUsuario
        });

        labor.HorasTotales += horas;
        labor.CostoTotal += costo;
        labor.Estado = EstadoLabor.EnProceso;
        labor.ModificadoPor = NombreUsuario;

        return costo;
    }

    private bool PuedeActualizar(IReadOnlyCollection<int> asignados, int? miColaboradorId) =>
        EsAdmin || (miColaboradorId is { } mio && asignados.Contains(mio));

    private async Task<int?> MiColaboradorIdAsync()
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId is null) return null;

        return await _db.Colaboradores.AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId && c.Activo)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<LaborDetalleViewModel?> ConstruirDetalleAsync(int id)
    {
        var vm = await _db.Labores.AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LaborDetalleViewModel
            {
                Id = l.Id,
                TerrenoId = l.TerrenoId,
                TerrenoCodigo = l.Terreno.Codigo,
                TerrenoNombre = l.Terreno.Nombre,
                Tipo = l.Tipo,
                TipoOtro = l.TipoOtro,
                Fecha = l.Fecha,
                Observaciones = l.Observaciones,
                RegistradoPor = l.RegistradoPor,
                Estado = l.Estado,
                HorasTotales = l.HorasTotales,
                CostoTotal = l.CostoTotal,
                FechaCierre = l.FechaCierre,
                CerradaPor = l.CerradaPor,
                ComentarioCierre = l.ComentarioCierre,
                Colaboradores = l.Colaboradores
                    .OrderBy(c => c.Colaborador.Nombre)
                    .Select(c => new ColaboradorLaborViewModel
                    {
                        Id = c.ColaboradorId,
                        Nombre = c.Colaborador.Nombre,
                        Cargo = c.Colaborador.Cargo,
                        TarifaJornada = c.Colaborador.TarifaJornada,
                        Dias = l.Avances.Where(a => a.ColaboradorId == c.ColaboradorId)
                            .Select(a => a.Fecha).Distinct().Count(),
                        Horas = l.Avances.Where(a => a.ColaboradorId == c.ColaboradorId).Sum(a => a.Horas),
                        Jornadas = l.Avances.Where(a => a.ColaboradorId == c.ColaboradorId)
                            .Sum(a => a.HorasPorJornada > 0 ? a.Horas / a.HorasPorJornada : 0),
                        Costo = l.Avances.Where(a => a.ColaboradorId == c.ColaboradorId).Sum(a => a.Costo)
                    })
                    .ToList(),
                Avances = l.Avances
                    .OrderByDescending(a => a.Fecha)
                    .ThenByDescending(a => a.Id)
                    .Select(a => new AvanceItemViewModel
                    {
                        Fecha = a.Fecha,
                        ColaboradorId = a.ColaboradorId,
                        Colaborador = a.Colaborador.Nombre,
                        Horas = a.Horas,
                        Jornadas = a.HorasPorJornada > 0 ? a.Horas / a.HorasPorJornada : 0,
                        TarifaJornada = a.TarifaJornada,
                        Costo = a.Costo,
                        Comentario = a.Comentario,
                        RegistradoPor = a.RegistradoPor
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (vm is null) return null;

        vm.HorasPorJornada = _jornada.HorasPorJornada;
        vm.EsAdmin = EsAdmin;
        vm.MiColaboradorId = await MiColaboradorIdAsync();
        vm.PuedeActualizar = vm.Estado != EstadoLabor.Finalizada &&
                             PuedeActualizar(vm.Colaboradores.Select(c => c.Id).ToList(), vm.MiColaboradorId);
        return vm;
    }

    private async Task<bool> PrepararFormularioAsync(LaborFormViewModel model)
    {
        model.Terrenos = await TerrenosAsync(soloDisponibles: true);
        model.PuedeElegirColaboradores = EsAdmin;
        model.HorasPorJornada = _jornada.HorasPorJornada;

        if (EsAdmin)
        {
            model.ColaboradoresDisponibles = await _db.Colaboradores.AsNoTracking()
                .Where(c => c.Activo || model.ColaboradoresIds.Contains(c.Id))
                .OrderBy(c => c.Nombre)
                .Select(c => new OpcionViewModel { Id = c.Id, Texto = c.Nombre + " — " + c.Cargo })
                .ToListAsync();
            return true;
        }

        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        var propio = await _db.Colaboradores.AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId && c.Activo)
            .Select(c => c.Nombre + " — " + c.Cargo)
            .FirstOrDefaultAsync();

        if (propio is null)
        {
            TempData["Error"] = "Tu usuario no está vinculado a un colaborador activo. Pide al administrador que lo vincule para poder registrar labores.";
            return false;
        }

        model.ColaboradorPropio = propio;
        return true;
    }

    private async Task<List<int>> ResolverColaboradoresAsync(LaborFormViewModel model)
    {
        if (EsAdmin)
            return model.ColaboradoresIds.Distinct().ToList();

        var mio = await MiColaboradorIdAsync();
        return mio is null ? new List<int>() : new List<int> { mio.Value };
    }

    private async Task ValidarFormularioAsync(LaborFormViewModel model, List<int> idsColaboradores, IReadOnlyCollection<int>? yaAsignados = null)
    {
        yaAsignados ??= Array.Empty<int>();

        if (model.TerrenoId is { } terrenoId)
        {
            var loteValido = await _db.Terrenos.AnyAsync(l =>
                l.Id == terrenoId && !l.Eliminado && l.Estado != EstadoTerreno.Inactivo);
            if (!loteValido)
                ModelState.AddModelError(nameof(model.TerrenoId), "La finca o el lote seleccionado no existe o está inactivo.");
        }

        if (model.Tipo is { } tipo && !Enum.IsDefined(tipo))
            ModelState.AddModelError(nameof(model.Tipo), "El tipo de labor no es válido.");

        if (model.Fecha is { } fechaLabor)
        {
            if (fechaLabor.Date < Validaciones.FechaMinima)
                ModelState.AddModelError(nameof(model.Fecha),
                    $"La fecha no puede ser anterior al {Validaciones.FechaMinima:dd/MM/yyyy}.");
            else if (fechaLabor.Date > DateTime.Today.AddYears(1))
                ModelState.AddModelError(nameof(model.Fecha), "La fecha no puede ser mayor a un año en el futuro.");
        }

        var tipoOtro = model.TipoOtro?.Trim();
        if (model.Tipo == TipoLabor.Otra && string.IsNullOrWhiteSpace(tipoOtro))
            ModelState.AddModelError(nameof(model.TipoOtro), "Escriba qué labor se realizó (por ejemplo: Siembra o Caños).");

        if (model.TerrenoId is { } idTerreno && model.Tipo is { } tipoLabor &&
            (tipoLabor != TipoLabor.Otra || !string.IsNullOrWhiteSpace(tipoOtro)))
        {
            var abiertas = _db.Labores.AsNoTracking()
                .Where(l => l.TerrenoId == idTerreno && l.Tipo == tipoLabor &&
                            l.Estado != EstadoLabor.Finalizada && l.Id != model.Id);

            if (tipoLabor == TipoLabor.Otra)
                abiertas = abiertas.Where(l => l.TipoOtro != null && l.TipoOtro.ToLower() == tipoOtro!.ToLower());

            var abierta = await abiertas
                .Select(l => new { l.Id, l.Fecha, l.Estado, Codigo = l.Terreno.Codigo })
                .FirstOrDefaultAsync();

            if (abierta is not null)
            {
                model.LaborAbiertaId = abierta.Id;
                ModelState.AddModelError(nameof(model.Tipo),
                    $"Ya existe una labor de {Ui.NombreLabor(tipoLabor, tipoOtro).ToLower()} sin finalizar en «{abierta.Codigo}» " +
                    $"(del {abierta.Fecha:dd/MM/yyyy}, {Ui.EstadoLaborTexto(abierta.Estado).ToLower()}). " +
                    "Registre los avances en esa labor o ciérrela antes de crear otra.");
            }
        }

        if (idsColaboradores.Count == 0)
        {
            ModelState.AddModelError(nameof(model.ColaboradoresIds), "Debe asignar al menos un colaborador a la labor.");
            return;
        }

        var validos = await _db.Colaboradores
            .CountAsync(c => idsColaboradores.Contains(c.Id) && (c.Activo || yaAsignados.Contains(c.Id)));
        if (validos != idsColaboradores.Count)
            ModelState.AddModelError(nameof(model.ColaboradoresIds), "Uno o más colaboradores seleccionados no existen o están inactivos.");
    }

    private async Task<string> CodigoTerrenoAsync(int terrenoId) =>
        await _db.Terrenos.Where(l => l.Id == terrenoId).Select(l => l.Codigo).FirstAsync();

    private async Task<IEnumerable<OpcionViewModel>> TerrenosAsync(bool soloDisponibles)
    {
        var query = _db.Terrenos.AsNoTracking().Where(l => !l.Eliminado);

        if (soloDisponibles)
            query = query.Where(l => l.Estado != EstadoTerreno.Inactivo);

        return await query
            .OrderBy(l => l.Codigo)
            .Select(l => new OpcionViewModel { Id = l.Id, Texto = l.Codigo + " — " + l.Nombre })
            .ToListAsync();
    }
}
