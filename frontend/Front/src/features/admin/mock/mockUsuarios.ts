import type { Usuario } from "../types";

export const mockUsuarios: Usuario[] = [
  { nombre: "Ing. R. Domínguez", email: "r.dominguez@constructora.mx", rol: "Superintendente", activo: true },
  { nombre: "Ing. A. Herrera", email: "a.herrera@sopot.gob.mx", rol: "Residente", activo: true },
  { nombre: "Ing. L. Martínez", email: "l.martinez@supervision.mx", rol: "Supervisor", activo: true },
  { nombre: "Lic. P. Gutiérrez", email: "p.gutierrez@sopot.gob.mx", rol: "Financiero", activo: true },
  { nombre: "Dir. C. Flores", email: "c.flores@sopot.gob.mx", rol: "Dependencia", activo: false },
];
