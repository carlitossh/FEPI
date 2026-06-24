import { observado, obra } from "../../../styles/theme";
import type { Estimacion } from "../types";

export const mockEstimaciones: Estimacion[] = [
  {
    id: 14,
    periodo: "Jun 2026",
    monto: 1_248_300,
    estado: "Enviada",
    dias: "18/20",
    conceptos: [
      { clave: "03.012", desc: "Losa de cimentación f'c=250", unidad: "m³", cantEjecutada: 120, precioUnitario: 4050, importe: 486000 },
      { clave: "05.004", desc: "Trabe de liga eje B-C", unidad: "ml", cantEjecutada: 34, precioUnitario: 6185, importe: 210290, flag: true },
      { clave: "06.001", desc: "Castillo K-1", unidad: "pza", cantEjecutada: 18, precioUnitario: 5300, importe: 95400 },
      { clave: "07.002", desc: "Muro de block hueco 15cm", unidad: "m²", cantEjecutada: 240, precioUnitario: 1903, importe: 456720 },
    ],
    notasVinculadas: ["0138", "0140", "0141"],
    observaciones: [
      { ref: "Concepto 05.004", txt: "El cálculo de la trabe no coincide con el generador adjunto: reporta 34 ml, el generador muestra 31 ml.", quien: "Ing. L. Martínez", hora: "hace 2 horas", color: observado },
      { ref: "General", txt: "Falta la firma del residente en la nota de bitácora 0141 referenciada.", quien: "Ing. L. Martínez", hora: "hace 2 horas", color: obra },
    ],
    historial: [
      { estadoAnterior: "—", estadoNuevo: "Borrador", fecha: "01-jun-2026 09:15", usuario: "Ing. R. Domínguez" },
      { estadoAnterior: "Borrador", estadoNuevo: "Enviada", fecha: "03-jun-2026 14:22", usuario: "Ing. R. Domínguez" },
    ],
  },
  { id: 13, periodo: "May 2026", monto: 980_200, estado: "Aprobada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 12, periodo: "Abr 2026", monto: 1_100_000, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 11, periodo: "Mar 2026", monto: 1_050_500, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 10, periodo: "Feb 2026", monto: 890_000, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
];
