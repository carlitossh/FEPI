using Fepi.Api.Data;
using Fepi.Api.DTOs;
using Fepi.Api.Interfaces;
using Fepi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fepi.Api.Services;

public class ConvenioService : IConvenioService
{
    private readonly IConvenioRepository _convenioRepo;
    private readonly FepiDbContext _context;
    private readonly IAlertaService _alertaService;
    private readonly IContratoService _contratoService;

    public ConvenioService(IConvenioRepository convenioRepo, FepiDbContext context, IAlertaService alertaService, IContratoService contratoService)
    {
        _convenioRepo = convenioRepo;
        _context = context;
        _alertaService = alertaService;
        _contratoService = contratoService;
    }

    public async Task<int> SolicitarAsync(CrearConvenioDto dto, CancellationToken ct = default)
    {
        var contrato = await _context.Contratos.FindAsync(new object[] { dto.ContratoId }, ct)
            ?? throw new InvalidOperationException("Contrato no encontrado.");

        var variacionPrevia = await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == dto.ContratoId && c.Estado == EstadoConvenio.Aplicado)
            .SumAsync(c => c.VariacionAcumuladaPorcentaje, ct);

        var variacionNueva = variacionPrevia;
        if (dto.Tipo == TipoConvenio.ModificacionMonto && dto.MontoSolicitado.HasValue && contrato.ImporteTotal > 0)
            variacionNueva += (dto.MontoSolicitado.Value / contrato.ImporteTotal) * 100;

        var convenio = new ConvenioModificatorio
        {
            ContratoId = dto.ContratoId,
            Tipo = dto.Tipo,
            Justificacion = dto.Justificacion,
            Estado = EstadoConvenio.Revisado,
            MontoSolicitado = dto.MontoSolicitado,
            PlazoDiasSolicitado = dto.PlazoDiasSolicitado,
            VariacionAcumuladaPorcentaje = Math.Round(variacionNueva, 2),
            SolicitanteId = dto.SolicitanteId
        };

        foreach (var url in dto.UrlsDocumentos)
            convenio.Documentos.Add(new ConvenioDocumento { Nombre = Path.GetFileName(url), UrlArchivo = url });

        await _convenioRepo.AddAsync(convenio, ct);
        await _convenioRepo.SaveChangesAsync(ct);

        await _alertaService.EmitirAsync(dto.ContratoId, TipoAlerta.ConvenioPendienteResolucion, convenio.Id,
            nameof(ConvenioModificatorio), RolSistema.SupervisorExterno, "Nueva solicitud de convenio modificatorio pendiente de revisión.", ct);

        return convenio.Id;
    }

    public async Task<ConvenioDetalleDto> ObtenerDetalleAsync(int convenioId, CancellationToken ct = default)
    {
        var c = await _convenioRepo.GetConDetalleAsync(convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        return new ConvenioDetalleDto(c.Id, c.ContratoId, c.Tipo, c.Justificacion, c.Estado, c.MontoSolicitado,
            c.PlazoDiasSolicitado, c.VariacionAcumuladaPorcentaje,
            c.RevisionSupervision is null ? null : new RevisionSupervisionDto(c.RevisionSupervision.Decision, c.RevisionSupervision.Justificacion, c.RevisionSupervision.SupervisorId, c.RevisionSupervision.Fecha),
            c.PromocionResidencia is null ? null : new PromocionResidenciaDto(c.PromocionResidencia.ResidenteId, c.PromocionResidencia.Fecha),
            c.ResolucionDependencia is null ? null : new ResolucionDependenciaDto(c.ResolucionDependencia.Aprobado, c.ResolucionDependencia.MotivoRechazo, c.ResolucionDependencia.UsuarioDependenciaId, c.ResolucionDependencia.Fecha));
    }

    public async Task<List<ConvenioResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var convenios = await _convenioRepo.GetByContratoAsync(contratoId, ct);
        return convenios.Select(c => new ConvenioResumenDto(c.Id, c.Tipo, c.Estado, c.MontoSolicitado, c.VariacionAcumuladaPorcentaje)).ToList();
    }

    public async Task RevisarAsync(int convenioId, RevisarConvenioDto dto, CancellationToken ct = default)
    {
        var convenio = await _convenioRepo.GetByIdAsync(convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        _context.ConvenioRevisionesSupervision.Add(new ConvenioRevisionSupervision
        {
            ConvenioModificatorioId = convenioId,
            Decision = dto.Decision,
            Justificacion = dto.Justificacion,
            SupervisorId = dto.SupervisorId
        });

        convenio.Estado = EstadoConvenio.AprobadoSupervision;
        await _context.SaveChangesAsync(ct);
    }

    public async Task PromoverAsync(int convenioId, PromoverConvenioDto dto, CancellationToken ct = default)
    {
        var convenio = await _convenioRepo.GetByIdAsync(convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        var tieneRevision = await _context.ConvenioRevisionesSupervision.AnyAsync(r => r.ConvenioModificatorioId == convenioId, ct);
        if (!tieneRevision)
            throw new InvalidOperationException("No se puede promover: la solicitud no tiene revisión de supervisión.");

        _context.ConvenioPromocionesResidencia.Add(new ConvenioPromocionResidencia
        {
            ConvenioModificatorioId = convenioId,
            ResidenteId = dto.ResidenteId
        });

        convenio.Estado = EstadoConvenio.AprobadoResidencia;
        await _context.SaveChangesAsync(ct);

        await _alertaService.EmitirAsync(convenio.ContratoId, TipoAlerta.ConvenioPendienteResolucion, convenioId,
            nameof(ConvenioModificatorio), RolSistema.Dependencia, "Convenio promovido — pendiente de resolución de Dependencia.", ct);
    }

    public async Task ResolverAsync(int convenioId, ResolverConvenioDto dto, CancellationToken ct = default)
    {
        var convenio = await _convenioRepo.GetByIdAsync(convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        _context.ConvenioResolucionesDependencia.Add(new ConvenioResolucionDependencia
        {
            ConvenioModificatorioId = convenioId,
            Aprobado = dto.Aprobado,
            MotivoRechazo = dto.MotivoRechazo,
            UsuarioDependenciaId = dto.UsuarioDependenciaId
        });

        // Cuando no es aprobado vuelve a estado Revisado para nueva iteración
        convenio.Estado = dto.Aprobado ? EstadoConvenio.Aplicado : EstadoConvenio.Revisado;
        await _context.SaveChangesAsync(ct);

        if (dto.Aprobado && convenio.Tipo == TipoConvenio.ModificacionMonto && convenio.MontoSolicitado.HasValue)
        {
            var contrato = await _context.Contratos.FindAsync(new object[] { convenio.ContratoId }, ct)!;
            await _contratoService.ActualizarMontoContratadoAsync(
                convenio.ContratoId, contrato!.ImporteTotal + convenio.MontoSolicitado.Value, ct);
        }

        await _alertaService.ResolverAsync(TipoAlerta.ConvenioPendienteResolucion, convenioId, ct);
    }
}
