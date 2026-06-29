using Fepi.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Fepi.Api.DTOs;

// ===================== EMPRESAS =====================

public record CrearEmpresaRequest(
    string Nombre,
    int RepresentanteUsuarioId
);

public record EmpresaResponse(
    int Id,
    string Nombre,
    int RepresentanteUsuarioId,
    string RepresentanteNombre
);

// ===================== USUARIOS =====================

public record CrearUsuarioRequest(
    string Nombre,
    string Correo,
    string Username,
    string Password,
    RolUsuario Rol
);

public record UsuarioLoginRequest(string Username, string Password);

public record UsuarioLoginResponse(int Id, string Nombre, string Username, RolUsuario Rol);

// ===================== CONTRATOS =====================

public record ContratoResumenResponse(
    int Id,
    string NumeroContrato,
    string? NombreObra,
    string DependenciaContratante,
    int EmpresaId,
    string EmpresaNombre,
    decimal ImporteTotal,
    EstadoContrato Estado,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    int NumeroPeriodos
);

public record CrearContratoRequest(
    string NumeroContrato,
    string? NumeroLicitacion,
    string? NombreObra,
    TipoContrato Tipo,
    int EmpresaId,
    string DependenciaContratante,
    string? UbicacionExacta,
    decimal ImporteTotal,
    decimal ImporteSinIVA,
    ModalidadPago ModalidadPago,
    decimal PorcentajeAnticipo,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    TipoPeriodoEstimacion TipoPeriodo
);

public record ContratoResponse(
    int Id,
    string NumeroContrato,
    string? NumeroLicitacion,
    string? NombreObra,
    TipoContrato Tipo,
    int EmpresaId,
    string EmpresaNombre,
    string DependenciaContratante,
    string? UbicacionExacta,
    decimal ImporteTotal,
    decimal ImporteSinIVA,
    decimal IVA,
    ModalidadPago ModalidadPago,
    decimal PorcentajeAnticipo,
    decimal MontoAnticipo,
    DateOnly FechaInicio,
    DateOnly FechaTermino,
    TipoPeriodoEstimacion TipoPeriodo,
    int NumeroPeriodos,
    EstadoContrato Estado,
    DateTime FechaCreacion
);

// ===================== SECCIONES Y CONCEPTOS =====================

public record CrearSeccionRequest(
    string Nombre,
    string? Descripcion
);

public record SeccionConceptoResponse(
    int Id,
    int ContratoId,
    string Nombre,
    string? Descripcion,
    decimal ImporteTotal,
    decimal Peso,
    List<ConceptoContratoResponse> Conceptos
);

public record CrearConceptoRequest(
    int SeccionConceptoId,
    string Clave,
    string Descripcion,
    string UnidadMedida,
    decimal CantidadContratada,
    decimal PrecioUnitario,
    bool EsExtraordinario = false,
    int? ConvenioModificatorioId = null
);

public record ConceptoContratoResponse(
    int Id,
    int ContratoId,
    int? SeccionConceptoId,
    string Clave,
    string Descripcion,
    string UnidadMedida,
    decimal CantidadContratada,
    decimal PrecioUnitario,
    decimal PrecioTotal,
    bool EsExtraordinario,
    bool Activo
);

// ===================== PERIODOS =====================

public record PeriodoContratoResponse(
    int Id,
    int Numero,
    DateOnly FechaInicio,
    DateOnly FechaFin
);

// ===================== PROGRAMA DE OBRA =====================

public record CrearProgramaSeccionRequest(
    int SeccionConceptoId,
    int PeriodoInicioId,
    int PeriodoFinId
);

public record ProgramaObraPeriodoResponse(
    int PeriodoContratoId,
    int NumeroPeriodo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    decimal PorcentajePlanificadoPeriodo,
    decimal ImportePlanificadoPeriodo,
    decimal AvanceParcialPlanificado
);

public record ProgramaObraSeccionResponse(
    int Id,
    int SeccionConceptoId,
    string NombreSeccion,
    int PeriodoInicioId,
    int PeriodoFinId,
    List<ProgramaObraPeriodoResponse> Periodos
);

public record ProgramaObraResponse(
    int ContratoId,
    List<ProgramaObraSeccionResponse> Secciones
);

public record AvancePlanificadoPeriodoDto(
    int PeriodoContratoId,
    int NumeroPeriodo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    decimal AvancePlanificadoAcumulado,
    decimal ImportePlanificadoAcumulado
);

// ===================== ESTIMACIONES =====================

public record CrearEstimacionRequest(
    int ContratoId,
    int PeriodoContratoId,
    string Periodo
);

public record AgregarConceptoEstimacionRequest(
    int ConceptoContratoId,
    decimal CantidadEjecutadaPeriodo,
    decimal PrecioUnitarioActual
);

public record EstimacionResponse(
    int Id,
    int ContratoId,
    int NumeroEstimacion,
    int? PeriodoContratoId,
    string Periodo,
    EstadoEstimacion Estado,
    decimal ImporteEstimacion,
    DateTime FechaCreacion,
    DateTime? FechaEnvio,
    DateTime? FechaAprobacionSupervision,
    DateTime? FechaAprobacionResidencia,
    DateTime? FechaPago
);

public record CaratulaEstimacionResponse(
    decimal ImporteContrato,
    decimal ImporteEstimadoAcumuladoAnterior,
    decimal ImporteEstimacionActual,
    decimal ImporteEstimadoAcumuladoActual,
    decimal SaldoPorEstimar,
    decimal ImporteAnticipo,
    decimal ImporteAmortizacionActual,
    decimal ImporteAmortizadoAcumuladoAnterior,
    decimal ImporteAmortizadoAcumuladoActual,
    decimal SaldoPorAmortizar,
    decimal AvanceRealPeriodo
);

public record CrearObservacionRequest(string Contenido);

// ===================== CONVENIOS =====================

public record CrearConvenioRequest(
    int ContratoId,
    string NumeroConvenio,
    TipoConvenio Tipo,
    string Descripcion,
    string Justificacion,
    decimal? MontoSolicitado,
    int? PlazoDiasSolicitado,
    int SolicitanteId
);

public record RegistrarCambioConvenioRequest(
    string EntidadAfectada,
    string CampoAfectado,
    string ValorAnterior,
    string ValorNuevo,
    string? DescripcionCambio
);

public record ConvenioResponse(
    int Id,
    int ContratoId,
    string NumeroConvenio,
    TipoConvenio Tipo,
    string Descripcion,
    EstadoConvenio Estado,
    decimal? MontoSolicitado,
    int? PlazoDiasSolicitado,
    DateTime FechaEmision,
    DateTime? FechaAutorizacion
);

// ===================== BITÁCORA =====================

public record AbrirBitacoraRequest(
    int ContratoId,
    string FolioBitacora,
    int ResidenteObraUsuarioId,
    int SuperintendenteUsuarioId,
    int SupervisorObraUsuarioId
);

public record BitacoraCaratulaResponse(
    int Id,
    string FolioBitacora,
    string NombreContrato,
    string NumeroContrato,
    string Dependencia,
    string Contratista,
    decimal MontoContratadoConIVA,
    decimal MontoContratadoSinIVA,
    DateOnly FechaInicio,
    DateOnly FechaTerminoProgramada,
    string Residente,
    string Supervisor,
    string Superintendente,
    DateTime FechaApertura,
    bool Abierta
);

public record CrearBitacoraNotaRequest(
    string Asunto,
    string Contenido,
    int AutorUsuarioId,
    TipoNotaBitacora TipoRegistro = TipoNotaBitacora.Nota,
    int? TipoNotaCatalogoId = null,
    int? FolioVinculadoId = null
);

public record FirmarNotaRequest(int UsuarioId);

// ===================== REGISTROS DIARIOS =====================

public record CrearRegistroDiarioRequest(
    DateOnly Fecha,
    int ResponsableUsuarioId,
    string Descripcion
);

public record RegistroDiarioResponse(
    int Id,
    int ContratoId,
    DateOnly Fecha,
    int ResponsableUsuarioId,
    string ResponsableNombre,
    string Descripcion,
    DateTime FechaRegistro
);

// ===================== ARCHIVOS =====================

/// <summary>Request para subir un archivo como multipart/form-data.</summary>
public class SubirArchivoRequest
{
    public IFormFile Archivo { get; set; } = null!;
    public int UsuarioId { get; set; }
}

public record ArchivoResponse(
    int Id,
    string NombreOriginal,
    string TipoContenido,
    long TamanoBytes,
    DateTime FechaSubida,
    int UsuarioSubioId
);

// ===================== ALERTAS USUARIO =====================

public record AlertaUsuarioResponse(
    int Id,
    int UsuarioId,
    int? ContratoId,
    string EntidadRelacionada,
    int? EntidadId,
    string Titulo,
    string Mensaje,
    TipoAlerta Tipo,
    PrioridadAlerta Prioridad,
    DateTime FechaCreacion,
    bool Leida,
    DateTime? FechaLectura
);

public record CrearAlertaRequest(
    int UsuarioId,
    int? ContratoId,
    string EntidadRelacionada,
    int? EntidadId,
    string Titulo,
    string Mensaje,
    TipoAlerta Tipo,
    PrioridadAlerta Prioridad
);

// ===================== FINIQUITO CONTRATO =====================

public record IniciarCierreContratoRequest(int UsuarioId);

public record IniciarCierreContratoResponse(
    int FiniquitoId,
    int ContratoId,
    EstadoFiniquitoContrato Estado
);

public record RegistrarFiniquitoRequest(
    DateTime FechaFiniquito,
    decimal Deductivas,
    decimal Retenciones,
    decimal PenasConvencionales,
    string? Observaciones
);

public record FiniquitoContratoResponse(
    int Id,
    int ContratoId,
    int? BitacoraNotaCierreId,
    DateTime FechaInicioCierre,
    DateTime? FechaFiniquito,
    decimal ImporteContratoOriginal,
    decimal ImporteConvenios,
    decimal ImporteContratoFinal,
    decimal ImporteEstimadoTotal,
    decimal ImportePagadoTotal,
    decimal SaldoPendiente,
    decimal Deductivas,
    decimal Retenciones,
    decimal PenasConvencionales,
    decimal ImporteFinalAFiniquitar,
    string? Observaciones,
    EstadoFiniquitoContrato Estado,
    int CreadoPorUsuarioId,
    DateTime FechaCreacion,
    bool NotaCierreFirmada
);

public record FiniquitoResumenResponse(
    decimal ImporteContratoOriginal,
    decimal ImporteConvenios,
    decimal ImporteContratoFinal,
    decimal ImporteEstimadoTotal,
    decimal ImportePagadoTotal,
    decimal SaldoPendiente
);
