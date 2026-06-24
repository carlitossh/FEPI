import { useState } from "react";
import { X, ChevronRight } from "lucide-react";
import { TopBar } from "./components/TopBar";
import { TabBar, TABS } from "./components/TabBar";
import { AlertsPanel } from "../components/AlertsPanel";
import { TabDashboard } from "../features/dashboard/pages/TabDashboard";
import { TabEstimaciones } from "../features/estimaciones/pages/TabEstimaciones";
import { TabAvance } from "../features/avance/pages/TabAvance";
import { TabBitacora } from "../features/bitacora/pages/TabBitacora";
import { TabConvenios } from "../features/convenios/pages/TabConvenios";
import { TabExpediente } from "../features/expediente/pages/TabExpediente";
import { TabFiniquito } from "../features/finiquito/pages/TabFiniquito";
import { TabAdmin } from "../features/admin/pages/TabAdmin";
import { mockContrato } from "../features/dashboard/mock/mockContrato";
import { paper, paper2, rule, folio, obra, obraSoft, muted } from "../styles/theme";

export default function App() {
  const [activeTab, setActiveTab] = useState("dashboard");
  const [alertOpen, setAlertOpen] = useState(false);
  const [rol, setRol] = useState("Superintendente");
  const [showRolPicker, setShowRolPicker] = useState(false);

  return (
    <div
      style={{
        minHeight: "100vh",
        background: paper,
        fontFamily: "'IBM Plex Sans', sans-serif",
      }}
    >
      {/* Top bar */}
      <TopBar
        rol={rol}
        setRol={setRol}
        showRolPicker={showRolPicker}
        setShowRolPicker={setShowRolPicker}
        alertOpen={alertOpen}
        setAlertOpen={setAlertOpen}
      />

      {/* Tab bar */}
      <TabBar activeTab={activeTab} setActiveTab={setActiveTab} />

      {/* Main content */}
      <div style={{ maxWidth: 1200, margin: "0 auto", padding: "28px 28px 60px" }}>
        <div
          style={{
            display: "flex",
            alignItems: "center",
            gap: 8,
            marginBottom: 20,
          }}
        >
          <div
            style={{
              fontSize: 10.5,
              letterSpacing: "0.12em",
              textTransform: "uppercase",
              color: folio,
              fontWeight: 700,
            }}
          >
            {TABS.find((t) => t.id === activeTab)?.label}
          </div>
          <ChevronRight size={12} color={rule} />
          <div style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>
            {mockContrato.id}
          </div>
          <div
            style={{
              marginLeft: "auto",
              fontSize: 11,
              color: muted,
              background: obraSoft,
              border: `1px solid ${obra}`,
              borderRadius: 10,
              padding: "2px 10px",
              fontWeight: 600,
            }}
          >
            {rol}
          </div>
        </div>

        {activeTab === "dashboard" && <TabDashboard />}
        {activeTab === "estimaciones" && <TabEstimaciones rol={rol} />}
        {activeTab === "avance" && <TabAvance rol={rol} />}
        {activeTab === "bitacora" && <TabBitacora rol={rol} />}
        {activeTab === "convenios" && <TabConvenios rol={rol} />}
        {activeTab === "expediente" && <TabExpediente rol={rol} />}
        {activeTab === "finiquito" && <TabFiniquito rol={rol} />}
        {activeTab === "admin" && <TabAdmin />}
      </div>

      {/* Alert flyout */}
      {alertOpen && (
        <div
          style={{
            position: "fixed",
            top: 56,
            right: 0,
            bottom: 0,
            width: 300,
            background: paper2,
            borderLeft: `1px solid ${rule}`,
            zIndex: 60,
            boxShadow: "-8px 0 30px rgba(26,34,56,0.1)",
            padding: 20,
            overflowY: "auto",
          }}
        >
          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              marginBottom: 16,
            }}
          >
            <div
              style={{
                fontSize: 12,
                fontWeight: 700,
                textTransform: "uppercase",
                letterSpacing: "0.07em",
                color: "#1A2238",
              }}
            >
              Alertas activas
            </div>
            <button
              onClick={() => setAlertOpen(false)}
              style={{
                background: "none",
                border: "none",
                cursor: "pointer",
                color: muted,
              }}
            >
              <X size={16} />
            </button>
          </div>
          <AlertsPanel />
        </div>
      )}

      {/* Footer */}
      <div
        style={{
          borderTop: `1px solid ${rule}`,
          padding: "20px 28px",
          textAlign: "center",
          fontSize: 11.5,
          color: muted,
          fontFamily: "JetBrains Mono",
        }}
      >
        FEPI · Sistema de Seguimiento en Ejecución de Obra Pública · v2.0 · Junio 2026
      </div>
    </div>
  );
}
