import { useState, useEffect } from "react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { adminService } from "../services/adminService";
import { obra, paper2, rule, muted, observado, observadoSoft } from "../../../styles/theme";
import type { RolUsuario } from "../types";

const CONTRATO_ID = 1;

const ROL_FRONTEND: Record<number, RolUsuario> = {
  1: "Dependencia",
  2: "Residente",
  3: "Superintendente",
  4: "Supervisor",
  5: "Financiero",
  6: "Administrador",
};

const ROL_BACKEND: Record<string, number> = {
  Superintendente: 3,
  Residente: 2,
  Supervisor: 4,
  Financiero: 5,
  Dependencia: 1,
  Administrador: 6,
};

interface UsuarioFila {
  id: number;
  nombre: string;
  email: string;
  rol: RolUsuario;
  activo: boolean;
}

export function TabAdmin() {
  const [usuarios, setUsuarios] = useState<UsuarioFila[]>([]);
  const [loading, setLoading] = useState(true);

  const roles: RolUsuario[] = [
    "Superintendente",
    "Residente",
    "Supervisor",
    "Financiero",
    "Dependencia",
  ];

  useEffect(() => {
    cargar();
  }, []);

  async function cargar() {
    setLoading(true);
    try {
      const data = await adminService.getUsuarios(CONTRATO_ID);
      setUsuarios(
        data.map((u: any) => ({
          id: u.id,
          nombre: u.nombre,
          email: u.correo,
          rol: ROL_FRONTEND[u.rol] ?? "Administrador",
          activo: u.activo,
        }))
      );
    } catch (e: any) {
      console.error("Error cargando usuarios:", e);
    } finally {
      setLoading(false);
    }
  }

  async function handleRolChange(usuario: UsuarioFila, nuevoRol: string) {
    const rolBackend = ROL_BACKEND[nuevoRol];
    if (rolBackend === undefined) return;
    try {
      await adminService.actualizarRol(usuario.id, CONTRATO_ID, rolBackend);
      setUsuarios((prev) =>
        prev.map((u) =>
          u.id === usuario.id ? { ...u, rol: nuevoRol as RolUsuario } : u
        )
      );
    } catch (e: any) {
      alert("Error al actualizar rol: " + e.message);
    }
  }

  async function handleToggleAcceso(usuario: UsuarioFila) {
    try {
      if (usuario.activo) {
        await adminService.suspenderUsuario(usuario.id);
      } else {
        await adminService.activarUsuario(usuario.id);
      }
      setUsuarios((prev) =>
        prev.map((u) =>
          u.id === usuario.id ? { ...u, activo: !u.activo } : u
        )
      );
    } catch (e: any) {
      alert("Error al cambiar acceso: " + e.message);
    }
  }

  return (
    <div>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 20,
        }}
      >
        <PrimaryBtn onClick={() => {}}>+ Invitar usuario</PrimaryBtn>
        <div style={{ fontSize: 12, color: muted }}>
          Los usuarios sin rol asignado no tienen acceso al sistema.
        </div>
      </div>

      <Card style={{ overflow: "hidden" }}>
        {loading ? (
          <div style={{ padding: "24px", textAlign: "center", color: muted, fontSize: 12.5 }}>
            Cargando usuarios...
          </div>
        ) : (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["Nombre", "Correo", "Rol en contrato", "Acceso", "Acciones"]} />
            <tbody>
              {usuarios.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ padding: "24px", textAlign: "center", color: muted }}>
                    No hay usuarios registrados en este contrato.
                  </td>
                </tr>
              ) : (
                usuarios.map((u, i) => (
                  <tr key={u.id} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                        <div
                          style={{
                            width: 28,
                            height: 28,
                            borderRadius: "50%",
                            background: obra,
                            color: "#fff",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            fontSize: 11,
                            fontWeight: 700,
                            flexShrink: 0,
                          }}
                        >
                          {u.nombre
                            .split(" ")
                            .map((n) => n[0])
                            .join("")
                            .slice(0, 2)}
                        </div>
                        {u.nombre}
                      </div>
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                        fontSize: 12,
                        color: muted,
                        fontFamily: "JetBrains Mono",
                      }}
                    >
                      {u.email}
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <select
                        value={u.rol}
                        onChange={(e) => handleRolChange(u, e.target.value)}
                        style={{
                          border: `1px solid ${rule}`,
                          background: paper2,
                          borderRadius: 3,
                          padding: "5px 8px",
                          fontSize: 12,
                          fontFamily: "'IBM Plex Sans', sans-serif",
                          outline: "none",
                        }}
                      >
                        {roles.map((r) => (
                          <option key={r}>{r}</option>
                        ))}
                      </select>
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <EstadoBadge estado={u.activo ? "vigente" : "vencida"} />
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <SecondaryBtn
                        onClick={() => handleToggleAcceso(u)}
                        style={{ fontSize: 11 }}
                      >
                        {u.activo ? "Suspender" : "Activar"}
                      </SecondaryBtn>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        )}
      </Card>

      <div
        style={{
          marginTop: 20,
          background: observadoSoft,
          border: `1px solid ${observado}`,
          borderRadius: 4,
          padding: "12px 16px",
          fontSize: 12.5,
          color: observado,
        }}
      >
        <strong>Restricciones por rol:</strong> Cada usuario solo puede realizar las acciones
        correspondientes a su rol asignado en este contrato.
      </div>
    </div>
  );
}
