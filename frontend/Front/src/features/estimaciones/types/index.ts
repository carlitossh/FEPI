export type EstadoEst = "Borrador" | "Enviada" | "Observada" | "Aprobada" | "Pagada";

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
}

export interface Estimacion {
  id: number;
  periodo: string;
  monto: number;
  estado: EstadoEst;
  dias?: string;
  conceptos: ConceptoEstimacion[];
  notasVinculadas: string[];
  observaciones: ObservacionEstimacion[];
  historial: HistorialEstimacion[];
}

export interface ConceptoCatalogo {
  clave: string;
  desc: string;
  unidad: string;
  precioUnitario: number;
}
