using Fepi.Api.DTOs;
using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

/// <summary>SV-01 — Gestión de Estimaciones. Cubre HU-01 a HU-06.</summary>
public interface IEstimacionService
{
    Task<EstimacionResumenDto> CrearAsync(CrearEstimacionDto dto, CancellationToken ct = default);
    Task<List<EstimacionResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task<EstimacionDetalleDto> ObtenerDetalleAsync(int estimacionId, CancellationToken ct = default);
    Task ActualizarConceptosAsync(int estimacionId, ActualizarConceptosEstimacionDto dto, CancellationToken ct = default);
    Task VincularNotasBitacoraAsync(int estimacionId, VincularNotasDto dto, CancellationToken ct = default);
    Task EnviarAsync(int estimacionId, int usuarioId, CancellationToken ct = default);
    Task<ObservacionDto> AgregarObservacionAsync(int estimacionId, CrearObservacionDto dto, int usuarioId, CancellationToken ct = default);
    Task CambiarEstadoAsync(int estimacionId, CambiarEstadoEstimacionDto dto, CancellationToken ct = default);
    Task RegistrarPagoAsync(int estimacionId, RegistrarPagoEstimacionDto dto, CancellationToken ct = default);
    Task<List<EstimacionHistorialDto>> ObtenerHistorialAsync(int estimacionId, CancellationToken ct = default);
}

/// <summary>SV-02 — Control de Avance Físico y Curva S. Cubre HU-07, HU-08.</summary>
public interface IAvanceService
{
    Task RegistrarAvanceDiarioAsync(RegistrarAvanceDto dto, CancellationToken ct = default);
    Task<CurvaSDto> ObtenerCurvaSAsync(int contratoId, CancellationToken ct = default);
    Task<List<AvanceResumenContratoDto>> ObtenerResumenPorDependenciaAsync(string dependencia, CancellationToken ct = default);
}

/// <summary>SV-03 — Bitácora Electrónica. Cubre HU-09 a HU-14.</summary>
public interface IBitacoraService
{
    Task<CaratulaBitacoraDto> AbrirBitacoraAsync(AbrirBitacoraDto dto, CancellationToken ct = default);
    Task<BitacoraNotaDto> CrearNotaAsync(CrearNotaBitacoraDto dto, CancellationToken ct = default);
    Task FirmarNotaAsync(int notaId, FirmarNotaDto dto, CancellationToken ct = default);
    Task<List<BitacoraNotaDto>> BuscarNotasAsync(int BitacoraId, string? asunto, DateOnly? fechaInicio, DateOnly? fechaFin, int? actorId, CancellationToken ct = default);
    Task CrearMinutaAsync(CrearMinutaDto dto, CancellationToken ct = default);
    Task<int> CrearIncidenciaAsync(CrearIncidenciaDto dto, CancellationToken ct = default);
    Task<BitacoraNotaDto> GenerarNotaDesdeIncidenciaAsync(GenerarNotaDesdeIncidenciaDto dto, CancellationToken ct = default);
}

/// <summary>SV-04 — Convenios Modificatorios. Cubre HU-15 a HU-18.</summary>
public interface IConvenioService
{
    Task<int> SolicitarAsync(CrearConvenioDto dto, CancellationToken ct = default);
    Task<ConvenioDetalleDto> ObtenerDetalleAsync(int convenioId, CancellationToken ct = default);
    Task<List<ConvenioResumenDto>> ListarPorContratoAsync(int contratoId, CancellationToken ct = default);
    Task RevisarAsync(int convenioId, RevisarConvenioDto dto, CancellationToken ct = default);
    Task PromoverAsync(int convenioId, PromoverConvenioDto dto, CancellationToken ct = default);
    Task ResolverAsync(int convenioId, ResolverConvenioDto dto, CancellationToken ct = default);
}

/// <summary>SV-05 — Entrega-Recepción y Finiquito. Cubre HU-19, HU-20.</summary>
public interface IEntregaRecepcionService
{
    Task IniciarAsync(IniciarEntregaRecepcionDto dto, CancellationToken ct = default);
    Task<EntregaRecepcionDto> ObtenerAsync(int contratoId, CancellationToken ct = default);
    Task<FiniquitoDto> EmitirFiniquitoAsync(int contratoId, EmitirFiniquitoDto dto, CancellationToken ct = default);
    Task<FiniquitoDto> ObtenerFiniquitoAsync(int contratoId, CancellationToken ct = default);
}

/// <summary>SV-06 — Alertas y Notificaciones. Cubre HU-21. Solo panel persistente, sin correo.</summary>
public interface IAlertaService
{
    Task EmitirAsync(int contratoId, TipoAlerta tipo, int entidadId, string entidadTipo, RolSistema rolDestino, string mensaje, CancellationToken ct = default);
    Task ResolverAsync(TipoAlerta tipo, int entidadId, CancellationToken ct = default);
    Task<List<AlertaDto>> ObtenerActivasPorRolAsync(RolSistema rol, int? contratoId = null, CancellationToken ct = default);
}

/// <summary>SV-07 — Expediente Digital y Registro de Contrato. Cubre HU-22 a HU-24.</summary>
public interface IContratoService
{
    Task<int> CrearAsync(CrearContratoDto dto, CancellationToken ct = default);
    Task<ContratoDetalleDto> ObtenerDetalleAsync(int contratoId, CancellationToken ct = default);
    Task<List<ContratoResumenDto>> ListarPorDependenciaAsync(string dependencia, EstadoContrato? Estado, CancellationToken ct = default);
    Task ActualizarProgramaObraAsync(int contratoId, ActualizarProgramaObraDto dto, CancellationToken ct = default);
    Task AgregarOActualizarConceptoCatalogoAsync(int contratoId, ConceptoContratoInputDto dto, CancellationToken ct = default);
    Task ActualizarMontoContratadoAsync(int contratoId, decimal nuevoMonto, CancellationToken ct = default);
}

/// <summary>SV-08 — Administración de Actores y Roles. Solo lectura; alta vía scripts (HU-25).</summary>
public interface IUsuarioService
{
    Task<UsuarioDto?> ObtenerPorIdAsync(int usuarioId, CancellationToken ct = default);
    Task<List<UsuarioContratoDto>> ObtenerRolesYContratosAsync(int usuarioId, CancellationToken ct = default);
}

/// <summary>SV-10 — Panel Ejecutivo (Dashboard). Cubre HU-26.</summary>
public interface IDashboardService
{
    Task<DashboardContratoDto> ObtenerPorContratoAsync(int contratoId, CancellationToken ct = default);
}
