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
  firmasDetalle?: FirmaDetalle[];
  folioRef: string | null;
  foto?: boolean;
}

export interface FirmaDetalle {
  rol: string;
  usuarioId: number;
  nombreUsuario: string;
  firmado: boolean;
  fechaFirma: string | null;
}