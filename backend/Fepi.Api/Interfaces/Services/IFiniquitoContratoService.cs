using Fepi.Api.DTOs;
using Fepi.Api.Models;

namespace Fepi.Api.Interfaces;

public interface IFiniquitoContratoService
{
    Task<IniciarCierreContratoResponse> IniciarCierreContratoAsync(int contratoId, int usuarioId, CancellationToken ct = default);

    Task<int> CrearNotaCierreContratoAsync(int finiquitoId, int autorUsuarioId, CancellationToken ct = default);

    Task<bool> ValidarNotaCierreFirmadaAsync(int finiquitoId, CancellationToken ct = default);

    Task<FiniquitoContratoResponse> RegistrarFiniquitoAsync(int finiquitoId, RegistrarFiniquitoRequest dto, CancellationToken ct = default);

    Task<FiniquitoContratoResponse> AprobarFiniquitoAsync(int finiquitoId, CancellationToken ct = default);

    Task CerrarContratoAsync(int finiquitoId, CancellationToken ct = default);

    Task<FiniquitoContratoResponse?> ObtenerFiniquitoPorContratoAsync(int contratoId, CancellationToken ct = default);

    Task<FiniquitoResumenResponse> CalcularResumenFiniquitoAsync(int contratoId, CancellationToken ct = default);
}
