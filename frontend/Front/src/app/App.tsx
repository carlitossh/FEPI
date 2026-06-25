import { useEffect, useState } from "react";
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
import { getContratos } from "../features/dashboard/services/dashboardService";
import { C } from "../styles/theme";

export default function App() {
  const [activeTab, setActiveTab] = useState("dashboard");
  const [alertOpen, setAlertOpen] = useState(false);
  const [rol, setRol] = useState("Superintendente");
  const [showRolPicker, setShowRolPicker] = useState(false);
  const [contratoNumero, setContratoNumero] = useState("—");
  const [contratoExists, setContratoExists] = useState<boolean | null>(null);

  const refreshContratos = () => {
    getContratos()
      .then((list) => {
        if (list.length > 0) {
          setContratoNumero(list[0].numeroContrato);
          setContratoExists(true);
        } else {
          setContratoNumero("—");
          setContratoExists(false);
        }
      })
      .catch(() => setContratoExists(null));
  };

  useEffect(() => {
    refreshContratos();
  }, []);

  return (
    <div
      style={{
        minHeight: "100vh",
        background: C.bg,
        fontFamily: "'IBM Plex Sans', sans-serif",
        color: C.fg,
      }}
    >
      <TopBar
        rol={rol}
        setRol={setRol}
        showRolPicker={showRolPicker}
        setShowRolPicker={setShowRolPicker}
        alertOpen={alertOpen}
        setAlertOpen={setAlertOpen}
      />

      <TabBar activeTab={activeTab} setActiveTab={setActiveTab} />

      {/* Main content */}
      <div style={{ maxWidth: 1200, margin: "0 auto", padding: "28px 28px 60px" }}>
        {/* Breadcrumb */}
        <div
          style={{
            display: "flex",
            alignItems: "center",
            gap: 8,
            marginBottom: 20,
          }}
        >
          <div style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: C.blue }}>
            {contratoNumero}
          </div>
          <ChevronRight size={11} color={C.fgSub} />
          <div style={{ fontSize: 12, color: C.fg, fontWeight: 500 }}>
            {TABS.find((t) => t.id === activeTab)?.label}
          </div>
          <div
            style={{
              marginLeft: "auto",
              fontSize: 11,
              color: C.blue,
              background: C.blueSoft,
              border: `1px solid ${C.blue}44`,
              borderRadius: 999,
              padding: "3px 12px",
              fontWeight: 600,
            }}
          >
            {rol}
          </div>
        </div>

        {/* Primera vez banner */}
        {contratoExists === false && activeTab !== "expediente" && (
          <div
            style={{
              background: C.amberSoft,
              border: `1px solid ${C.amber}44`,
              borderRadius: 12,
              padding: "12px 16px",
              marginBottom: 20,
              fontSize: 12.5,
              color: C.amber,
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <span>
              <strong>Primera vez en el sistema.</strong> Para comenzar, registra el contrato de obra.
            </span>
            <button
              onClick={() => setActiveTab("expediente")}
              style={{
                background: C.amber,
                color: "#000",
                border: "none",
                borderRadius: 8,
                padding: "6px 14px",
                fontSize: 12,
                fontWeight: 700,
                cursor: "pointer",
                fontFamily: "'IBM Plex Sans', sans-serif",
                whiteSpace: "nowrap",
                marginLeft: 16,
              }}
            >
              Ir a Expediente →
            </button>
          </div>
        )}

        {activeTab === "dashboard"    && <TabDashboard rol={rol} />}
        {activeTab === "estimaciones" && <TabEstimaciones rol={rol} />}
        {activeTab === "avance"       && <TabAvance rol={rol} />}
        {activeTab === "bitacora"     && <TabBitacora rol={rol} />}
        {activeTab === "convenios"    && <TabConvenios rol={rol} />}
        {activeTab === "expediente"   && (
          <TabExpediente
            rol={rol}
            onContratoSaved={() => {
              refreshContratos();
              setTimeout(() => setActiveTab("dashboard"), 1500);
            }}
          />
        )}
        {activeTab === "finiquito" && <TabFiniquito rol={rol} />}
        {activeTab === "admin"     && <TabAdmin />}
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
            background: C.surface,
            borderLeft: `1px solid ${C.border}`,
            zIndex: 60,
            boxShadow: "-8px 0 30px rgba(0,0,0,0.4)",
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
                fontSize: 13,
                fontWeight: 700,
                color: C.fg,
              }}
            >
              Alertas activas
            </div>
            <button
              onClick={() => setAlertOpen(false)}
              style={{
                background: C.surface2,
                border: `1px solid ${C.border}`,
                borderRadius: 8,
                cursor: "pointer",
                color: C.fgMuted,
                padding: 6,
                display: "flex",
              }}
            >
              <X size={14} />
            </button>
          </div>
          <AlertsPanel rol={rol} contratoId={1} />
        </div>
      )}

      {/* Footer */}
      <div
        style={{
          borderTop: `1px solid ${C.border}`,
          padding: "20px 28px",
          textAlign: "center",
          fontSize: 11.5,
          color: C.fgSub,
          fontFamily: "JetBrains Mono",
        }}
      >
        FEPI · Sistema de Seguimiento en Ejecución de Obra Pública · v2.0 · Junio 2026
      </div>
    </div>
  );
}
