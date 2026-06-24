export interface PuntoCurvaS {
  mes: string;
  programado: number;
  real: number | null;
}

export const mockCurvaS: PuntoCurvaS[] = [
  { mes: "Mar", programado: 8, real: 8 },
  { mes: "Abr", programado: 24, real: 22 },
  { mes: "May", programado: 45, real: 41 },
  { mes: "Jun", programado: 68, real: 63 },
  { mes: "Jul", programado: 84, real: null },
  { mes: "Ago", programado: 95, real: null },
  { mes: "Sep", programado: 100, real: null },
];
