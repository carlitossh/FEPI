import { useState } from "react";
import { Download, FileText } from "lucide-react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { Field } from "../../../components/Field";
import { mockContrato } from "../../dashboard/mock/mockContrato";
import { mockCurvaS } from "../../dashboard/mock/mockCurvaS";
import { mockEstimaciones } from "../../estimaciones/mock/mockEstimaciones";
import { mockBitacora } from "../../bitacora/mock/mockBitacora";
import { mockConvenios } from "../../convenios/mock/mockConvenios";
import { mockCatalogoConceptos } from "../../estimaciones/mock/mockCatalogoConceptos";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, obra, obraSoft, folio, muted,
} from "../../../styles/theme";
import type { SeccionExpediente } from "../types";

interface TabExpedienteProps {
  rol: string;
}

export function TabExpediente({ rol: _rol }: TabExpedienteProps) {
  const secciones: SeccionExpediente[] = [
    "Datos generales",
    "Catálogo",
    "Programa de obra",
    "Garantías",
    "Estimaciones",
    "Bitácora",
    "Convenios",
    "Documentación",
  ];
  const [sec, setSec] = useState<SeccionExpediente>("Datos generales");

  return (
    <div style={{ display: "grid", gridTemplateColumns: "200px 1fr", gap: 20 }}>
      <Card style={{ overflow: "hidden", height: "fit-content" }}>
        {secciones.map((s) => (
          <button
            key={s}
            onClick={() => setSec(s)}
            style={{
              display: "block",
              width: "100%",
              textAlign: "left",
              padding: "10px 14px",
              fontSize: 12.5,
              color: sec === s ? obra : ink,
              fontWeight: sec === s ? 600 : 400,
              background: sec === s ? obraSoft : "transparent",
              border: "none",
              borderBottom: `1px solid ${rule}`,
              borderLeft: `3px solid ${sec === s ? obra : "transparent"}`,
              cursor: "pointer",
              fontFamily: "'IBM Plex Sans', sans-serif",
            }}
          >
            {s}
          </button>
        ))}
      </Card>

      <Card style={{ padding: "22px 24px" }}>
        <div
          style={{
            fontFamily: "'IBM Plex Serif', serif",
            fontWeight: 700,
            fontSize: 18,
            marginBottom: 20,
            color: ink,
          }}
        >
          {sec}
        </div>

        {sec === "Datos generales" && (
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
            {[
              ["Número de contrato", mockContrato.numero],
              ["Tipo de contrato", mockContrato.tipo],
              ["Monto contratado", fmtMXN(mockContrato.monto)],
              ["Anticipo otorgado", `${fmtMXN(mockContrato.anticipo)} (10%)`],
              ["Fecha de inicio", mockContrato.fechaInicio],
              ["Fecha de término", mockContrato.fechaTermino],
              ["Contratista", mockContrato.contratista],
              ["Dependencia contratante", mockContrato.dependencia],
              ["Residente de obra", mockContrato.residente],
              ["Supervisor externo", mockContrato.supervisor],
            ].map(([l, v]) => (
              <Field key={l} label={l} value={v} />
            ))}
          </div>
        )}

        {sec === "Catálogo" && (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["Clave", "Descripción", "Unidad", "P.U.", ""]} />
            <tbody>
              {mockCatalogoConceptos.map((c, i) => (
                <tr key={c.clave} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                      color: obra,
                    }}
                  >
                    {c.clave}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    {c.desc}
                  </td>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                    }}
                  >
                    {c.unidad}
                  </td>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                    }}
                  >
                    {fmtMXN(c.precioUnitario)}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    <button
                      style={{
                        fontSize: 11.5,
                        color: muted,
                        background: "none",
                        border: `1px solid ${rule}`,
                        borderRadius: 3,
                        padding: "3px 8px",
                        cursor: "pointer",
                        display: "flex",
                        alignItems: "center",
                        gap: 4,
                      }}
                    >
                      <Download size={11} /> Descargar
                    </button>
                  </td>
                </tr>
              ))}
              <tr style={{ background: ink }}>
                <td
                  colSpan={3}
                  style={{
                    padding: "10px 14px",
                    color: paper2,
                    fontSize: 11,
                    fontWeight: 600,
                    textTransform: "uppercase",
                    letterSpacing: "0.05em",
                  }}
                >
                  Importe total del contrato
                </td>
                <td
                  colSpan={2}
                  style={{
                    padding: "10px 14px",
                    color: paper2,
                    fontFamily: "JetBrains Mono",
                    fontWeight: 700,
                    fontSize: 13,
                  }}
                >
                  {fmtMXN(mockContrato.monto)}
                </td>
              </tr>
            </tbody>
          </table>
        )}

        {sec === "Garantías" && (
          <div>
            {mockContrato.garantias.map((g) => (
              <div
                key={g.tipo}
                style={{
                  border: `1px solid ${rule}`,
                  borderRadius: 4,
                  padding: "16px 18px",
                  marginBottom: 12,
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <div>
                  <div style={{ fontWeight: 600, fontSize: 13, marginBottom: 4 }}>
                    {g.tipo}
                  </div>
                  <div style={{ fontSize: 12, color: muted }}>
                    {fmtMXN(g.monto)} · Vence: {g.vencimiento}
                  </div>
                </div>
                <div style={{ textAlign: "right" }}>
                  <EstadoBadge estado={g.diasRestantes < 30 ? "por vencer" : "vigente"} />
                  <div
                    style={{
                      fontSize: 11,
                      color: g.diasRestantes < 30 ? folio : muted,
                      marginTop: 6,
                      fontFamily: "JetBrains Mono",
                    }}
                  >
                    {g.diasRestantes} días restantes
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {sec === "Programa de obra" && (
          <div>
            <div style={{ marginBottom: 16 }}>
              <ResponsiveContainer width="100%" height={200}>
                <LineChart
                  data={mockCurvaS}
                  margin={{ top: 4, right: 8, bottom: 0, left: -20 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke={rule} />
                  <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted }} />
                  <YAxis tick={{ fontSize: 11, fill: muted }} unit="%" domain={[0, 100]} />
                  <Tooltip
                    contentStyle={{
                      background: paper2,
                      border: `1px solid ${rule}`,
                      borderRadius: 3,
                      fontSize: 12,
                    }}
                    formatter={(v: number | null) => (v !== null ? [`${v}%`] : ["—"])}
                  />
                  <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
                  <Line
                    dataKey="programado"
                    name="Programado"
                    stroke={obra}
                    strokeWidth={2}
                    dot={false}
                  />
                  <Line
                    dataKey="real"
                    name="Real"
                    stroke={folio}
                    strokeWidth={2}
                    strokeDasharray="5 4"
                    dot={false}
                    connectNulls={false}
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
            <p style={{ fontSize: 12, color: muted }}>
              El programa se actualiza automáticamente al aprobarse convenios modificatorios.
            </p>
          </div>
        )}

        {(sec === "Estimaciones" || sec === "Bitácora" || sec === "Convenios") && (
          <div style={{ color: muted, fontSize: 12.5 }}>
            <p style={{ marginBottom: 12 }}>
              Vista unificada del expediente — navega a la pestaña correspondiente para el detalle.
            </p>
            <div style={{ display: "grid", gap: 8 }}>
              {sec === "Estimaciones" &&
                mockEstimaciones.slice(0, 3).map((e) => (
                  <div
                    key={e.id}
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      padding: "10px 14px",
                      border: `1px solid ${rule}`,
                      borderRadius: 3,
                      alignItems: "center",
                    }}
                  >
                    <span
                      style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}
                    >
                      EST-{String(e.id).padStart(3, "0")}
                    </span>
                    <span style={{ fontSize: 12 }}>{e.periodo}</span>
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5 }}>
                      {fmtMXN(e.monto)}
                    </span>
                    <EstadoBadge estado={e.estado} />
                    <button
                      style={{
                        fontSize: 11.5,
                        color: muted,
                        background: "none",
                        border: `1px solid ${rule}`,
                        borderRadius: 3,
                        padding: "3px 8px",
                        cursor: "pointer",
                        display: "flex",
                        alignItems: "center",
                        gap: 4,
                      }}
                    >
                      <Download size={11} /> PDF
                    </button>
                  </div>
                ))}
              {sec === "Bitácora" &&
                mockBitacora.slice(0, 4).map((b) => (
                  <div
                    key={b.id}
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      padding: "10px 14px",
                      border: `1px solid ${rule}`,
                      borderRadius: 3,
                      alignItems: "center",
                    }}
                  >
                    <span
                      style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}
                    >
                      {b.folio}
                    </span>
                    <span style={{ fontSize: 12 }}>{b.asunto}</span>
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>
                      {b.fecha}
                    </span>
                    <button
                      style={{
                        fontSize: 11.5,
                        color: muted,
                        background: "none",
                        border: `1px solid ${rule}`,
                        borderRadius: 3,
                        padding: "3px 8px",
                        cursor: "pointer",
                        display: "flex",
                        alignItems: "center",
                        gap: 4,
                      }}
                    >
                      <Download size={11} /> PDF
                    </button>
                  </div>
                ))}
              {sec === "Convenios" &&
                mockConvenios.map((c) => (
                  <div
                    key={c.id}
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      padding: "10px 14px",
                      border: `1px solid ${rule}`,
                      borderRadius: 3,
                      alignItems: "center",
                    }}
                  >
                    <span
                      style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}
                    >
                      {c.id}
                    </span>
                    <span style={{ fontSize: 12 }}>{c.tipo}</span>
                    <EstadoBadge estado={c.estado} />
                    <button
                      style={{
                        fontSize: 11.5,
                        color: muted,
                        background: "none",
                        border: `1px solid ${rule}`,
                        borderRadius: 3,
                        padding: "3px 8px",
                        cursor: "pointer",
                        display: "flex",
                        alignItems: "center",
                        gap: 4,
                      }}
                    >
                      <Download size={11} /> PDF
                    </button>
                  </div>
                ))}
            </div>
          </div>
        )}

        {sec === "Documentación" && (
          <div>
            {[
              "Contrato original firmado",
              "Programa de obra original",
              "Acta de inicio",
              "Dictamen técnico previo",
            ].map((doc) => (
              <div
                key={doc}
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                  padding: "10px 14px",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  marginBottom: 8,
                }}
              >
                <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                  <FileText size={15} color={muted} />
                  <span style={{ fontSize: 12.5 }}>{doc}</span>
                </div>
                <button
                  style={{
                    fontSize: 11.5,
                    color: obra,
                    background: "none",
                    border: `1px solid ${rule}`,
                    borderRadius: 3,
                    padding: "4px 10px",
                    cursor: "pointer",
                    display: "flex",
                    alignItems: "center",
                    gap: 4,
                    fontFamily: "'IBM Plex Sans', sans-serif",
                  }}
                >
                  <Download size={12} /> Descargar
                </button>
              </div>
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}
