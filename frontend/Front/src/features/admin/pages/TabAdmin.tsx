import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { mockUsuarios } from "../mock/mockUsuarios";
import { obra, paper2, rule, muted, observado, observadoSoft } from "../../../styles/theme";
import type { RolUsuario } from "../types";

export function TabAdmin() {
  const roles: RolUsuario[] = [
    "Superintendente",
    "Residente",
    "Supervisor",
    "Financiero",
    "Dependencia",
  ];

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
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["Nombre", "Correo", "Rol en contrato", "Acceso", "Acciones"]} />
          <tbody>
            {mockUsuarios.map((u, i) => (
              <tr key={u.email} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
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
                    defaultValue={u.rol}
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
                  <SecondaryBtn onClick={() => {}} style={{ fontSize: 11 }}>
                    {u.activo ? "Suspender" : "Activar"}
                  </SecondaryBtn>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
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
