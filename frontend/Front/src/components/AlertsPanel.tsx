import { Bell } from "lucide-react";
import { useEffect, useState } from "react";
import { C } from "../styles/theme";
import { Card } from "./Card";

const API = "http://localhost:5000/api";

const ROL_NUM: Record<string, number> = {
  Dependencia: 1, Residente: 2, Superintendente: 3,
  Supervisor: 4, Financiero: 5, Administrador: 6,
};

function tipoANivel(tipo: number): "rojo" | "amarillo" | "gris" {
  if (tipo === 1 || tipo === 2) return "rojo";
  if (tipo === 3 || tipo === 4) return "amarillo";
  return "gris";
}

interface AlertsPanelProps {
  rol?: string;
  contratoId?: number;
}

export function AlertsPanel({ rol = "Superintendente", contratoId = 1 }: AlertsPanelProps) {
  const [alertas, setAlertas] = useState<any[]>([]);

  useEffect(() => {
    const rolNum = ROL_NUM[rol] ?? 3;
    fetch(`${API}/alertas?rol=${rolNum}&contratoId=${contratoId}`)
      .then((r) => (r.ok ? r.json() : []))
      .then(setAlertas)
      .catch(() => setAlertas([]));
  }, [rol, contratoId]);

  return (
    <Card style={{ padding: "18px 16px", height: "fit-content" }}>
      <div
        style={{
          fontSize: 13,
          fontWeight: 600,
          color: C.fg,
          marginBottom: 14,
          display: "flex",
          alignItems: "center",
          gap: 8,
        }}
      >
        <Bell size={14} color={C.fgMuted} />
        Alertas activas
      </div>
      {alertas.length === 0 && (
        <div style={{ fontSize: 12, color: C.fgMuted, textAlign: "center", padding: "12px 0" }}>
          Sin alertas activas.
        </div>
      )}
      {alertas.map((a, i) => {
        const nivel = tipoANivel(a.tipo);
        const color =
          nivel === "rojo" ? C.red : nivel === "amarillo" ? C.amber : C.fgMuted;
        const bg =
          nivel === "rojo" ? C.redSoft : nivel === "amarillo" ? C.amberSoft : "rgba(136,136,136,0.1)";
        return (
          <div
            key={i}
            style={{
              display: "flex",
              alignItems: "flex-start",
              gap: 10,
              padding: "10px 12px",
              background: bg,
              borderRadius: 10,
              marginBottom: 8,
            }}
          >
            <div
              style={{
                width: 7,
                height: 7,
                borderRadius: "50%",
                background: color,
                flexShrink: 0,
                marginTop: 4,
              }}
            />
            <div style={{ flex: 1 }}>
              <div style={{ fontSize: 12, color: C.fg, lineHeight: 1.45 }}>
                {a.mensaje}
              </div>
            </div>
          </div>
        );
      })}
    </Card>
  );
}
