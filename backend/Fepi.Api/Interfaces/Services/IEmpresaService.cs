using Fepi.Api.DTOs;

namespace Fepi.Api.Interfaces;

public interface IEmpresaService
{
    Task<List<EmpresaResponse>> ListarAsync(CancellationToken ct = default);
    Task<EmpresaResponse> ObtenerAsync(int id, CancellationToken ct = default);
    Task<int> CrearAsync(CrearEmpresaRequest dto, CancellationToken ct = default);
    Task ActualizarAsync(int id, CrearEmpresaRequest dto, CancellationToken ct = default);
    Task ActualizarRepresentanteAsync(int empresaId, int usuarioId, CancellationToken ct = default);
}
