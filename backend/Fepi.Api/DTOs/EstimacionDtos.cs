using Fepi.Api.Models;

namespace Fepi.Api.DTOs;

public record CrearEstimacionDto(int ContratoId, int PeriodoContratoId);
public record EstimacionConceptoInputDto(int ConceptoContratoId, decimal CantidadEjecutada);
public record ActualizarConceptosEstimacionDto(List<EstimacionConceptoInputDto> Conceptos);

public record EstimacionResumenDto(
    int Id, int NumeroCorrelativo, string Periodo,
    EstadoEstimacion Estado, EstadoPagoEstimacion EstadoPago,
    decimal MontoEstimado, decimal MontoPagadoAcumulado, decimal SaldoPendientePago);

public record EstimacionDetalleDto(
    int Id, int ContratoId, int NumeroCorrelativo, string Periodo,
    EstadoEstimacion Estado, EstadoPagoEstimacion EstadoPago,
    decimal MontoEstimado, decimal MontoContratado, decimal MontoAcumuladoContrato, decimal SaldoPendiente,
    decimal MontoPagadoAcumulado,
    DateTime? FechaAprobacionSupervision, int? UsuarioAprobacionSupervisionId,
    DateTime? FechaAprobacionResidencia, int? UsuarioAprobacionResidenciaId,
    List<EstimacionConceptoDetalleDto> Conceptos,
    List<string> NotasVinculadasResumen,
    List<ObservacionDto> Observaciones,
    List<PagoEstimacionDto> Pagos);

public record EstimacionConceptoDetalleDto(string Clave, string Descripcion, decimal CantidadEjecutada, decimal PrecioUnitario, decimal Importe);
public record ObservacionDto(int Id, string Texto, DateTime Fecha, int UsuarioId);
public record PagoEstimacionDto(int Id, DateOnly FechaPago, string ReferenciaBancaria, decimal MontoPagado, int? UsuarioRegistroId, DateTime FechaRegistro);
public record CrearObservacionDto(string Texto);
public record VincularNotasDto(List<int> NotaBitacoraIds);
public record CambiarEstadoEstimacionDto(EstadoEstimacion NuevoEstado, string? Comentario, int UsuarioId);
public record RegistrarPagoEstimacionDto(DateOnly FechaPago, string ReferenciaBancaria, decimal MontoPagado, int? UsuarioRegistroId);
public record EstimacionHistorialDto(EstadoEstimacion EstadoAnterior, EstadoEstimacion EstadoNuevo, DateTime Fecha, int UsuarioId, string? Comentario);
