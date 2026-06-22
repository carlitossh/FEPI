using Fepi.Api.Models;

namespace Fepi.Api.DTOs;

// SV-02
public record RegistrarAvanceDto(int ContratoId, DateOnly Fecha, int ConceptoContratoId, decimal CantidadEjecutada, string DescripcionActividad, int ActorId, List<string> UrlsEvidencia);
public record CurvaSPuntoDto(string Periodo, decimal PorcentajeProgramado, decimal PorcentajeReal);
public record CurvaSDto(List<CurvaSPuntoDto> Puntos, decimal PorcentajeCumplimientoActual);
public record AvanceResumenContratoDto(int ContratoId, string NumeroContrato, decimal AvanceFisicoPct, decimal AvanceFinancieroPct, decimal DesviacionPct);

// SV-03
public record AbrirBitacoraDto(int ContratoId, string NombreContrato, string NumeroContrato, string TipoContrato,
    string DependenciaContratante, string ContratistaEmpresa, string ResidenteNombre,
    string SupervisorDesignadoNombre, string SuperintendenteNombre, DateOnly FechaAperturaFormal);
public record CaratulaBitacoraDto(int Id, string FolioBitacora, bool Abierta);
public record CrearNotaBitacoraDto(int BitacoraId, int TipoNotaCatalogoId, string Asunto, string Contenido,
    int? FolioVinculadoId, int UsuarioEmisorId, RolSistema RolEmisor);
public record FirmarNotaDto(int UsuarioId, RolSistema Rol);
public record BitacoraNotaDto(int Id, int Folio, TipoNotaBitacora TipoRegistro, string Asunto, string Contenido, DateTime FechaRegistro, bool Cerrada, List<FirmaDto> Firmas);
public record FirmaDto(int UsuarioId, RolSistema Rol, bool EsEmisor, bool Firmado, DateTime? FechaFirma);
public record CrearMinutaDto(int BitacoraId, DateOnly Fecha, string Lugar, string ContenidoAcuerdos, List<string> Participantes);
public record CrearIncidenciaDto(int BitacoraId, DateOnly FechaEvento, string Descripcion, string UrlFotografia, int ActorRegistroId);
public record GenerarNotaDesdeIncidenciaDto(int IncidenciaId, int TipoNotaCatalogoId, int UsuarioEmisorId, RolSistema RolEmisor);

// SV-04
public record CrearConvenioDto(int ContratoId, TipoModificacionConvenio Tipo, string Justificacion,
    decimal? MontoSolicitado, int? PlazoDiasSolicitado, int SolicitanteId, List<string> UrlsDocumentos);
public record ConvenioResumenDto(int Id, TipoModificacionConvenio Tipo, EstadoConvenio Estado, decimal? MontoSolicitado, decimal VariacionAcumuladaPorcentaje);
public record ConvenioDetalleDto(int Id, int ContratoId, TipoModificacionConvenio Tipo, string Justificacion, EstadoConvenio Estado,
    decimal? MontoSolicitado, int? PlazoDiasSolicitado, decimal VariacionAcumuladaPorcentaje,
    RevisionSupervisionDto? Revision, PromocionResidenciaDto? Promocion, ResolucionDependenciaDto? Resolucion);
public record RevisionSupervisionDto(DictamenTecnico Decision, string Justificacion, int SupervisorId, DateTime Fecha);
public record PromocionResidenciaDto(int ResidenteId, DateTime Fecha);
public record ResolucionDependenciaDto(bool Aprobado, string? MotivoRechazo, int UsuarioDependenciaId, DateTime Fecha);
public record RevisarConvenioDto(DictamenTecnico Decision, string Justificacion, int SupervisorId);
public record PromoverConvenioDto(int ResidenteId);
public record ResolverConvenioDto(bool Aprobado, string? MotivoRechazo, int UsuarioDependenciaId);

// SV-05
public record IniciarEntregaRecepcionDto(int ContratoId, DateOnly FechaEntrega, string EstadoObraDescripcion, string EstadoGarantiasDescripcion, List<string> UrlsEvidencia);
public record EntregaRecepcionDto(int Id, DateOnly FechaEntrega, string EstadoObraDescripcion);
public record EmitirFiniquitoDto(decimal TotalDeductivas, decimal TotalRetenciones);
public record FiniquitoDto(int Id, decimal TotalPagado, decimal TotalPendiente, decimal TotalDeductivas, decimal TotalRetenciones, decimal MontoFinal, string? UrlReporteFiniquito);

// SV-06
public record AlertaDto(int Id, TipoAlerta Tipo, string Mensaje, DateTime FechaGeneracion);

// SV-07
public record ConceptoContratoInputDto(string Clave, string Descripcion, string UnidadMedida, decimal CantidadContratada, decimal PrecioUnitario);
public record ProgramaObraInputDto(string Periodo, decimal PorcentajeProgramado, decimal MontoProgramado);
public record GarantiaInputDto(TipoGarantia Tipo, decimal Monto, decimal Porcentaje, DateOnly Vigencia);
public record CrearContratoDto(string NumeroContrato, TipoContrato Tipo, decimal MontoContratado, DateOnly FechaInicio, DateOnly FechaTermino,
    PeriodoEstimacion PeriodoEstimacion, string DependenciaContratante, string ContratistaEmpresa, string ContratistaRepresentante,
    List<ConceptoContratoInputDto> ConceptoContratos, List<ProgramaObraInputDto> ProgramaObra, List<GarantiaInputDto> Garantias);
public record ContratoResumenDto(int Id, string NumeroContrato, decimal MontoContratado, decimal MontoEstimado, decimal MontoPagado, EstadoContrato Estado);
public record ContratoDetalleDto(int Id, string NumeroContrato, TipoContrato Tipo, decimal MontoContratado, DateOnly FechaInicio, DateOnly FechaTermino,
    string DependenciaContratante, string ContratistaEmpresa, EstadoContrato Estado, decimal ImporteTotalCatalogo,
    List<ConceptoContratoDto> ConceptoContratos, List<GarantiaDto> Garantias);
public record ConceptoContratoDto(int Id, string Clave, string Descripcion, string UnidadMedida, decimal CantidadContratada, decimal PrecioUnitario, decimal Importe);
public record GarantiaDto(int Id, TipoGarantia Tipo, decimal Monto, decimal Porcentaje, DateOnly Vigencia, EstadoGarantia Estado);
public record ActualizarProgramaObraDto(List<ProgramaObraInputDto> ProgramaObra);

// SV-08
public record UsuarioDto(int Id, string Nombre, string Correo, bool Activo);
public record UsuarioContratoDto(int ContratoId, RolSistema Rol);

// SV-10
public record DashboardContratoDto(int ContratoId, string NumeroContrato, decimal AvanceFisicoPct, decimal AvanceProgramadoPct,
    decimal MontoEjercido, decimal MontoContratado, int EstimacionesPendientes, int ConveniosActivos, int GarantiasPorVencer);
