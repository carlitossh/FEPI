using Fepi.Api.Models;

namespace Fepi.Api.DTOs;

public record CrearEstimacionDto(int ContratoId, string Periodo);
public record EstimacionConceptoInputDto(int ConceptoContratoId, decimal CantidadEjecutada);
public record ActualizarConceptosEstimacionDto(List<EstimacionConceptoInputDto> Conceptos);

public record EstimacionResumenDto(int Id, int NumeroCorrelativo, string Periodo, EstadoEstimacion Estado, decimal MontoEstimado);

public record EstimacionDetalleDto(
    int Id, int ContratoId, int NumeroCorrelativo, string Periodo, EstadoEstimacion Estado,
    decimal MontoEstimado, decimal MontoContratado, decimal MontoAcumuladoContrato, decimal SaldoPendiente,
    List<EstimacionConceptoDetalleDto> Conceptos, List<string> NotasVinculadasResumen, List<ObservacionDto> Observaciones);

public record EstimacionConceptoDetalleDto(string Clave, string Descripcion, decimal CantidadEjecutada, decimal PrecioUnitario, decimal Importe);
public record ObservacionDto(int Id, string Texto, DateTime Fecha, int UsuarioId);
public record CrearObservacionDto(string Texto);
public record VincularNotasDto(List<int> NotaBitacoraIds);
public record CambiarEstadoEstimacionDto(EstadoEstimacion NuevoEstado, string? Comentario, int UsuarioId);
public record RegistrarPagoEstimacionDto(DateOnly FechaPago, string ReferenciaBancaria, decimal MontoPagado);
public record EstimacionHistorialDto(EstadoEstimacion EstadoAnterior, EstadoEstimacion EstadoNuevo, DateTime Fecha, int UsuarioId, string? Comentario);
