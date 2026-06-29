using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IConceptosService
{
    Task<List<SeccionConceptoResponse>> ListarSeccionesAsync(int contratoId, CancellationToken ct = default);
    Task<int> CrearSeccionAsync(int contratoId, CrearSeccionRequest dto, CancellationToken ct = default);
    Task<int> CrearConceptoAsync(int contratoId, CrearConceptoRequest dto, CancellationToken ct = default);
    Task ActualizarConceptoAsync(int conceptoId, CrearConceptoRequest dto, CancellationToken ct = default);
    Task DesactivarConceptoAsync(int conceptoId, CancellationToken ct = default);
}
