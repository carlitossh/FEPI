import { Bell } from "lucide-react";
import { useEffect, useState } from "react";
import { folio, observado, ink, muted } from "../styles/theme";
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
          fontSize: 11,
          fontWeight: 700,
          textTransform: "uppercase",
          letterSpacing: "0.07em",
          color: muted,
          marginBottom: 14,
          display: "flex",
          alignItems: "center",
          gap: 6,
        }}
      >
        <Bell size={12} /> Alertas activas
      </div>
      {alertas.length === 0 && (
        <div style={{ fontSize: 12, color: muted, textAlign: "center", padding: "12px 0" }}>
          Sin alertas activas.
        </div>
      )}
      {alertas.map((a, i) => {
        const nivel = tipoANivel(a.tipo);
        const color =
          nivel === "rojo" ? folio : nivel === "amarillo" ? observado : "#999";
        const bg =
          nivel === "rojo" ? "#F3E4E0" : nivel === "amarillo" ? "#F6EAD0" : "#F5F5F5";
        return (
          <div
            key={i}
            style={{
              display: "flex",
              alignItems: "flex-start",
              gap: 8,
              padding: "9px 10px",
              background: bg,
              borderRadius: 3,
              marginBottom: 8,
              borderLeft: `3px solid ${color}`,
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
            <div style={{ fontSize: 11.5, color: ink, lineHeight: 1.4 }}>
              {a.mensaje}
            </div>
          </div>
        );
      })}
    </Card>
  );
}
