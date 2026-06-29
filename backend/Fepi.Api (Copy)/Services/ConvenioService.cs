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

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            throw new InvalidOperationException("La descripción del convenio es obligatoria.");

        var variacionPrevia = await _context.ConveniosModificatorios
            .Where(c => c.ContratoId == dto.ContratoId && c.Aplicado)
            .SumAsync(c => c.VariacionAcumuladaPorcentaje, ct);

        var variacionNueva = variacionPrevia;
        if ((dto.Tipo == TipoConvenio.ModificacionMonto || dto.Tipo == TipoConvenio.ConceptosExtraordinarios)
            && dto.MontoSolicitado.HasValue && contrato.ImporteTotal > 0)
            variacionNueva += (dto.MontoSolicitado.Value / contrato.ImporteTotal) * 100;

        var numeroConvenio = dto.NumeroConvenio
            ?? $"CV-{dto.ContratoId:D4}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var convenio = new ConvenioModificatorio
        {
            ContratoId = dto.ContratoId,
            NumeroConvenio = numeroConvenio,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Justificacion = dto.Justificacion,
            Observaciones = dto.Observaciones,
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

        _context.ConvenioHistoriales.Add(new ConvenioHistorial
        {
            ConvenioModificatorioId = convenio.Id,
            Accion = "created",
            EstadoNuevo = EstadoConvenio.Revisado,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.SolicitanteId
        });

        await _context.SaveChangesAsync(ct);

        await _alertaService.EmitirAsync(dto.ContratoId, TipoAlerta.ConvenioPendienteResolucion, convenio.Id,
            nameof(ConvenioModificatorio), RolSistema.SupervisorExterno, "Nueva solicitud de convenio modificatorio pendiente de revisión.", ct);

        return convenio.Id;
    }

    public async Task<ConvenioDetalleDto> ObtenerDetalleAsync(int convenioId, CancellationToken ct = default)
    {
        var c = await _convenioRepo.GetConDetalleAsync(convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        var historial = await _context.ConvenioHistoriales
            .Where(h => h.ConvenioModificatorioId == convenioId)
            .OrderBy(h => h.Fecha)
            .Select(h => new ConvenioHistorialDto(h.Id, h.Accion, h.EstadoNuevo, h.Fecha, h.UsuarioId, h.Comentario))
            .ToListAsync(ct);

        return new ConvenioDetalleDto(
            c.Id, c.ContratoId, c.NumeroConvenio, c.Tipo, c.Descripcion, c.Justificacion, c.Observaciones,
            c.Estado, c.MontoSolicitado, c.PlazoDiasSolicitado, c.VariacionAcumuladaPorcentaje,
            c.FechaEmision, c.FechaAutorizacion, c.Aplicado, c.FechaAplicacion,
            c.RevisionSupervision is null ? null : new RevisionSupervisionDto(c.RevisionSupervision.Decision, c.RevisionSupervision.Justificacion, c.RevisionSupervision.SupervisorId, c.RevisionSupervision.Fecha),
            c.PromocionResidencia is null ? null : new PromocionResidenciaDto(c.PromocionResidencia.ResidenteId, c.PromocionResidencia.Fecha),
            c.ResolucionDependencia is null ? null : new ResolucionDependenciaDto(c.ResolucionDependencia.Aprobado, c.ResolucionDependencia.MotivoRechazo, c.ResolucionDependencia.UsuarioDependenciaId, c.ResolucionDependencia.Fecha),
            historial);
    }

    public async Task<List<ConvenioResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default)
    {
        var convenios = await _convenioRepo.GetByContratoAsync(contratoId, ct);
        return convenios.Select(c => new ConvenioResumenDto(
            c.Id, c.NumeroConvenio, c.Tipo, c.Estado, c.MontoSolicitado,
            c.PlazoDiasSolicitado, c.VariacionAcumuladaPorcentaje, c.FechaEmision, c.Aplicado)).ToList();
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

        _context.ConvenioHistoriales.Add(new ConvenioHistorial
        {
            ConvenioModificatorioId = convenioId,
            Accion = dto.Decision == DictamenTecnico.Procedente ? "reviewed" : "observed",
            EstadoNuevo = EstadoConvenio.AprobadoSupervision,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.SupervisorId
        });

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

        _context.ConvenioHistoriales.Add(new ConvenioHistorial
        {
            ConvenioModificatorioId = convenioId,
            Accion = "promoted",
            EstadoNuevo = EstadoConvenio.AprobadoResidencia,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.ResidenteId
        });

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

        var nuevoEstado = dto.Aprobado ? EstadoConvenio.Aplicado : EstadoConvenio.Revisado;
        convenio.Estado = nuevoEstado;

        if (dto.Aprobado)
            convenio.FechaAutorizacion = DateTime.UtcNow;

        _context.ConvenioHistoriales.Add(new ConvenioHistorial
        {
            ConvenioModificatorioId = convenioId,
            Accion = dto.Aprobado ? "approved" : "rejected",
            EstadoNuevo = nuevoEstado,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.UsuarioDependenciaId,
            Comentario = dto.MotivoRechazo
        });

        await _context.SaveChangesAsync(ct);
        await _alertaService.ResolverAsync(TipoAlerta.ConvenioPendienteResolucion, convenioId, ct);
    }

    public async Task<AplicarConvenioResultadoDto> AplicarAsync(int convenioId, AplicarConvenioDto dto, CancellationToken ct = default)
    {
        var convenio = await _context.ConveniosModificatorios
            .Include(cv => cv.Cambios)
            .FirstOrDefaultAsync(cv => cv.Id == convenioId, ct)
            ?? throw new InvalidOperationException("Convenio no encontrado.");

        if (convenio.Estado != EstadoConvenio.Aplicado)
            throw new InvalidOperationException("Solo se puede aplicar un convenio en estado Aprobado (Aplicado).");

        if (convenio.Aplicado)
            throw new InvalidOperationException("Este convenio ya fue aplicado anteriormente.");

        var contrato = await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == convenio.ContratoId, ct)
            ?? throw new InvalidOperationException("Contrato del convenio no encontrado.");

        // Aplicar cambios según tipo
        switch (convenio.Tipo)
        {
            case TipoConvenio.AmpliacionPlazo:
                await AplicarExtensionPlazo(convenio, contrato, ct);
                break;

            case TipoConvenio.ConceptosExtraordinarios:
                await AplicarAdicionConceptos(convenio, contrato, ct);
                break;

            case TipoConvenio.ModificacionMonto:
                await AplicarModificacionMonto(convenio, contrato, ct);
                break;

            case TipoConvenio.ModificacionCantidades:
                await AplicarModificacionCantidades(convenio, contrato, ct);
                break;

            case TipoConvenio.AjusteFechas:
                await AplicarExtensionPlazo(convenio, contrato, ct);
                break;
        }

        // Recalcular IVA e importe total
        var iva = Math.Round(contrato.ImporteSinIVA * contrato.IvaPorcentaje / 100m, 2);
        contrato.IVA = iva;
        contrato.ImporteTotal = contrato.ImporteSinIVA + iva;
        contrato.MontoAnticipo = Math.Round(contrato.ImporteTotal * contrato.PorcentajeAnticipo / 100m, 2);

        // Verificar cuadre de conceptos
        var sumaConceptosActivos = await _context.ConceptosContrato
            .Where(cc => cc.ContratoId == contrato.Id && cc.Activo)
            .SumAsync(cc => cc.CantidadContratada * cc.PrecioUnitario, ct);

        var cuadra = Math.Abs(sumaConceptosActivos - contrato.ImporteSinIVA) <= 0.01m;

        convenio.Aplicado = true;
        convenio.FechaAplicacion = DateTime.UtcNow;

        _context.ConvenioHistoriales.Add(new ConvenioHistorial
        {
            ConvenioModificatorioId = convenioId,
            Accion = "changes_applied",
            EstadoNuevo = EstadoConvenio.Aplicado,
            Fecha = DateTime.UtcNow,
            UsuarioId = dto.UsuarioId
        });

        await _context.SaveChangesAsync(ct);

        return new AplicarConvenioResultadoDto(
            convenioId,
            contrato.Id,
            contrato.ImporteSinIVA,
            contrato.IVA,
            contrato.ImporteTotal,
            contrato.FechaTermino,
            cuadra);
    }

    public async Task<List<ConvenioHistorialDto>> ObtenerHistorialAsync(int convenioId, CancellationToken ct = default)
    {
        return await _context.ConvenioHistoriales
            .Where(h => h.ConvenioModificatorioId == convenioId)
            .OrderBy(h => h.Fecha)
            .Select(h => new ConvenioHistorialDto(h.Id, h.Accion, h.EstadoNuevo, h.Fecha, h.UsuarioId, h.Comentario))
            .ToListAsync(ct);
    }

    private async Task AplicarExtensionPlazo(ConvenioModificatorio convenio, Contrato contrato, CancellationToken ct)
    {
        if (convenio.PlazoDiasSolicitado.HasValue && convenio.PlazoDiasSolicitado.Value > 0)
        {
            contrato.FechaTermino = contrato.FechaTermino.AddDays(convenio.PlazoDiasSolicitado.Value);

            _context.ConvenioCambios.Add(new ConvenioCambio
            {
                ConvenioModificatorioId = convenio.Id,
                TipoCambio = "ExtensionContrato",
                EntidadAfectada = "Contrato",
                EntidadId = contrato.Id,
                CampoAfectado = "FechaTermino",
                ValorAnterior = contrato.FechaTermino.AddDays(-convenio.PlazoDiasSolicitado.Value).ToString("yyyy-MM-dd"),
                ValorNuevo = contrato.FechaTermino.ToString("yyyy-MM-dd"),
                DescripcionCambio = $"Extensión de {convenio.PlazoDiasSolicitado.Value} días"
            });
        }
    }

    private async Task AplicarAdicionConceptos(ConvenioModificatorio convenio, Contrato contrato, CancellationToken ct)
    {
        if (convenio.MontoSolicitado.HasValue)
        {
            var valorAnterior = contrato.ImporteSinIVA;
            contrato.ImporteSinIVA += convenio.MontoSolicitado.Value;

            _context.ConvenioCambios.Add(new ConvenioCambio
            {
                ConvenioModificatorioId = convenio.Id,
                TipoCambio = "AdicionConcepto",
                EntidadAfectada = "Contrato",
                EntidadId = contrato.Id,
                CampoAfectado = "ImporteSinIVA",
                ValorAnterior = valorAnterior.ToString("F2"),
                ValorNuevo = contrato.ImporteSinIVA.ToString("F2"),
                DescripcionCambio = "Adición de conceptos extraordinarios"
            });
        }
    }

    private async Task AplicarModificacionMonto(ConvenioModificatorio convenio, Contrato contrato, CancellationToken ct)
    {
        if (convenio.MontoSolicitado.HasValue)
        {
            var valorAnterior = contrato.ImporteSinIVA;
            contrato.ImporteSinIVA += convenio.MontoSolicitado.Value;

            _context.ConvenioCambios.Add(new ConvenioCambio
            {
                ConvenioModificatorioId = convenio.Id,
                TipoCambio = "CambioPrecioConcepto",
                EntidadAfectada = "Contrato",
                EntidadId = contrato.Id,
                CampoAfectado = "ImporteSinIVA",
                ValorAnterior = valorAnterior.ToString("F2"),
                ValorNuevo = contrato.ImporteSinIVA.ToString("F2"),
                DescripcionCambio = "Modificación de monto contractual"
            });
        }
    }

    private async Task AplicarModificacionCantidades(ConvenioModificatorio convenio, Contrato contrato, CancellationToken ct)
    {
        // Los cambios de cantidades se registran via ConvenioCambios previamente cargados
        // Aquí procesamos los cambios que fueron pre-registrados en el convenio
        foreach (var cambio in convenio.Cambios.Where(c => c.TipoCambio == "CambioCantidadConcepto" || c.TipoCambio == "CambioPrecioConcepto"))
        {
            if (cambio.EntidadId.HasValue)
            {
                var concepto = await _context.ConceptosContrato.FindAsync(new object[] { cambio.EntidadId.Value }, ct);
                if (concepto is null) continue;

                if (cambio.CampoAfectado == "CantidadContratada" && decimal.TryParse(cambio.ValorNuevo, out var nuevaCantidad))
                    concepto.CantidadContratada = nuevaCantidad;
                else if (cambio.CampoAfectado == "PrecioUnitario" && decimal.TryParse(cambio.ValorNuevo, out var nuevoPrecio))
                    concepto.PrecioUnitario = nuevoPrecio;
            }
        }

        // Recalcular ImporteSinIVA desde suma de conceptos activos
        var suma = await _context.ConceptosContrato
            .Where(cc => cc.ContratoId == contrato.Id && cc.Activo)
            .SumAsync(cc => cc.CantidadContratada * cc.PrecioUnitario, ct);

        contrato.ImporteSinIVA = suma;
    }
}
