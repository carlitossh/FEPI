import { useState, useEffect } from "react";
import { Download, CheckCircle, Upload } from "lucide-react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { Modal } from "../../../components/Modal";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { finiquitoService } from "../services/finiquitoService";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, folio, obra, aprobado, aprobadoSoft,
  observado, observadoSoft, pagado, muted,
} from "../../../styles/theme";

const CONTRATO_ID = 1;

interface TabFiniquitoProps {
  rol: string;
}

interface DatosFiniquito {
  pagado: number;
  pendiente: number;
  deductivas: number;
  retenciones: number;
  total: number;
}

export function TabFiniquito({ rol }: TabFiniquitoProps) {
  const [showCierre, setShowCierre] = useState(false);
  const [cierreRegistrado, setCierreRegistrado] = useState(false);
  const [datos, setDatos] = useState<DatosFiniquito | null>(null);
  const [calculando, setCalculando] = useState(false);
  const [cierreFecha, setCierreFecha] = useState("");
  const [cierreEstadoObra, setCierreEstadoObra] = useState("");
  const [cierreEstadoGarantias, setCierreEstadoGarantias] = useState("");

  const puedeRegistrarCierre = rol === "Residente";
  const puedeVerFiniquito =
    rol === "Financiero" || rol === "Dependencia" || rol === "Residente";

  useEffect(() => {
    finiquitoService
      .calcularFiniquito(CONTRATO_ID)
      .then((d) => {
        setDatos({
          pagado: d.totalPagado,
          pendiente: d.totalPendiente,
          deductivas: d.totalDeductivas,
          retenciones: d.totalRetenciones,
          total: d.montoFinal,
        });
        setCierreRegistrado(true);
      })
      .catch(() => {});
  }, []);

  async function handleCalcular() {
    setCalculando(true);
    try {
      const d = await finiquitoService.calcularFiniquito(CONTRATO_ID);
      setDatos({
        pagado: d.totalPagado,
        pendiente: d.totalPendiente,
        deductivas: d.totalDeductivas,
        retenciones: d.totalRetenciones,
        total: d.montoFinal,
      });
    } catch (e: any) {
      alert("No se pudo calcular el finiquito: " + e.message);
    } finally {
      setCalculando(false);
    }
  }

  async function handleRegistrarCierre() {
    if (!cierreFecha || !cierreEstadoObra) {
      alert("La fecha de entrega y el estado de obra son requeridos.");
      return;
    }
    try {
      const [dia, mes, anio] = cierreFecha.split("-");
      const fechaISO = `${anio}-${mes}-${dia}`;
      await finiquitoService.registrarCierre(CONTRATO_ID, {
        fechaEntrega: fechaISO,
        estadoObraDescripcion: cierreEstadoObra,
        estadoGarantiasDescripcion: cierreEstadoGarantias || "Vigentes",
        urlsEvidencia: [],
      });
      setCierreRegistrado(true);
      setShowCierre(false);
    } catch (e: any) {
      alert("Error al registrar cierre: " + e.message);
    }
  }

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
                  Documentación adjunta en expediente
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
            {puedeVerFiniquito && !datos && (
              <PrimaryBtn onClick={handleCalcular} disabled={calculando}>
                {calculando ? "Calculando..." : "Calcular finiquito"}
              </PrimaryBtn>
            )}
          </div>
          {datos ? (
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

      <AlertsPanel rol={rol} contratoId={1} />

      {showCierre && (
        <Modal
          title="Registrar entrega-recepción"
          subtitle="HU-19 · Residente de obra"
          onClose={() => setShowCierre(false)}
          width={460}
        >
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Fecha de entrega *</SectionLabel>
            <TextInput
              placeholder="DD-MM-YYYY"
              value={cierreFecha}
              onChange={setCierreFecha}
            />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Estado de la obra *</SectionLabel>
            <TextArea
              placeholder="Describe el estado físico de la obra al momento de entrega..."
              value={cierreEstadoObra}
              onChange={setCierreEstadoObra}
              rows={3}
            />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Estado de garantías</SectionLabel>
            <TextInput
              placeholder="Ej: Garantía de calidad vigente hasta 2027"
              value={cierreEstadoGarantias}
              onChange={setCierreEstadoGarantias}
            />
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
              onClick={handleRegistrarCierre}
              style={{ flex: 1 }}
              disabled={!cierreFecha || !cierreEstadoObra}
            >
              Registrar cierre
            </PrimaryBtn>
          </div>
        </Modal>
      )}
    </div>
  );
}
