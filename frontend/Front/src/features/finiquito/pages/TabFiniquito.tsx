import { useState } from "react";
import { Download, CheckCircle, Upload } from "lucide-react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { Modal } from "../../../components/Modal";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { mockContrato } from "../../dashboard/mock/mockContrato";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, folio, obra, aprobado, aprobadoSoft,
  observado, observadoSoft, pagado, muted,
} from "../../../styles/theme";

interface TabFiniquitoProps {
  rol: string;
}

export function TabFiniquito({ rol }: TabFiniquitoProps) {
  const [showCierre, setShowCierre] = useState(false);
  const [cierreRegistrado, setCierreRegistrado] = useState(false);
  const [finiquitoCalc, setFiniquitoCalc] = useState(false);

  const puedeRegistrarCierre = rol === "Residente";
  const puedeVerFiniquito =
    rol === "Financiero" || rol === "Dependencia" || rol === "Residente";

  const datos = {
    pagado: 5_268_700,
    pendiente: 1_248_300,
    deductivas: 240_000,
    retenciones: 524_500,
    total: 5_268_700 + 1_248_300 - 240_000 - 524_500,
  };

  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", gap: 24 }}>
      <div>
        {/* Entrega-Recepción */}
        <Card style={{ padding: "20px 22px", marginBottom: 20 }}>
          <div
            style={{
              fontFamily: "'IBM Plex Serif', serif",
              fontWeight: 600,
              fontSize: 15,
              marginBottom: 16,
            }}
          >
            Entrega-Recepción de obra
          </div>
          {cierreRegistrado ? (
            <div
              style={{
                background: aprobadoSoft,
                border: `1px solid ${aprobado}`,
                borderRadius: 3,
                padding: "14px 18px",
                display: "flex",
                alignItems: "center",
                gap: 12,
              }}
            >
              <CheckCircle size={22} color={aprobado} />
              <div>
                <div style={{ fontSize: 13, fontWeight: 600, color: aprobado }}>
                  Cierre físico registrado
                </div>
                <div style={{ fontSize: 12, color: muted, marginTop: 2 }}>
                  Fecha de entrega: 25-jun-2026 · Documentación adjunta
                </div>
              </div>
            </div>
          ) : (
            <div style={{ textAlign: "center", padding: "20px 0" }}>
              <div style={{ color: muted, fontSize: 12.5, marginBottom: 16 }}>
                No se ha registrado el cierre físico de obra.
              </div>
              {puedeRegistrarCierre && (
                <PrimaryBtn onClick={() => setShowCierre(true)}>
                  Registrar entrega-recepción
                </PrimaryBtn>
              )}
            </div>
          )}
        </Card>

        {/* Finiquito */}
        <Card style={{ padding: "20px 22px" }}>
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: 16,
            }}
          >
            <div
              style={{
                fontFamily: "'IBM Plex Serif', serif",
                fontWeight: 600,
                fontSize: 15,
              }}
            >
              Finiquito del contrato
            </div>
            {puedeVerFiniquito && !finiquitoCalc && (
              <PrimaryBtn
                onClick={() => {
                  /* api.calcularFiniquito */
                  setFiniquitoCalc(true);
                }}
              >
                Calcular finiquito
              </PrimaryBtn>
            )}
          </div>
          {finiquitoCalc ? (
            <div>
              <div
                style={{
                  display: "grid",
                  gridTemplateColumns: "1fr 1fr",
                  gap: 14,
                  marginBottom: 16,
                }}
              >
                {[
                  { l: "Total estimaciones pagadas", v: datos.pagado, color: aprobado },
                  { l: "Estimaciones pendientes", v: datos.pendiente, color: observado },
                  { l: "Deductivas aplicadas", v: -datos.deductivas, color: folio },
                  { l: "Retenciones", v: -datos.retenciones, color: pagado },
                ].map((k) => (
                  <div
                    key={k.l}
                    style={{ border: `1px solid ${rule}`, borderRadius: 3, padding: "12px 14px" }}
                  >
                    <div
                      style={{
                        fontFamily: "'IBM Plex Serif', serif",
                        fontSize: 18,
                        fontWeight: 700,
                        color: k.color,
                      }}
                    >
                      {fmtMXN(k.v)}
                    </div>
                    <div
                      style={{
                        fontSize: 10.5,
                        textTransform: "uppercase",
                        letterSpacing: "0.04em",
                        color: muted,
                        marginTop: 2,
                      }}
                    >
                      {k.l}
                    </div>
                  </div>
                ))}
              </div>
              <div
                style={{
                  background: ink,
                  color: paper2,
                  borderRadius: 4,
                  padding: "16px 20px",
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <span
                  style={{ fontSize: 12, textTransform: "uppercase", letterSpacing: "0.08em" }}
                >
                  Monto final del contrato
                </span>
                <span
                  style={{
                    fontFamily: "'IBM Plex Serif', serif",
                    fontSize: 24,
                    fontWeight: 700,
                  }}
                >
                  {fmtMXN(datos.total)}
                </span>
              </div>
              <button
                style={{
                  marginTop: 12,
                  width: "100%",
                  background: "none",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  padding: "9px",
                  fontSize: 12,
                  color: muted,
                  cursor: "pointer",
                  fontFamily: "'IBM Plex Sans', sans-serif",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  gap: 6,
                }}
              >
                <Download size={13} /> Descargar reporte de finiquito
              </button>
            </div>
          ) : (
            <div style={{ color: muted, fontSize: 12.5, textAlign: "center", padding: "20px 0" }}>
              {puedeVerFiniquito
                ? "Presiona 'Calcular finiquito' para consolidar la información del contrato."
                : "Solo el Área Financiera puede calcular el finiquito."}
            </div>
          )}
        </Card>
      </div>

      <AlertsPanel />

      {showCierre && (
        <Modal
          title="Registrar entrega-recepción"
          subtitle="HU-19 · Residente de obra"
          onClose={() => setShowCierre(false)}
          width={460}
        >
          {[
            { key: "fecha", label: "Fecha de entrega *", placeholder: "DD-MM-YYYY" },
            { key: "acta", label: "Número de acta *", placeholder: "Ej: ACTA-2026-001" },
          ].map((f) => (
            <div key={f.key} style={{ marginBottom: 14 }}>
              <SectionLabel>{f.label}</SectionLabel>
              <TextInput placeholder={f.placeholder} value="" onChange={() => {}} />
            </div>
          ))}
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Estado de garantías al momento de entrega</SectionLabel>
            {mockContrato.garantias.map((g) => (
              <div
                key={g.tipo}
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  padding: "8px 12px",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  marginBottom: 6,
                  fontSize: 12,
                }}
              >
                <span>{g.tipo}</span>
                <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5 }}>
                  {g.vencimiento}
                </span>
                <EstadoBadge estado={g.diasRestantes < 30 ? "por vencer" : "vigente"} />
              </div>
            ))}
          </div>
          <div style={{ marginBottom: 20 }}>
            <SectionLabel>Documentación / fotografía (requerida) *</SectionLabel>
            <div
              style={{
                border: `2px dashed ${rule}`,
                borderRadius: 4,
                padding: 16,
                textAlign: "center",
                cursor: "pointer",
                background: "#FAF8F2",
              }}
              onClick={() => {}}
            >
              <Upload size={20} style={{ color: muted }} />
              <br />
              <span style={{ fontSize: 12, color: muted }}>Adjuntar acta y fotos</span>
            </div>
          </div>
          <div style={{ display: "flex", gap: 10 }}>
            <SecondaryBtn onClick={() => setShowCierre(false)} style={{ flex: 1 }}>
              Cancelar
            </SecondaryBtn>
            <PrimaryBtn
              onClick={() => {
                setCierreRegistrado(true);
                setShowCierre(false);
              }}
              style={{ flex: 1 }}
            >
              Registrar cierre
            </PrimaryBtn>
          </div>
        </Modal>
      )}
    </div>
  );
}
