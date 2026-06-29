using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

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
    Task<List<PagoEstimacionDto>> ObtenerPagosAsync(int estimacionId, CancellationToken ct = default);
}
