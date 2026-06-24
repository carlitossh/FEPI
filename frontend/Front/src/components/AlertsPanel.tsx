import { Bell } from "lucide-react";
import { folio, observado, ink, muted } from "../styles/theme";
import { Card } from "./Card";
import { mockAlertas } from "../features/dashboard/mock/mockAlertas";

export function AlertsPanel() {
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
      {mockAlertas.map((a, i) => {
        const color =
          a.nivel === "rojo" ? folio : a.nivel === "amarillo" ? observado : "#999";
        const bg =
          a.nivel === "rojo"
            ? "#F3E4E0"
            : a.nivel === "amarillo"
            ? "#F6EAD0"
            : "#F5F5F5";
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
            <div>
              <div style={{ fontSize: 11.5, color: ink, lineHeight: 1.4 }}>
                {a.texto}
              </div>
              <div
                style={{
                  fontFamily: "JetBrains Mono",
                  fontSize: 10,
                  color,
                  marginTop: 2,
                  fontWeight: 600,
                }}
              >
                {a.ref}
              </div>
            </div>
          </div>
        );
      })}
    </Card>
  );
}
