import {
  FileText, TrendingUp, BookOpen, GitBranch, Folder,
  LayoutDashboard, Shield, Users,
} from "lucide-react";
import { C } from "../../styles/theme";

export const TABS = [
  { id: "dashboard",    label: "Dashboard",         icon: <LayoutDashboard size={13} /> },
  { id: "estimaciones", label: "Estimaciones",      icon: <FileText size={13} />        },
  { id: "avance",       label: "Avance / Curva S",  icon: <TrendingUp size={13} />      },
  { id: "bitacora",     label: "Bitácora",           icon: <BookOpen size={13} />        },
  { id: "convenios",    label: "Convenios",          icon: <GitBranch size={13} />       },
  { id: "expediente",   label: "Expediente",         icon: <Folder size={13} />          },
  { id: "finiquito",    label: "Cierre / Finiquito", icon: <Shield size={13} />          },
  { id: "admin",        label: "Usuarios",           icon: <Users size={13} />           },
];

interface TabBarProps {
  activeTab: string;
  setActiveTab: (id: string) => void;
}

export function TabBar({ activeTab, setActiveTab }: TabBarProps) {
  return (
    <div
      style={{
        position: "sticky",
        top: 56,
        zIndex: 40,
        background: C.surface,
        borderBottom: `1px solid ${C.border}`,
        display: "flex",
        paddingLeft: 8,
        overflowX: "auto",
      }}
    >
      {TABS.map((t) => {
        const active = activeTab === t.id;
        return (
          <button
            key={t.id}
            onClick={() => setActiveTab(t.id)}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 6,
              padding: "0 16px",
              height: 48,
              fontSize: 12.5,
              color: active ? C.blue : C.fgMuted,
              background: "none",
              border: "none",
              borderBottom: `2px solid ${active ? C.blue : "transparent"}`,
              fontWeight: active ? 600 : 400,
              cursor: "pointer",
              fontFamily: "'IBM Plex Sans', sans-serif",
              whiteSpace: "nowrap",
              transition: "color 0.15s, border-color 0.15s",
            }}
            onMouseEnter={(el) => { if (!active) el.currentTarget.style.color = C.fg; }}
            onMouseLeave={(el) => { if (!active) el.currentTarget.style.color = C.fgMuted; }}
          >
            {t.icon}
            {t.label}
          </button>
        );
      })}
    </div>
  );
}
