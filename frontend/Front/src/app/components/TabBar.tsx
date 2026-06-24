import {
  FileText, TrendingUp, BookOpen, GitBranch, Folder,
  LayoutDashboard, Shield, Users,
} from "lucide-react";
import { paper2, rule, obra, muted } from "../../styles/theme";

export const TABS = [
  { id: "dashboard", label: "Dashboard", icon: <LayoutDashboard size={13} /> },
  { id: "estimaciones", label: "Estimaciones", icon: <FileText size={13} /> },
  { id: "avance", label: "Avance / Curva S", icon: <TrendingUp size={13} /> },
  { id: "bitacora", label: "Bitácora", icon: <BookOpen size={13} /> },
  { id: "convenios", label: "Convenios", icon: <GitBranch size={13} /> },
  { id: "expediente", label: "Expediente", icon: <Folder size={13} /> },
  { id: "finiquito", label: "Cierre / Finiquito", icon: <Shield size={13} /> },
  { id: "admin", label: "Usuarios", icon: <Users size={13} /> },
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
        background: "#EFEAE0",
        borderBottom: `1px solid ${rule}`,
        display: "flex",
        paddingLeft: 28,
        overflowX: "auto",
      }}
    >
      {TABS.map((t) => (
        <button
          key={t.id}
          onClick={() => setActiveTab(t.id)}
          style={{
            display: "flex",
            alignItems: "center",
            gap: 6,
            padding: "11px 18px",
            fontSize: 12.5,
            color: activeTab === t.id ? obra : muted,
            background: activeTab === t.id ? paper2 : "transparent",
            border: "none",
            borderBottom: `3px solid ${activeTab === t.id ? obra : "transparent"}`,
            fontWeight: activeTab === t.id ? 600 : 400,
            cursor: "pointer",
            fontFamily: "'IBM Plex Sans', sans-serif",
            whiteSpace: "nowrap",
          }}
        >
          {t.icon}
          {t.label}
        </button>
      ))}
    </div>
  );
}
