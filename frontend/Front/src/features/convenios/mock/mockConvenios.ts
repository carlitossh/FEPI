import type { Convenio } from "../types";

export const mockConvenios: Convenio[] = [
  {
    id: "CM-008",
    rawId: 8,
    tipo: "Ampliación de plazo",
    monto: null,
    justificacion: "Condiciones climatológicas atípicas en mayo impiden cumplir con el programa.",
    estado: "Promovida",
    dictamen: "Procedente",
    dictamenJust: "Se comprueba con registros meteorológicos adjuntos.",
    pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"],
  },
  {
    id: "CM-006",
    rawId: 6,
    tipo: "Incremento de monto",
    monto: 450000,
    justificacion: "Volúmenes adicionales de excavación.",
    estado: "Aprobado",
    dictamen: "Procedente",
    dictamenJust: "",
    pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"],
  },
  {
    id: "CM-003",
    rawId: 3,
    tipo: "Ajuste de catálogo",
    monto: 120000,
    justificacion: "Cambio de especificación en concreto.",
    estado: "Aprobado",
    dictamen: "Procedente",
    dictamenJust: "",
    pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"],
  },
];
