export interface Garantia {
  tipo: string;
  monto: number;
  vencimiento: string;
  estado: string;
  diasRestantes: number;
}

export interface Contrato {
  id: string;
  numero: string;
  tipo: string;
  monto: number;
  anticipo: number;
  fechaInicio: string;
  fechaTermino: string;
  contratista: string;
  dependencia: string;
  residente: string;
  supervisor: string;
  superintendente: string;
  descripcion: string;
  garantias: Garantia[];
}

export const mockContrato: Contrato = {
  id: "CT-2026-014",
  numero: "CT-2026-014",
  tipo: "A precio unitario",
  monto: 11_000_000,
  anticipo: 1_100_000,
  fechaInicio: "01 marzo 2026",
  fechaTermino: "30 septiembre 2026",
  contratista: "Constructora Domínguez S.A. de C.V.",
  dependencia: "SOPOT Edo. de México",
  residente: "Ing. A. Herrera",
  supervisor: "Ing. L. Martínez",
  superintendente: "Ing. R. Domínguez",
  descripcion: "Pavimentación Av. Reforma Norte",
  garantias: [
    { tipo: "Cumplimiento", monto: 550_000, vencimiento: "30-sep-2026", estado: "vigente", diasRestantes: 99 },
    { tipo: "Anticipo", monto: 1_100_000, vencimiento: "30-jul-2026", estado: "vigente", diasRestantes: 37 },
    { tipo: "Vicios ocultos", monto: 110_000, vencimiento: "30-mar-2027", estado: "vigente", diasRestantes: 280 },
  ],
};
