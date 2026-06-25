export type EstadoEst =
  | "Borrador"
  | "Enviada"
  | "ObservadaSupervision"
  | "AprobadaSupervision"
  | "RechazadaResidencia"
  | "AprobadaResidencia"
  | "Cancelada";

export type EstadoPagoEst = "SinPago" | "PagoParcial" | "Pagada";

export interface ConceptoEstimacion {
  clave: string;
  desc: string;
  unidad: string;
  cantEjecutada: number;
  precioUnitario: number;
  importe: number;
  flag?: boolean;
}

export interface ObservacionEstimacion {
  ref: string;
  txt: string;
  quien: string;
  hora: string;
  color: string;
}

export interface HistorialEstimacion {
  estadoAnterior: string;
  estadoNuevo: string;
  fecha: string;
  usuario: string;
  comentario?: string;
}

export interface PagoEstimacion {
  id: number;
  fechaPago: string;
  referenciaBancaria: string;
  montoPagado: number;
  usuarioRegistroId?: number;
  fechaRegistro: string;
}

export interface Estimacion {
  id: number;
  periodo: string;
  monto: number;
  montoPagadoAcumulado: number;
  saldoPendientePago: number;
  estado: EstadoEst;
  estadoPago: EstadoPagoEst;
  dias?: string;
  conceptos: ConceptoEstimacion[];
  notasVinculadas: string[];
  observaciones: ObservacionEstimacion[];
  historial: HistorialEstimacion[];
  pagos: PagoEstimacion[];
}

export interface ConceptoCatalogo {
  clave: string;
  desc: string;
  unidad: string;
  precioUnitario: number;
}
