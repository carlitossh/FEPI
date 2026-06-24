export type RolUsuario =
  | "Superintendente"
  | "Supervisor"
  | "Residente"
  | "Financiero"
  | "Dependencia"
  | "Administrador";

export interface Usuario {
  nombre: string;
  email: string;
  rol: RolUsuario;
  activo: boolean;
}
