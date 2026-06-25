import { Bell, ChevronDown } from "lucide-react";
import { C } from "../../styles/theme";

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
  const initials = rol.slice(0, 2).toUpperCase();

  return (
    <div
      style={{
        position: "sticky",
        top: 0,
        zIndex: 50,
        background: C.surface,
        borderBottom: `1px solid ${C.border}`,
        padding: "0 24px",
        display: "flex",
        alignItems: "center",
        gap: 14,
        height: 56,
        flexShrink: 0,
      }}
    >
      {/* Logo */}
      <div style={{ display: "flex", alignItems: "center", gap: 9, paddingRight: 16, borderRight: `1px solid ${C.border}`, height: "100%", flexShrink: 0 }}>
        <div style={{
          width: 30, height: 30, borderRadius: 9,
          background: `linear-gradient(135deg, ${C.blue}, #7ca8ff)`,
          display: "flex", alignItems: "center", justifyContent: "center",
          fontWeight: 800, fontSize: 11, color: "#fff",
        }}>
          FE
        </div>
        <div>
          <div style={{ fontSize: 13.5, fontWeight: 700, color: C.fg, lineHeight: 1 }}>FEPI</div>
          <div style={{ fontSize: 10, color: C.fgMuted, marginTop: 1 }}>Obra Pública</div>
        </div>
      </div>

      <div style={{ flex: 1 }} />

      {/* Right actions */}
      <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
        {/* Rol selector */}
        <div style={{ position: "relative" }}>
          <button
            onClick={() => setShowRolPicker(!showRolPicker)}
            style={{
              background: C.surface2,
              border: `1px solid ${C.border}`,
              borderRadius: 10,
              color: C.fg,
              padding: "5px 10px",
              fontSize: 11.5,
              cursor: "pointer",
              fontFamily: "'IBM Plex Sans', sans-serif",
              display: "flex",
              alignItems: "center",
              gap: 6,
            }}
          >
            <span style={{ color: C.fgMuted }}>Rol:</span>
            <span style={{ fontWeight: 600 }}>{rol}</span>
            <ChevronDown size={11} color={C.fgMuted} />
          </button>
          {showRolPicker && (
            <div
              style={{
                position: "absolute",
                top: "110%",
                right: 0,
                background: C.surface2,
                border: `1px solid ${C.borderHi}`,
                borderRadius: 12,
                boxShadow: "0 16px 40px rgba(0,0,0,0.4)",
                zIndex: 200,
                minWidth: 170,
                overflow: "hidden",
                padding: 6,
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
                    padding: "9px 12px",
                    fontSize: 12.5,
                    color: r === rol ? C.blue : C.fg,
                    background: r === rol ? C.blueSoft : "transparent",
                    border: "none",
                    borderRadius: 8,
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

        {/* Bell */}
        <button
          onClick={() => setAlertOpen(!alertOpen)}
          style={{
            position: "relative",
            width: 34,
            height: 34,
            borderRadius: 10,
            background: alertOpen ? C.redSoft : C.surface2,
            border: `1px solid ${alertOpen ? C.red + "55" : C.border}`,
            color: alertOpen ? C.red : C.fgMuted,
            cursor: "pointer",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            flexShrink: 0,
          }}
        >
          <Bell size={15} />
          <span
            style={{
              position: "absolute",
              top: 6,
              right: 6,
              width: 7,
              height: 7,
              background: C.red,
              borderRadius: "50%",
              border: `1.5px solid ${C.surface}`,
            }}
          />
        </button>

        {/* User chip */}
        <div style={{
          display: "flex", alignItems: "center", gap: 8,
          background: C.surface2,
          borderRadius: 10,
          padding: "5px 10px 5px 6px",
          border: `1px solid ${C.border}`,
        }}>
          <div style={{
            width: 24, height: 24, borderRadius: "50%",
            background: `linear-gradient(135deg, ${C.purple}, ${C.blue})`,
            display: "flex", alignItems: "center", justifyContent: "center",
            fontSize: 9, fontWeight: 700, color: "#fff", flexShrink: 0,
          }}>
            {initials}
          </div>
          <div style={{ lineHeight: 1 }}>
            <div style={{ fontSize: 12, fontWeight: 600, color: C.fg, whiteSpace: "nowrap" }}>{rol}</div>
            <div style={{ fontSize: 10, color: C.fgMuted, marginTop: 1 }}>Sesión activa</div>
          </div>
        </div>
      </div>
    </div>
  );
}
