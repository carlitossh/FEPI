export interface Alerta {
  nivel: "rojo" | "amarillo" | "gris";
  texto: string;
  ref: string;
}

export const mockAlertas: Alerta[] = [
  { nivel: "rojo", texto: "Estimación 014 — vence en 2 días hábiles", ref: "EST-014" },
  { nivel: "amarillo", texto: "Nota 0140 — firma pendiente de Supervisión", ref: "FOL-0140" },
  { nivel: "amarillo", texto: "Garantía de cumplimiento — vence en 22 días", ref: "GAR-001" },
  { nivel: "gris", texto: "Convenio CM-008 — pendiente de resolución", ref: "CM-008" },
];
