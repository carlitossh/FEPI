using Fepi.Api.Models;

namespace Fepi.Api.DTOs;

// ===================== RESPUESTA ESTANDARIZADA =====================
public record ApiResponse<T>(bool Success, string Message, T? Data);
public record ApiErrorResponse(bool Success, string Message, List<string> Errors);

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
public record FirmaDto(int UsuarioId, string NombreUsuario, RolSistema Rol, bool EsEmisor, bool Firmado, DateTime? FechaFirma);
public record CrearMinutaDto(int BitacoraId, DateOnly Fecha, string Lugar, string ContenidoAcuerdos, List<string> Participantes);
public record CrearIncidenciaDto(int BitacoraId, DateOnly FechaEvento, string Descripcion, string UrlFotografia, int ActorRegistroId);
public record GenerarNotaDesdeIncidenciaDto(int IncidenciaId, int TipoNotaCatalogoId, int UsuarioEmisorId, RolSistema RolEmisor);

// SV-04 — Convenios (usa TipoConvenio y EstadoConvenio actualizados)
public record CrearConvenioDto(
    int ContratoId,
    TipoConvenio Tipo,
    string Justificacion,
    decimal? MontoSolicitado,
    int? PlazoDiasSolicitado,
    int SolicitanteId,
    List<string> UrlsDocumentos,
    int? EmpresaId = null,
    string? NumeroConvenio = null);

public record ConvenioResumenDto(int Id, TipoConvenio Tipo, EstadoConvenio Estado, decimal? MontoSolicitado, decimal VariacionAcumuladaPorcentaje);

public record ConvenioDetalleDto(
    int Id,
    int ContratoId,
    TipoConvenio Tipo,
    string Justificacion,
    EstadoConvenio Estado,
    decimal? MontoSolicitado,
    int? PlazoDiasSolicitado,
    decimal VariacionAcumuladaPorcentaje,
    RevisionSupervisionDto? Revision,
    PromocionResidenciaDto? Promocion,
    ResolucionDependenciaDto? Resolucion);

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

public record CrearContratoDto(
    string NumeroContrato,
    TipoContrato Tipo,
    decimal MontoContratado,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    string DependenciaContratante,
    int? EmpresaId = null,
    string? NumeroLicitacion = null,
    string NombreObra = "",
    string? UbicacionExacta = null,
    TipoPeriodoEstimacion? TipoPeriodo = null,
    ModalidadPago? ModalidadPago = null,
    decimal? PorcentajeAnticipo = null,
    string ResidenteNombre = "",
    string SupervisorExternoNombre = "",
    string SuperintendenteNombre = "",
    List<GarantiaInputDto>? Garantias = null);

public record ActualizarContratoDto(
    string NumeroContrato,
    TipoContrato Tipo,
    decimal MontoContratado,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    string DependenciaContratante,
    string NombreObra = "",
    string ResidenteNombre = "",
    string SupervisorExternoNombre = "",
    string SuperintendenteNombre = "");

public record ContratoResumenDto(int Id, string NumeroContrato, decimal MontoContratado, decimal MontoEstimado, decimal MontoPagado, EstadoContrato Estado);

public record ContratoDetalleDto(
    int Id,
    string NumeroContrato,
    TipoContrato Tipo,
    decimal MontoContratado,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    string DependenciaContratante,
    string ContratistaEmpresa,
    string ContratistaRepresentante,
    EstadoContrato Estado,
    decimal ImporteTotalCatalogo,
    string? NombreObra,
    string? ResidenteNombre,
    string? SupervisorExternoNombre,
    string? SuperintendenteNombre,
    List<ConceptoContratoDto> ConceptoContratos,
    List<GarantiaDto> Garantias);

public record ConceptoContratoDto(int Id, string Clave, string Descripcion, string UnidadMedida, decimal CantidadContratada, decimal PrecioUnitario, decimal Importe);
public record GarantiaDto(int Id, TipoGarantia Tipo, decimal Monto, decimal Porcentaje, DateOnly Vigencia, EstadoGarantia Estado);
public record ActualizarProgramaObraDto(List<ProgramaObraInputDto> ProgramaObra);

// SV-08
public record UsuarioDto(int Id, string Nombre, string Correo, RolUsuario Rol, bool Activo);
public record UsuarioContratoDto(int ContratoId, RolSistema Rol);
public record UsuarioContratoDetalleDto(int Id, string Nombre, string Correo, RolSistema Rol, bool Activo);
public record InvitarUsuarioContratoDto(string Nombre, string Correo, RolSistema Rol);
public record ActualizarRolDto(int ContratoId, RolSistema Rol);
public record RegistrarCierreDto(DateOnly FechaEntrega, string EstadoObraDescripcion, string EstadoGarantiasDescripcion, List<string> UrlsEvidencia);

// SV-10
public record DashboardContratoDto(int ContratoId, string NumeroContrato, decimal AvanceFisicoPct, decimal AvanceProgramadoPct,
    decimal MontoEjercido, decimal MontoContratado, int EstimacionesPendientes, int ConveniosActivos, int GarantiasPorVencer,
    decimal MontoPagadoTotal);

public record CurvaFinancieraPuntoDto(string Periodo, decimal PorcentajeEstimado, decimal PorcentajePagado);
public record CurvaFinancieraDto(List<CurvaFinancieraPuntoDto> Puntos, decimal PorcentajeEstimadoActual, decimal PorcentajePagadoActual);
public record VencimientoDto(string Tipo, string Descripcion, DateOnly FechaLimite, int DiasRestantes, string Severidad);
public record BitacoraResumenDto(bool Abierta, int TotalNotas, int NotasCerradas, int NotasFirmasPendientes,
    int Minutas, int Incidencias, int IncidenciasConNota, string? UltimaNotaAsunto, DateTime? UltimaNotaFecha);
public record EstimacionesResumenDto(int Total, int Borrador, int Enviada, int Observada,
    int AprobadaSupervision, int Rechazada, int AprobadaResidencia, int Pagada,
    int SinPago, int PagoParcial, int PagadaEstado);
public record ConveniosResumenDto(int Total, int Activos, int Aprobados, int Rechazados,
    int PendientesRevision, decimal VariacionMontoAcumulada, int VariacionPlazoAcumuladaDias);
public record ActividadRecienteDto(DateTime Fecha, string Modulo, string Descripcion, string? Usuario, string? Referencia);

// ===================== DETALLE COMPLETO DE CONTRATO =====================

public record EmpresaResumenDetalleDto(int Id, string Nombre, string RepresentanteNombre);

public record ResumenConceptosDetalleDto(
    int TotalSecciones,
    int TotalConceptos,
    int TotalConceptosActivos,
    decimal ImporteTotalCatalogo);

public record ResumenEstimacionesDetalleDto(
    int Total,
    int Borrador,
    int Enviadas,
    int Aprobadas,
    int Pagadas,
    decimal MontoEstimadoTotal,
    decimal MontoPagadoTotal,
    decimal SaldoPendiente);

public record ResumenConveniosDetalleDto(
    int Total,
    int Aplicados,
    int Pendientes,
    decimal? VariacionMonto,
    int? VariacionDias);

public record ResumenBitacoraDetalleDto(
    bool Abierta,
    int TotalNotas,
    int NotasPendientesFirma);

public record FiniquitoResumenDetalleDto(
    int Id,
    EstadoFiniquitoContrato Estado,
    decimal? ImporteFinalAFiniquitar);

public record AlertaResumenDetalleDto(
    int Id,
    string Titulo,
    TipoAlerta Tipo,
    PrioridadAlerta Prioridad,
    DateTime FechaCreacion,
    bool Leida);

public record ContratoDetalleCompletoDto(
    int Id,
    string NumeroContrato,
    string? NumeroLicitacion,
    string? NombreObra,
    TipoContrato Tipo,
    EstadoContrato Estado,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    DateTime FechaCreacion,
    string DependenciaContratante,
    string? UbicacionExacta,
    decimal ImporteTotal,
    decimal ImporteSinIVA,
    ModalidadPago ModalidadPago,
    decimal PorcentajeAnticipo,
    decimal MontoAnticipo,
    int NumeroPeriodos,
    string? ResidenteNombre,
    string? SupervisorExternoNombre,
    string? SuperintendenteNombre,
    EmpresaResumenDetalleDto Empresa,
    List<PeriodoContratoResponse> Periodos,
    ResumenConceptosDetalleDto Conceptos,
    bool TieneProgramaObra,
    ResumenEstimacionesDetalleDto Estimaciones,
    ResumenConveniosDetalleDto Convenios,
    ResumenBitacoraDetalleDto? Bitacora,
    int TotalRegistrosDiarios,
    FiniquitoResumenDetalleDto? Finiquito,
    List<AlertaResumenDetalleDto> AlertasRecientes);
