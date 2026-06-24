export type EstadoConv =
  | "En evaluación"
  | "Dictaminada"
  | "Promovida"
  | "Aprobado"
  | "Rechazado";

export interface Convenio {
  id: string;
  tipo: string;
  monto: number | null;
  justificacion: string;
  estado: EstadoConv;
  dictamen: string | null;
  dictamenJust: string;
  pasos: string[];
}
