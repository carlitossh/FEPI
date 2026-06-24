import { useState } from "react";
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
import { mockConvenios } from "../mock/mockConvenios";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, obra, obraSoft, aprobado, aprobadoSoft,
  observado, muted,
} from "../../../styles/theme";
import type { Convenio, EstadoConv } from "../types";

interface TabConveniosProps {
  rol: string;
}

export function TabConvenios({ rol }: TabConveniosProps) {
  const [convenios, setConvenios] = useState<Convenio[]>(mockConvenios);
  const [showNuevo, setShowNuevo] = useState(false);
  const [detalle, setDetalle] = useState<Convenio | null>(null);
  const [showDictamen, setShowDictamen] = useState(false);
  const [showResolucion, setShowResolucion] = useState(false);

  const puedeCrear = rol === "Superintendente";
  const puedeDictaminar = rol === "Supervisor";
  const puedePromover = rol === "Residente";
  const puedeResolver = rol === "Dependencia";

  const pasoActivo = (conv: Convenio): number => {
    if (!conv) return 0;
    const map: Record<string, number> = {
      "En evaluación": 0,
      Dictaminada: 1,
      Promovida: 2,
      Aprobado: 3,
      Rechazado: 3,
    };
    return map[conv.estado] ?? 0;
  };

  function handlePromover(id: string) {
    // api.promoverConvenio(id)
    setConvenios((prev) =>
      prev.map((c) => (c.id === id ? { ...c, estado: "Promovida" as EstadoConv } : c))
    );
    setDetalle((prev) => (prev ? { ...prev, estado: "Promovida" as EstadoConv } : null));
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
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["ID", "Tipo", "Monto", "Estado", "Dictamen", ""]} />
          <tbody>
            {convenios.map((c, i) => (
              <tr
                key={c.id}
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
                    onClick={() => setDetalle(c)}
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
            ))}
          </tbody>
        </table>
      </Card>

      {/* Flujo de aprobación */}
      {convenios
        .filter((c) => ["En evaluación", "Dictaminada", "Promovida"].includes(c.estado))
        .map((c) => (
          <div key={c.id} style={{ marginBottom: 24 }}>
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
              <PrimaryBtn onClick={() => handlePromover(detalle.id)}>
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

      {showDictamen && (
        <ModalDictamen
          onClose={() => setShowDictamen(false)}
          onGuardar={(data) => {
            // api.dictaminarConvenio(detalle.id, data)
            setConvenios((prev) =>
              prev.map((c) =>
                c.id === detalle!.id
                  ? {
                      ...c,
                      estado: "Dictaminada" as EstadoConv,
                      dictamen: data.procedencia,
                      dictamenJust: data.justificacion,
                    }
                  : c
              )
            );
            setDetalle((prev) =>
              prev
                ? {
                    ...prev,
                    estado: "Dictaminada" as EstadoConv,
                    dictamen: data.procedencia,
                    dictamenJust: data.justificacion,
                  }
                : null
            );
            setShowDictamen(false);
          }}
        />
      )}

      {showResolucion && (
        <ModalResolucion
          onClose={() => setShowResolucion(false)}
          onGuardar={(data) => {
            // api.resolverConvenio(detalle.id, data)
            setConvenios((prev) =>
              prev.map((c) =>
                c.id === detalle!.id
                  ? {
                      ...c,
                      estado: (data.resolucion === "Aprobado"
                        ? "Aprobado"
                        : "Rechazado") as EstadoConv,
                    }
                  : c
              )
            );
            setDetalle(null);
            setShowResolucion(false);
          }}
        />
      )}

      {showNuevo && (
        <ModalNuevoConvenio
          onClose={() => setShowNuevo(false)}
          onCrear={(data) => {
            // api.crearConvenio(mockContrato.id, data)
            setConvenios((prev) => [
              {
                id: `CM-${String(prev.length + 1).padStart(3, "0")}`,
                tipo: data.tipo,
                monto: data.monto ? parseFloat(data.monto) : null,
                justificacion: data.justificacion,
                estado: "En evaluación" as EstadoConv,
                dictamen: null,
                dictamenJust: "",
                pasos: [],
              },
              ...prev,
            ]);
            setShowNuevo(false);
          }}
        />
      )}
    </div>
  );
}
