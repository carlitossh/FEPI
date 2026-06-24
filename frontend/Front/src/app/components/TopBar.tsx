import { Bell, ChevronDown } from "lucide-react";
import { ink, paper, folio, rule } from "../../styles/theme";
import { mockContrato } from "../../features/dashboard/mock/mockContrato";

const ROLES = [
  "Superintendente",
  "Supervisor",
  "Residente",
  "Financiero",
  "Dependencia",
  "Administrador",
];

interface TopBarProps {
  rol: string;
  setRol: (rol: string) => void;
  showRolPicker: boolean;
  setShowRolPicker: (v: boolean) => void;
  alertOpen: boolean;
  setAlertOpen: (v: boolean) => void;
}

export function TopBar({
  rol,
  setRol,
  showRolPicker,
  setShowRolPicker,
  alertOpen,
  setAlertOpen,
}: TopBarProps) {
  return (
    <div
      style={{
        position: "sticky",
        top: 0,
        zIndex: 50,
        background: ink,
        color: paper,
        padding: "0 28px",
        display: "flex",
        alignItems: "center",
        gap: 14,
        height: 56,
        borderBottom: `3px solid ${folio}`,
      }}
    >
      <div
        style={{
          width: 32,
          height: 32,
          border: `2px solid ${paper}`,
          borderRadius: "50%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontFamily: "'IBM Plex Serif', serif",
          fontWeight: 700,
          fontSize: 12,
        }}
      >
        FE
      </div>
      <div
        style={{
          fontFamily: "'IBM Plex Serif', serif",
          fontWeight: 700,
          fontSize: 16,
          letterSpacing: "0.02em",
        }}
      >
        FEPI
      </div>
      <div style={{ width: 1, height: 24, background: "rgba(255,255,255,0.15)" }} />
      <div style={{ fontSize: 12, color: "#C7C2B0" }}>
        Contrato{" "}
        <span style={{ fontFamily: "JetBrains Mono", color: paper }}>
          {mockContrato.id}
        </span>
        <span style={{ marginLeft: 8 }}>· {mockContrato.descripcion}</span>
      </div>

      <div style={{ marginLeft: "auto", display: "flex", alignItems: "center", gap: 14 }}>
        {/* Rol selector */}
        <div style={{ position: "relative" }}>
          <button
            onClick={() => setShowRolPicker(!showRolPicker)}
            style={{
              background: "rgba(255,255,255,0.1)",
              border: "1px solid rgba(255,255,255,0.2)",
              borderRadius: 3,
              color: paper,
              padding: "5px 10px",
              fontSize: 11,
              cursor: "pointer",
              fontFamily: "'IBM Plex Sans', sans-serif",
              display: "flex",
              alignItems: "center",
              gap: 5,
            }}
          >
            <span style={{ color: "#C7C2B0" }}>Rol:</span> {rol}{" "}
            <ChevronDown size={11} />
          </button>
          {showRolPicker && (
            <div
              style={{
                position: "absolute",
                top: "110%",
                right: 0,
                background: "#FFFFFF",
                border: `1px solid ${rule}`,
                borderRadius: 4,
                boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
                zIndex: 200,
                minWidth: 160,
              }}
            >
              {ROLES.map((r) => (
                <button
                  key={r}
                  onClick={() => {
                    setRol(r);
                    setShowRolPicker(false);
                  }}
                  style={{
                    display: "block",
                    width: "100%",
                    textAlign: "left",
                    padding: "9px 14px",
                    fontSize: 12.5,
                    color: r === rol ? "#1F3864" : ink,
                    background: r === rol ? "#DCE6F2" : "transparent",
                    border: "none",
                    cursor: "pointer",
                    fontFamily: "'IBM Plex Sans', sans-serif",
                    fontWeight: r === rol ? 600 : 400,
                  }}
                >
                  {r === rol && "✓ "}
                  {r}
                </button>
              ))}
            </div>
          )}
        </div>

        <button
          onClick={() => setAlertOpen(!alertOpen)}
          style={{
            position: "relative",
            background: "none",
            border: "none",
            color: "#C7C2B0",
            cursor: "pointer",
            padding: 4,
          }}
        >
          <Bell size={16} />
          <span
            style={{
              position: "absolute",
              top: 0,
              right: 0,
              width: 8,
              height: 8,
              background: folio,
              borderRadius: "50%",
              border: `1.5px solid ${ink}`,
            }}
          />
        </button>
      </div>
    </div>
  );
}
