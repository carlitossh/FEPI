import { useEffect, useState } from "react";
import { Download, FileText } from "lucide-react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { Field } from "../../../components/Field";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink, paper2, rule, obra, obraSoft, folio, muted,
} from "../../../styles/theme";
import type { SeccionExpediente } from "../types";

const API = "http://localhost:5000/api";
const CONTRATO_ID = 1;
const BITACORA_ID = 1;

const TIPO_CONTRATO: Record<number, string> = { 1: "Obra Pública", 2: "Servicios relacionados" };
const TIPO_GARANTIA: Record<number, string> = { 1: "Anticipo", 2: "Cumplimiento", 3: "Vicios ocultos", 4: "Otra" };
const ESTADO_GARANTIA: Record<number, string> = { 1: "vigente", 2: "vencida", 3: "liberada", 4: "ejecutada" };
const ESTADO_ESTIMACION: Record<number, string> = { 1: "Borrador", 2: "Enviada", 3: "Observada", 4: "Aprobada", 5: "Rechazada", 6: "Pagada" };
const TIPO_CONVENIO: Record<number, string> = { 1: "Incremento de monto", 2: "Ampliación de plazo", 3: "Ajuste de catálogo" };
const ESTADO_CONVENIO: Record<number, string> = { 1: "En evaluación", 2: "Dictaminada", 3: "Promovida", 4: "Aprobado", 5: "Rechazado" };

function diasRestantes(fechaStr: string): number {
  return Math.ceil((new Date(fechaStr).getTime() - Date.now()) / 86_400_000);
}

interface TabExpedienteProps {
  rol: string;
}

export function TabExpediente({ rol: _rol }: TabExpedienteProps) {
  const secciones: SeccionExpediente[] = [
    "Datos generales", "Catálogo", "Programa de obra", "Garantías",
    "Estimaciones", "Bitácora", "Convenios", "Documentación",
  ];
  const [sec, setSec] = useState<SeccionExpediente>("Datos generales");
  const [contrato, setContrato] = useState<any>(null);
  const [curvaS, setCurvaS] = useState<any[]>([]);
  const [estimaciones, setEstimaciones] = useState<any[]>([]);
  const [eventos, setEventos] = useState<any[]>([]);
  const [convenios, setConvenios] = useState<any[]>([]);

  useEffect(() => {
    fetch(`${API}/contratos/${CONTRATO_ID}`)
      .then((r) => r.ok ? r.json() : null).then(setContrato).catch(() => {});
    fetch(`${API}/avance/contrato/${CONTRATO_ID}/curva-s`)
      .then((r) => r.ok ? r.json() : { puntos: [] }).then((d) => setCurvaS(d.puntos ?? [])).catch(() => {});
    fetch(`${API}/estimaciones/contrato/${CONTRATO_ID}`)
      .then((r) => r.ok ? r.json() : []).then(setEstimaciones).catch(() => {});
    fetch(`${API}/bitacora/${BITACORA_ID}/eventos`)
      .then((r) => r.ok ? r.json() : []).then(setEventos).catch(() => {});
    fetch(`${API}/contratos/${CONTRATO_ID}/convenios`)
      .then((r) => r.ok ? r.json() : []).then(setConvenios).catch(() => {});
  }, []);

  const curvaGrafica = curvaS.map((x) => ({
    mes: x.periodo?.substring(5) ?? x.periodo,
    programado: x.porcentajeProgramado,
    real: x.porcentajeReal,
  }));

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
        <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 18, marginBottom: 20, color: ink }}>
          {sec}
        </div>

        {sec === "Datos generales" && (
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
            {contrato ? (
              <>
                <Field label="Número de contrato" value={contrato.numeroContrato} />
                <Field label="Tipo de contrato" value={TIPO_CONTRATO[contrato.tipo] ?? String(contrato.tipo)} />
                <Field label="Monto contratado" value={fmtMXN(contrato.montoContratado)} />
                <Field label="Importe total catálogo" value={fmtMXN(contrato.importeTotalCatalogo)} />
                <Field label="Fecha de inicio" value={contrato.fechaInicio} />
                <Field label="Fecha de término" value={contrato.fechaTermino} />
                <Field label="Dependencia contratante" value={contrato.dependenciaContratante} />
                <Field label="Contratista" value={contrato.contratistaEmpresa} />
              </>
            ) : (
              <div style={{ fontSize: 12.5, color: muted, gridColumn: "span 2" }}>Cargando datos...</div>
            )}
          </div>
        )}

        {sec === "Catálogo" && (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["Clave", "Descripción", "Unidad", "Cant.", "P.U.", "Importe"]} />
            <tbody>
              {(contrato?.conceptoContratos ?? []).map((c: any, i: number) => (
                <tr key={c.id} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{c.clave}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{c.descripcion}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.unidadMedida}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.cantidadContratada}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(c.precioUnitario)}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, fontWeight: 600 }}>{fmtMXN(c.importe)}</td>
                </tr>
              ))}
              <tr style={{ background: ink }}>
                <td colSpan={5} style={{ padding: "10px 14px", color: paper2, fontSize: 11, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.05em" }}>
                  Importe total del contrato
                </td>
                <td style={{ padding: "10px 14px", color: paper2, fontFamily: "JetBrains Mono", fontWeight: 700, fontSize: 13 }}>
                  {fmtMXN(contrato?.montoContratado ?? 0)}
                </td>
              </tr>
            </tbody>
          </table>
        )}

        {sec === "Garantías" && (
          <div>
            {(contrato?.garantias ?? []).length === 0 && (
              <div style={{ fontSize: 12.5, color: muted }}>Sin garantías registradas.</div>
            )}
            {(contrato?.garantias ?? []).map((g: any) => {
              const dias = diasRestantes(g.vigencia);
              const estadoStr = ESTADO_GARANTIA[g.estado] ?? "vigente";
              return (
                <div
                  key={g.id}
                  style={{ border: `1px solid ${rule}`, borderRadius: 4, padding: "16px 18px", marginBottom: 12, display: "flex", justifyContent: "space-between", alignItems: "center" }}
                >
                  <div>
                    <div style={{ fontWeight: 600, fontSize: 13, marginBottom: 4 }}>
                      {TIPO_GARANTIA[g.tipo] ?? String(g.tipo)}
                    </div>
                    <div style={{ fontSize: 12, color: muted }}>
                      {fmtMXN(g.monto)} · Vence: {g.vigencia}
                    </div>
                  </div>
                  <div style={{ textAlign: "right" }}>
                    <EstadoBadge estado={dias < 30 ? "por vencer" : estadoStr} />
                    <div style={{ fontSize: 11, color: dias < 30 ? folio : muted, marginTop: 6, fontFamily: "JetBrains Mono" }}>
                      {dias} días restantes
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {sec === "Programa de obra" && (
          <div>
            <div style={{ marginBottom: 16 }}>
              <ResponsiveContainer width="100%" height={200}>
                <LineChart data={curvaGrafica} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke={rule} />
                  <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted }} />
                  <YAxis tick={{ fontSize: 11, fill: muted }} unit="%" domain={[0, 100]} />
                  <Tooltip
                    contentStyle={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 3, fontSize: 12 }}
                    formatter={(v: number | null) => (v !== null ? [`${v}%`] : ["—"])}
                  />
                  <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
                  <Line dataKey="programado" name="Programado" stroke={obra} strokeWidth={2} dot={false} />
                  <Line dataKey="real" name="Real" stroke={folio} strokeWidth={2} strokeDasharray="5 4" dot={false} connectNulls={false} />
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
                estimaciones.slice(0, 5).map((e: any) => (
                  <div
                    key={e.id}
                    style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}
                  >
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>
                      EST-{String(e.id).padStart(3, "0")}
                    </span>
                    <span style={{ fontSize: 12 }}>{e.periodo}</span>
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(e.montoEstimado ?? 0)}</span>
                    <EstadoBadge estado={ESTADO_ESTIMACION[e.estado] ?? String(e.estado)} />
                  </div>
                ))}
              {sec === "Bitácora" &&
                eventos.slice(0, 5).map((b: any, i: number) => (
                  <div
                    key={`${b.tipo}-${i}`}
                    style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}
                  >
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{b.folio}</span>
                    <span style={{ fontSize: 12 }}>{b.asunto}</span>
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>
                      {b.fecha ? new Date(b.fecha).toLocaleDateString("es-MX") : b.fechaRegistro ? new Date(b.fechaRegistro).toLocaleDateString("es-MX") : "—"}
                    </span>
                  </div>
                ))}
              {sec === "Convenios" &&
                convenios.map((c: any) => (
                  <div
                    key={c.id}
                    style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}
                  >
                    <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>
                      CM-{String(c.id).padStart(3, "0")}
                    </span>
                    <span style={{ fontSize: 12 }}>{TIPO_CONVENIO[c.tipo] ?? String(c.tipo)}</span>
                    <EstadoBadge estado={ESTADO_CONVENIO[c.estado] ?? String(c.estado)} />
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
                style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, marginBottom: 8 }}
              >
                <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                  <FileText size={15} color={muted} />
                  <span style={{ fontSize: 12.5 }}>{doc}</span>
                </div>
                <button
                  style={{ fontSize: 11.5, color: obra, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4, fontFamily: "'IBM Plex Sans', sans-serif" }}
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
