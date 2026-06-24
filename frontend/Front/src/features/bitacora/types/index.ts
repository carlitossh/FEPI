export type TipoNota = "Apertura" | "Nota" | "Minuta" | "Incidencia" | "Solicitud";

export interface FirmasBitacora {
  superintendente: boolean;
  residente: boolean;
  supervisor: boolean;
}

export interface NotaBitacora {
  id: number;
  folio: string;
  tipo: TipoNota;
  fecha: string;
  asunto: string;
  contenido: string;
  firmas: FirmasBitacora;
  folioRef: string | null;
  foto?: boolean;
}
