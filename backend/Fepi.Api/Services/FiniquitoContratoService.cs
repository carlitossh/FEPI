using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class FiniquitoContratoService : IFiniquitoContratoService
{
    private readonly FepiDbContext _context;
    private readonly IBitacoraNotaRepository _notaRepo;
    private readonly IAlertaUsuarioService _alertaUsuarioService;

    public FiniquitoContratoService(
        FepiDbContext context,
        IBitacoraNotaRepository notaRepo,
        IAlertaUsuarioService alertaUsuarioService)
    {
        _context = context;
        _notaRepo = notaRepo;
        _alertaUsuarioService = alertaUsuarioService;
    }

    public async Task<IniciarCierreContratoResponse> IniciarCierreContratoAsync(
        int contratoId, int usuarioId, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        if (contrato.Estado is EstadoContrato.EnCierre or EstadoContrato.Finiquitado
            or EstadoContrato.Cerrado or EstadoContrato.Cancelado)
            throw new InvalidOperationException("El contrato ya se encuentra en proceso de cierre o finalizado.");

        var tieneEstimacionesPendientes = await _context.Estimaciones.AnyAsync(e =>
            e.ContratoId == contratoId &&
            (e.Estado == EstadoEstimacion.Borrador ||
             e.Estado == EstadoEstimacion.Enviada ||
             e.Estado == EstadoEstimacion.Observada ||
             e.Estado == EstadoEstimacion.Rechazada), ct);

        if (tieneEstimacionesPendientes)
            throw new InvalidOperationException(
                "Existen estimaciones pendientes de finalizar. Corrija antes de iniciar el cierre.");

        var tieneConveniosPendientes = await _context.ConveniosModificatorios.AnyAsync(c =>
            c.ContratoId == contratoId &&
            (c.Estado == EstadoConvenio.Revisado ||
             c.Estado == EstadoConvenio.AprobadoSupervision ||
             c.Estado == EstadoConvenio.AprobadoResidencia), ct);

        if (tieneConveniosPendientes)
            throw new InvalidOperationException(
                "Existen convenios modificatorios pendientes de aplicar. Corrija antes de iniciar el cierre.");

        var existeFiniquito = await _context.FiniquitosContrato
            .AnyAsync(f => f.ContratoId == contratoId, ct);

        if (existeFiniquito)
            throw new InvalidOperationException("Ya existe un proceso de finiquito para este contrato.");

        contrato.Estado = EstadoContrato.EnCierre;

        var finiquito = new FiniquitoContrato
        {
            ContratoId = contratoId,
            FechaInicioCierre = DateTime.UtcNow,
            ImporteContratoOriginal = contrato.ImporteTotal,
            Estado = EstadoFiniquitoContrato.PendienteNotaCierre,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacion = DateTime.UtcNow
        };

        _context.FiniquitosContrato.Add(finiquito);
        await _context.SaveChangesAsync(ct);

        // Crear nota de cierre automáticamente
        await CrearNotaCierreInterna(finiquito, contrato, usuarioId, ct);

        await _alertaUsuarioService.CrearAlertasSegunEventoAsync(
            TipoAlerta.ContratoPorCerrar, contratoId, finiquito.Id, ct);

        return new IniciarCierreContratoResponse(finiquito.Id, contratoId, finiquito.Estado);
    }

    public async Task<int> CrearNotaCierreContratoAsync(
        int finiquitoId, int autorUsuarioId, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .FirstOrDefaultAsync(f => f.Id == finiquitoId, ct)
            ?? throw new InvalidOperationException("Finiquito no encontrado.");

        if (finiquito.BitacoraNotaCierreId.HasValue)
            throw new InvalidOperationException("Ya existe una nota de cierre para este finiquito.");

        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == finiquito.ContratoId, ct)!;

        return await CrearNotaCierreInterna(finiquito, contrato!, autorUsuarioId, ct);
    }

    private async Task<int> CrearNotaCierreInterna(
        FiniquitoContrato finiquito, Contrato contrato, int autorUsuarioId, CancellationToken ct)
    {
        var bitacora = await _context.Bitacoras
            .FirstOrDefaultAsync(b => b.ContratoId == finiquito.ContratoId, ct)
            ?? throw new InvalidOperationException("Bitácora no encontrada para este contrato.");

        var ultimoFolio = await _context.BitacoraNotas
            .Where(n => n.BitacoraId == bitacora.Id)
            .MaxAsync(n => (int?)n.Folio, ct) ?? 0;

        var nota = new BitacoraNota
        {
            BitacoraId = bitacora.Id,
            Folio = ultimoFolio + 1,
            TipoRegistro = TipoNotaBitacora.CierreContrato,
            AutorUsuarioId = autorUsuarioId,
            Asunto = "Cierre y terminación del contrato",
            Contenido = $"Nota de cierre del contrato {contrato.NumeroContrato}. " +
                        "Se hace constar que el contrato ha sido ejecutado en su totalidad " +
                        "conforme a las especificaciones y términos acordados.",
            FechaRegistro = DateTime.UtcNow,
            CantidadFirmasRequeridas = 3,
            EstadoFirma = EstadoFirmaNota.Pendiente
        };

        nota.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = autorUsuarioId,
            RolFirmante = RolSistema.Residencia,
            EsEmisor = true,
            OrdenFirma = 1,
            Firmado = false
        });

        nota.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = autorUsuarioId,
            RolFirmante = RolSistema.SupervisorExterno,
            EsEmisor = false,
            OrdenFirma = 2,
            Firmado = false
        });

        nota.Firmas.Add(new BitacoraFirma
        {
            UsuarioId = autorUsuarioId,
            RolFirmante = RolSistema.Superintendente,
            EsEmisor = false,
            OrdenFirma = 3,
            Firmado = false
        });

        _context.BitacoraNotas.Add(nota);
        await _context.SaveChangesAsync(ct);

        finiquito.BitacoraNotaCierreId = nota.Id;
        await _context.SaveChangesAsync(ct);

        await _alertaUsuarioService.CrearAlertasSegunEventoAsync(
            TipoAlerta.BitacoraFirmaPendiente, finiquito.ContratoId, nota.Id, ct);

        return nota.Id;
    }

    public async Task<bool> ValidarNotaCierreFirmadaAsync(int finiquitoId, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre)
                .ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.Id == finiquitoId, ct)
            ?? throw new InvalidOperationException("Finiquito no encontrado.");

        if (finiquito.BitacoraNotaCierre is null) return false;

        var estaFirmada = finiquito.BitacoraNotaCierre.Cerrada;

        if (estaFirmada && finiquito.Estado == EstadoFiniquitoContrato.PendienteNotaCierre)
        {
            finiquito.Estado = EstadoFiniquitoContrato.NotaCierreFirmada;
            await _context.SaveChangesAsync(ct);

            await _alertaUsuarioService.CrearAlertasSegunEventoAsync(
                TipoAlerta.FiniquitoPendiente, finiquito.ContratoId, finiquito.Id, ct);
        }

        return estaFirmada;
    }

    public async Task<FiniquitoContratoResponse> RegistrarFiniquitoAsync(
        int finiquitoId, RegistrarFiniquitoRequest dto, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre)
                .ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.Id == finiquitoId, ct)
            ?? throw new InvalidOperationException("Finiquito no encontrado.");

        if (finiquito.BitacoraNotaCierre is null)
            throw new InvalidOperationException("No existe nota de cierre. Cree la nota antes de registrar el finiquito.");

        if (!finiquito.BitacoraNotaCierre.Cerrada)
            throw new InvalidOperationException(
                "La nota de cierre aún no está firmada por todos los participantes requeridos.");

        if (finiquito.Estado is EstadoFiniquitoContrato.Registrado or EstadoFiniquitoContrato.Aprobado)
            throw new InvalidOperationException("El finiquito ya fue registrado.");

        var resumen = await CalcularResumenFiniquitoAsync(finiquito.ContratoId, ct);

        finiquito.ImporteConvenios = resumen.ImporteConvenios;
        finiquito.ImporteContratoFinal = resumen.ImporteContratoFinal;
        finiquito.ImporteEstimadoTotal = resumen.ImporteEstimadoTotal;
        finiquito.ImportePagadoTotal = resumen.ImportePagadoTotal;
        finiquito.SaldoPendiente = resumen.SaldoPendiente;

        finiquito.Deductivas = dto.Deductivas;
        finiquito.Retenciones = dto.Retenciones;
        finiquito.PenasConvencionales = dto.PenasConvencionales;
        finiquito.ImporteFinalAFiniquitar = finiquito.SaldoPendiente - dto.Deductivas - dto.Retenciones - dto.PenasConvencionales;
        finiquito.FechaFiniquito = dto.FechaFiniquito;
        finiquito.Observaciones = dto.Observaciones;
        finiquito.Estado = EstadoFiniquitoContrato.Registrado;

        var contrato = await _context.Contratos.FirstAsync(c => c.Id == finiquito.ContratoId, ct);
        contrato.Estado = EstadoContrato.Finiquitado;

        await _context.SaveChangesAsync(ct);

        return MapResponse(finiquito, true);
    }

    public async Task<FiniquitoContratoResponse> AprobarFiniquitoAsync(int finiquitoId, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre).ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.Id == finiquitoId, ct)
            ?? throw new InvalidOperationException("Finiquito no encontrado.");

        if (finiquito.Estado != EstadoFiniquitoContrato.Registrado)
            throw new InvalidOperationException("El finiquito debe estar en estado Registrado para poder aprobarlo.");

        finiquito.Estado = EstadoFiniquitoContrato.Aprobado;
        await _context.SaveChangesAsync(ct);

        return MapResponse(finiquito, finiquito.BitacoraNotaCierre?.Cerrada ?? false);
    }

    public async Task CerrarContratoAsync(int finiquitoId, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .FirstOrDefaultAsync(f => f.Id == finiquitoId, ct)
            ?? throw new InvalidOperationException("Finiquito no encontrado.");

        if (finiquito.Estado != EstadoFiniquitoContrato.Aprobado)
            throw new InvalidOperationException("El finiquito debe estar aprobado para cerrar el contrato.");

        var contrato = await _context.Contratos.FirstAsync(c => c.Id == finiquito.ContratoId, ct);
        contrato.Estado = EstadoContrato.Cerrado;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<FiniquitoContratoResponse?> ObtenerFiniquitoPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var finiquito = await _context.FiniquitosContrato
            .Include(f => f.BitacoraNotaCierre)
                .ThenInclude(n => n!.Firmas)
            .FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

        if (finiquito is null) return null;

        // Sincronizar estado si la nota está firmada
        if (finiquito.Estado == EstadoFiniquitoContrato.PendienteNotaCierre
            && finiquito.BitacoraNotaCierre?.Cerrada == true)
        {
            finiquito.Estado = EstadoFiniquitoContrato.NotaCierreFirmada;
            await _context.SaveChangesAsync(ct);

            await _alertaUsuarioService.CrearAlertasSegunEventoAsync(
                TipoAlerta.FiniquitoPendiente, contratoId, finiquito.Id, ct);
        }

        return MapResponse(finiquito, finiquito.BitacoraNotaCierre?.Cerrada ?? false);
    }

    public async Task<FiniquitoResumenResponse> CalcularResumenFiniquitoAsync(int contratoId, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == contratoId, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var finiquito = await _context.FiniquitosContrato
            .FirstOrDefaultAsync(f => f.ContratoId == contratoId, ct);

        var importeOriginal = finiquito?.ImporteContratoOriginal ?? contrato.ImporteTotal;

        var importeConvenios = await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == contratoId && c.Estado == EstadoConvenio.Aplicado)
            .SumAsync(c => c.MontoSolicitado ?? 0m, ct);

        var importeEstimadoTotal = await _context.EstimacionConceptos
            .Where(ec => ec.Estimacion != null &&
                         ec.Estimacion.ContratoId == contratoId &&
                         (ec.Estimacion.Estado == EstadoEstimacion.AprobadaResidencia ||
                          ec.Estimacion.Estado == EstadoEstimacion.Pagada))
            .SumAsync(ec => ec.ImporteTotal, ct);

        var importePagadoTotal = await _context.EstimacionConceptos
            .Where(ec => ec.Estimacion != null &&
                         ec.Estimacion.ContratoId == contratoId &&
                         ec.Estimacion.Estado == EstadoEstimacion.Pagada)
            .SumAsync(ec => ec.ImporteTotal, ct);

        var importeContratoFinal = contrato.ImporteTotal;
        var saldoPendiente = importeContratoFinal - importePagadoTotal;

        return new FiniquitoResumenResponse(
            importeOriginal,
            importeConvenios,
            importeContratoFinal,
            importeEstimadoTotal,
            importePagadoTotal,
            saldoPendiente
        );
    }

    private static FiniquitoContratoResponse MapResponse(FiniquitoContrato f, bool notaCierreFirmada) => new(
        f.Id,
        f.ContratoId,
        f.BitacoraNotaCierreId,
        f.FechaInicioCierre,
        f.FechaFiniquito,
        f.ImporteContratoOriginal,
        f.ImporteConvenios,
        f.ImporteContratoFinal,
        f.ImporteEstimadoTotal,
        f.ImportePagadoTotal,
        f.SaldoPendiente,
        f.Deductivas,
        f.Retenciones,
        f.PenasConvencionales,
        f.ImporteFinalAFiniquitar,
        f.Observaciones,
        f.Estado,
        f.CreadoPorUsuarioId,
        f.FechaCreacion,
        notaCierreFirmada
    );
}
