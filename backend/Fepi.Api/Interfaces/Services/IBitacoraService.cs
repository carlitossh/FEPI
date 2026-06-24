using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

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