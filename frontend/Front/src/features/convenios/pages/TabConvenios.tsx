import { useState, useEffect } from "react";
import { CheckCircle } from "lucide-react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { SectionLabel } from "../../../components/SectionLabel";
import { Modal } from "../../../components/Modal";
import { ModalNuevoConvenio } from "../components/ModalNuevoConvenio";
import { ModalDictamen } from "../components/ModalDictamen";
import { ModalResolucion } from "../components/ModalResolucion";
import { conveniosService } from "../services/conveniosService";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, obra, obraSoft, aprobado, aprobadoSoft,
  observado, muted,
} from "../../../styles/theme";
import type { Convenio, EstadoConv } from "../types";

const CONTRATO_ID = 1;
const USUARIO_ID = 1;

const ESTADO_MAP: Record<number, EstadoConv> = {
  1: "En evaluación",
  2: "Dictaminada",
  3: "Promovida",
  4: "Aprobado",
  5: "Rechazado",
};

const TIPO_MAP: Record<number, string> = {
  1: "Incremento de monto",
  2: "Ampliación de plazo",
  3: "Ajuste de catálogo",
};

const TIPO_BACKEND: Record<string, number> = {
  "Incremento de monto": 1,
  "Reducción de monto": 1,
  "Ampliación de plazo": 2,
  "Ajuste de catálogo": 3,
};

function mapResumen(c: any): Convenio {
  return {
    id: `CM-${String(c.id).padStart(3, "0")}`,
    rawId: c.id,
    tipo: TIPO_MAP[c.tipo] ?? "Convenio",
    monto: c.montoSolicitado ?? null,
    justificacion: "",
    estado: ESTADO_MAP[c.estado] ?? "En evaluación",
    dictamen: null,
    dictamenJust: "",
    pasos: [],
  };
}

function mapDetalle(c: any): Convenio {
  return {
    id: `CM-${String(c.id).padStart(3, "0")}`,
    rawId: c.id,
    tipo: TIPO_MAP[c.tipo] ?? "Convenio",
    monto: c.montoSolicitado ?? null,
    justificacion: c.justificacion ?? "",
    estado: ESTADO_MAP[c.estado] ?? "En evaluación",
    dictamen: c.revision ? (c.revision.decision === 1 ? "Procedente" : "No procedente") : null,
    dictamenJust: c.revision?.justificacion ?? "",
    pasos: [],
  };
}

interface TabConveniosProps {
  rol: string;
}

export function TabConvenios({ rol }: TabConveniosProps) {
  const [convenios, setConvenios] = useState<Convenio[]>([]);
  const [loading, setLoading] = useState(true);
  const [showNuevo, setShowNuevo] = useState(false);
  const [detalle, setDetalle] = useState<Convenio | null>(null);
  const [showDictamen, setShowDictamen] = useState(false);
  const [showResolucion, setShowResolucion] = useState(false);

  const puedeCrear = rol === "Superintendente";
  const puedeDictaminar = rol === "Supervisor";
  const puedePromover = rol === "Residente";
  const puedeResolver = rol === "Dependencia";

  useEffect(() => {
    cargar();
  }, []);

  async function cargar() {
    setLoading(true);
    try {
      const data = await conveniosService.getConvenios(CONTRATO_ID);
      setConvenios(data.map(mapResumen));
    } catch (e: any) {
      console.error("Error cargando convenios:", e);
    } finally {
      setLoading(false);
    }
  }

  async function verDetalle(conv: Convenio) {
    try {
      const data = await conveniosService.getConvenio(conv.rawId);
      setDetalle(mapDetalle(data));
    } catch {
      setDetalle(conv);
    }
  }

  const variacionActual = convenios.length > 0
    ? (convenios[0] as any).variacionAcumuladaPorcentaje ?? 18
    : 18;

  const pasoActivo = (conv: Convenio): number => {
    const map: Record<string, number> = {
      "En evaluación": 0,
      Dictaminada: 1,
      Promovida: 2,
      Aprobado: 3,
      Rechazado: 3,
    };
    return map[conv.estado] ?? 0;
  };

  async function handlePromover(rawId: number) {
    try {
      await conveniosService.promoverConvenio(rawId, { residenteId: USUARIO_ID });
      await cargar();
      setDetalle(null);
    } catch (e: any) {
      alert("Error al promover: " + e.message);
    }
  }

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20 }}>
        {puedeCrear && (
          <PrimaryBtn onClick={() => setShowNuevo(true)}>+ Nueva solicitud</PrimaryBtn>
        )}
        <div
          style={{
            marginLeft: "auto",
            fontFamily: "JetBrains Mono",
            fontSize: 11.5,
            color: muted,
          }}
        >
          Variación acumulada: <strong style={{ color: observado }}>18%</strong> / límite legal 25%
        </div>
      </div>

      <Card style={{ overflow: "hidden", marginBottom: 24 }}>
        {loading ? (
          <div style={{ padding: "24px", textAlign: "center", color: muted, fontSize: 12.5 }}>
            Cargando convenios...
          </div>
        ) : (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["ID", "Tipo", "Monto", "Estado", "Dictamen", ""]} />
            <tbody>
              {convenios.length === 0 ? (
                <tr>
                  <td colSpan={6} style={{ padding: "24px", textAlign: "center", color: muted }}>
                    No hay convenios registrados.
                  </td>
                </tr>
              ) : (
                convenios.map((c, i) => (
                  <tr
                    key={c.rawId}
                    style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                    onMouseEnter={(el) => (el.currentTarget.style.background = obraSoft)}
                    onMouseLeave={(el) =>
                      (el.currentTarget.style.background = i % 2 === 1 ? "#FAF8F2" : paper2)
                    }
                  >
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                        fontFamily: "JetBrains Mono",
                        fontWeight: 600,
                        color: obra,
                      }}
                    >
                      {c.id}
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      {c.tipo}
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                        fontFamily: "JetBrains Mono",
                        fontSize: 11.5,
                      }}
                    >
                      {c.monto ? fmtMXN(c.monto) : "—"}
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <EstadoBadge estado={c.estado} />
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 12 }}>
                      {c.dictamen || "—"}
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <button
                        onClick={() => verDetalle(c)}
                        style={{
                          fontSize: 11.5,
                          color: obra,
                          background: "none",
                          border: `1px solid ${rule}`,
                          borderRadius: 3,
                          padding: "4px 10px",
                          cursor: "pointer",
                          fontFamily: "'IBM Plex Sans', sans-serif",
                        }}
                      >
                        Ver →
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        )}
      </Card>

      {/* Flujo de aprobación */}
      {convenios
        .filter((c) => ["En evaluación", "Dictaminada", "Promovida"].includes(c.estado))
        .map((c) => (
          <div key={c.rawId} style={{ marginBottom: 24 }}>
            <div
              style={{
                fontSize: 12,
                fontWeight: 700,
                textTransform: "uppercase",
                letterSpacing: "0.06em",
                color: muted,
                marginBottom: 14,
              }}
            >
              Flujo de aprobación — {c.id}
            </div>
            <div
              style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}
            >
              {[
                { n: "1", label: "Solicitada", key: "En evaluación" },
                { n: "2", label: "Dictamen Supervisión", key: "Dictaminada" },
                { n: "3", label: "Promovida (Residencia)", key: "Promovida" },
                { n: "4", label: "Resolución Dependencia", key: "Resolucion" },
              ].map((s, idx) => {
                const paso = pasoActivo(c);
                const done = idx < paso;
                const active = idx === paso;
                return (
                  <div
                    key={s.n}
                    style={{
                      border: `1.5px solid ${done ? aprobado : active ? obra : rule}`,
                      background: done ? aprobadoSoft : active ? obraSoft : paper2,
                      borderRadius: 4,
                      padding: "14px 16px",
                      color: done ? aprobado : active ? obra : muted,
                    }}
                  >
                    <div
                      style={{
                        fontFamily: "JetBrains Mono",
                        fontSize: 22,
                        fontWeight: 700,
                        opacity: 0.2,
                      }}
                    >
                      {s.n}
                    </div>
                    <div
                      style={{ fontSize: 12.5, fontWeight: active ? 600 : 400, marginTop: 4 }}
                    >
                      {s.label}
                    </div>
                    {done && <div style={{ fontSize: 10.5, marginTop: 6 }}>✓ Completado</div>}
                    {active && <div style={{ fontSize: 10.5, marginTop: 6 }}>→ En proceso</div>}
                  </div>
                );
              })}
            </div>
          </div>
        ))}

      {/* Detalle modal */}
      {detalle && (
        <Modal
          title={`Convenio ${detalle.id}`}
          subtitle={detalle.tipo}
          onClose={() => setDetalle(null)}
          width={520}
        >
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Tipo de modificación</SectionLabel>
            <div style={{ fontSize: 13 }}>{detalle.tipo}</div>
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Monto de variación</SectionLabel>
            <div style={{ fontFamily: "JetBrains Mono", fontSize: 13 }}>
              {detalle.monto ? fmtMXN(detalle.monto) : "Sin impacto económico"}
            </div>
          </div>
          {detalle.justificacion && (
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Justificación</SectionLabel>
              <div
                style={{
                  background: "#FAF8F2",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  padding: "10px 14px",
                  fontSize: 12.5,
                  lineHeight: 1.6,
                }}
              >
                {detalle.justificacion}
              </div>
            </div>
          )}
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Estado</SectionLabel>
            <EstadoBadge estado={detalle.estado} />
          </div>
          {detalle.dictamen && (
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Dictamen de supervisión</SectionLabel>
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: 8,
                  padding: "10px 14px",
                  background: aprobadoSoft,
                  borderRadius: 3,
                  border: `1px solid ${aprobado}`,
                }}
              >
                <CheckCircle size={14} color={aprobado} />
                <span style={{ fontSize: 12.5, color: aprobado, fontWeight: 600 }}>
                  {detalle.dictamen}
                </span>
                {detalle.dictamenJust && (
                  <span style={{ fontSize: 12, color: muted }}>— {detalle.dictamenJust}</span>
                )}
              </div>
            </div>
          )}
          <div style={{ display: "flex", gap: 10, flexWrap: "wrap", marginTop: 6 }}>
            {puedeDictaminar && detalle.estado === "En evaluación" && (
              <PrimaryBtn onClick={() => setShowDictamen(true)}>Emitir dictamen</PrimaryBtn>
            )}
            {puedePromover && detalle.estado === "Dictaminada" && (
              <PrimaryBtn onClick={() => handlePromover(detalle.rawId)}>
                Promover ante Dependencia
              </PrimaryBtn>
            )}
            {puedeResolver && detalle.estado === "Promovida" && (
              <PrimaryBtn
                onClick={() => setShowResolucion(true)}
                style={{ background: aprobado }}
              >
                Emitir resolución
              </PrimaryBtn>
            )}
          </div>
        </Modal>
      )}

      {showDictamen && detalle && (
        <ModalDictamen
          onClose={() => setShowDictamen(false)}
          onGuardar={async (data) => {
            try {
              await conveniosService.dictaminarConvenio(detalle.rawId, {
                decision: data.procedencia === "Procedente" ? 1 : 2,
                justificacion: data.justificacion,
                supervisorId: USUARIO_ID,
              });
              setShowDictamen(false);
              setDetalle(null);
              await cargar();
            } catch (e: any) {
              alert("Error al dictaminar: " + e.message);
            }
          }}
        />
      )}

      {showResolucion && detalle && (
        <ModalResolucion
          onClose={() => setShowResolucion(false)}
          onGuardar={async (data) => {
            try {
              await conveniosService.resolverConvenio(detalle.rawId, {
                aprobado: data.resolucion === "Aprobado",
                motivoRechazo: data.motivo || null,
                usuarioDependenciaId: USUARIO_ID,
              });
              setShowResolucion(false);
              setDetalle(null);
              await cargar();
            } catch (e: any) {
              alert("Error al resolver: " + e.message);
            }
          }}
        />
      )}

      {showNuevo && (
        <ModalNuevoConvenio
          onClose={() => setShowNuevo(false)}
          onCrear={async (data) => {
            try {
              await conveniosService.crearConvenio(CONTRATO_ID, {
                contratoId: CONTRATO_ID,
                tipo: TIPO_BACKEND[data.tipo] ?? 1,
                justificacion: data.justificacion,
                montoSolicitado: data.monto ? parseFloat(data.monto) : null,
                plazoDiasSolicitado: null,
                solicitanteId: USUARIO_ID,
                urlsDocumentos: data.doc ? [data.doc] : [],
              });
              setShowNuevo(false);
              await cargar();
            } catch (e: any) {
              alert("Error al crear convenio: " + e.message);
            }
          }}
        />
      )}
    </div>
  );
}
