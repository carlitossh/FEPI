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
  rule, obra, obraSoft, folio, aprobado, aprobadoSoft, muted,
} from "../../../styles/theme";
import { C } from "../../../styles/theme";
import { contratosService } from "../services/contratosService";
import type { SeccionExpediente } from "../types";

const API = "http://localhost:5000/api";
const CONTRATO_ID = 1;
const BITACORA_ID = 1;

const TIPO_CONTRATO: Record<number, string> = { 1: "Obra Pública", 2: "Servicios relacionados" };
const TIPO_GARANTIA: Record<number, string> = { 1: "Anticipo", 2: "Cumplimiento", 3: "Vicios ocultos", 4: "Otra" };
const ESTADO_GARANTIA: Record<number, string> = { 1: "vigente", 2: "vencida", 3: "liberada", 4: "ejecutada" };
const ESTADO_ESTIMACION: Record<number, string> = {
  1: "Borrador", 2: "Enviada", 3: "ObservadaSupervision", 4: "AprobadaSupervision",
  5: "RechazadaResidencia", 6: "AprobadaResidencia", 7: "Cancelada",
};
const TIPO_CONVENIO: Record<number, string> = { 1: "Incremento de monto", 2: "Ampliación de plazo", 3: "Ajuste de catálogo" };
const ESTADO_CONVENIO: Record<number, string> = { 1: "En evaluación", 2: "Dictaminada", 3: "Promovida", 4: "Aprobado", 5: "Rechazado" };

function diasRestantes(fechaStr: string): number {
  return Math.ceil((new Date(fechaStr).getTime() - Date.now()) / 86_400_000);
}

interface ContratoForm {
  numeroContrato: string;
  nombreObra: string;
  tipo: string;
  periodoEstimacion: string;
  dependenciaContratante: string;
  contratistaEmpresa: string;
  contratistaRepresentante: string;
  residenteNombre: string;
  supervisorExternoNombre: string;
  superintendenteNombre: string;
  montoContratado: string;
  fechaInicio: string;
  fechaTermino: string;
}

const emptyForm: ContratoForm = {
  numeroContrato: "",
  nombreObra: "",
  tipo: "1",
  periodoEstimacion: "1",
  dependenciaContratante: "",
  contratistaEmpresa: "",
  contratistaRepresentante: "",
  residenteNombre: "",
  supervisorExternoNombre: "",
  superintendenteNombre: "",
  montoContratado: "",
  fechaInicio: "",
  fechaTermino: "",
};

function formFromContrato(c: any): ContratoForm {
  return {
    numeroContrato: c.numeroContrato ?? "",
    nombreObra: c.nombreObra ?? "",
    tipo: String(c.tipo ?? 1),
    periodoEstimacion: String(c.periodoEstimacion ?? 1),
    dependenciaContratante: c.dependenciaContratante ?? "",
    contratistaEmpresa: c.contratistaEmpresa ?? "",
    contratistaRepresentante: c.contratistaRepresentante ?? "",
    residenteNombre: c.residenteNombre ?? "",
    supervisorExternoNombre: c.supervisorExternoNombre ?? "",
    superintendenteNombre: c.superintendenteNombre ?? "",
    montoContratado: String(c.montoContratado ?? ""),
    fechaInicio: c.fechaInicio ?? "",
    fechaTermino: c.fechaTermino ?? "",
  };
}

const inputStyle = {
  border: `1px solid ${C.border}`,
  background: C.surface2,
  borderRadius: 10,
  padding: "9px 12px",
  fontSize: 12.5,
  color: C.fg,
  fontFamily: "'IBM Plex Sans', sans-serif",
  outline: "none",
  width: "100%",
  colorScheme: "dark",
  boxSizing: "border-box" as const,
};

const selectStyle = { ...inputStyle, cursor: "pointer" };

interface TabExpedienteProps {
  rol: string;
  onContratoSaved?: () => void;
}

export function TabExpediente({ rol: _rol, onContratoSaved }: TabExpedienteProps) {
  const secciones: SeccionExpediente[] = [
    "Datos generales", "Catálogo", "Programa de obra", "Garantías",
    "Estimaciones", "Bitácora", "Convenios", "Documentación",
  ];
  const [sec, setSec] = useState<SeccionExpediente>("Datos generales");
  const [contrato, setContrato] = useState<any>(null);
  const [dataLoaded, setDataLoaded] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [form, setForm] = useState<ContratoForm>(emptyForm);

  const [curvaS, setCurvaS] = useState<any[]>([]);
  const [estimaciones, setEstimaciones] = useState<any[]>([]);
  const [eventos, setEventos] = useState<any[]>([]);
  const [convenios, setConvenios] = useState<any[]>([]);

  const loadContrato = () => {
    fetch(`${API}/contratos/${CONTRATO_ID}`)
      .then((r) => (r.ok ? r.json() : null))
      .then((data) => {
        setContrato(data);
        setDataLoaded(true);
        if (data) {
          setForm(formFromContrato(data));
        } else {
          setIsEditing(true);
        }
      })
      .catch(() => {
        setDataLoaded(true);
        setIsEditing(true);
      });
  };

  useEffect(() => {
    loadContrato();
    fetch(`${API}/avance/contrato/${CONTRATO_ID}/curva-s`)
      .then((r) => (r.ok ? r.json() : { puntos: [] }))
      .then((d) => setCurvaS(d.puntos ?? []))
      .catch(() => {});
    fetch(`${API}/estimaciones/contrato/${CONTRATO_ID}`)
      .then((r) => (r.ok ? r.json() : []))
      .then(setEstimaciones)
      .catch(() => {});
    fetch(`${API}/bitacora/${BITACORA_ID}/eventos`)
      .then((r) => (r.ok ? r.json() : []))
      .then(setEventos)
      .catch(() => {});
    fetch(`${API}/contratos/${CONTRATO_ID}/convenios`)
      .then((r) => (r.ok ? r.json() : []))
      .then(setConvenios)
      .catch(() => {});
  }, []);

  const curvaGrafica = curvaS.map((x) => ({
    mes: x.periodo?.substring(5) ?? x.periodo,
    programado: x.porcentajeProgramado,
    real: x.porcentajeReal,
  }));

  const setField = (key: keyof ContratoForm, val: string) =>
    setForm((f) => ({ ...f, [key]: val }));

  const handleSave = async () => {
    setSaveError(null);
    setIsSaving(true);
    try {
      const payload = {
        numeroContrato: form.numeroContrato.trim(),
        nombreObra: form.nombreObra.trim(),
        tipo: parseInt(form.tipo),
        periodoEstimacion: parseInt(form.periodoEstimacion),
        dependenciaContratante: form.dependenciaContratante.trim(),
        contratistaEmpresa: form.contratistaEmpresa.trim(),
        contratistaRepresentante: form.contratistaRepresentante.trim(),
        residenteNombre: form.residenteNombre.trim(),
        supervisorExternoNombre: form.supervisorExternoNombre.trim(),
        superintendenteNombre: form.superintendenteNombre.trim(),
        montoContratado: parseFloat(form.montoContratado) || 0,
        fechaInicio: form.fechaInicio,
        fechaTermino: form.fechaTermino,
      };

      if (!contrato) {
        const id = await contratosService.crear({
          ...payload,
          conceptoContratos: [],
          programaObra: [],
          garantias: [],
        });
        const nuevo = await contratosService.obtener(id);
        setContrato(nuevo);
      } else {
        await contratosService.actualizar(CONTRATO_ID, payload);
        const updated = await contratosService.obtener(CONTRATO_ID);
        setContrato(updated);
      }

      setIsEditing(false);
      setSaveSuccess(true);
      setTimeout(() => setSaveSuccess(false), 4000);
      if (onContratoSaved) onContratoSaved();
    } catch (e: any) {
      setSaveError(e.message || "Error al guardar el contrato.");
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    if (contrato) {
      setForm(formFromContrato(contrato));
      setIsEditing(false);
      setSaveError(null);
    }
  };

  const labelStyle: React.CSSProperties = {
    fontSize: 10.5,
    fontWeight: 700,
    textTransform: "uppercase",
    letterSpacing: "0.06em",
    color: muted,
    marginBottom: 5,
    display: "block",
  };

  const sectionHeadStyle: React.CSSProperties = {
    fontSize: 10,
    fontWeight: 700,
    textTransform: "uppercase",
    letterSpacing: "0.1em",
    color: obra,
    borderBottom: `1px solid ${rule}`,
    paddingBottom: 6,
    marginTop: 24,
    marginBottom: 14,
  };

  const renderContratoForm = () => (
    <div>
      {!contrato && (
        <div
          style={{
            background: obraSoft,
            border: `1px solid ${obra}`,
            borderRadius: 4,
            padding: "12px 16px",
            marginBottom: 20,
            fontSize: 12.5,
            color: obra,
          }}
        >
          <strong>Primera vez en el sistema.</strong> Completa los datos del contrato para comenzar a usar FEPI.
        </div>
      )}

      {saveSuccess && (
        <div
          style={{
            background: aprobadoSoft,
            border: `1px solid ${aprobado}`,
            borderRadius: 4,
            padding: "10px 16px",
            marginBottom: 16,
            fontSize: 12.5,
            color: aprobado,
          }}
        >
          ✓ Contrato guardado correctamente.
        </div>
      )}

      {saveError && (
        <div
          style={{
            background: "#FCE8E8",
            border: `1px solid ${folio}`,
            borderRadius: 4,
            padding: "10px 16px",
            marginBottom: 16,
            fontSize: 12.5,
            color: folio,
          }}
        >
          {saveError}
        </div>
      )}

      <div style={sectionHeadStyle}>Datos generales</div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "14px 20px" }}>
        <div>
          <label style={labelStyle}>Número de contrato *</label>
          <input
            style={inputStyle}
            value={form.numeroContrato}
            onChange={(e) => setField("numeroContrato", e.target.value)}
            placeholder="Ej. CT-2026-001"
          />
        </div>
        <div>
          <label style={labelStyle}>Nombre de la obra</label>
          <input
            style={inputStyle}
            value={form.nombreObra}
            onChange={(e) => setField("nombreObra", e.target.value)}
            placeholder="Nombre descriptivo de la obra"
          />
        </div>
        <div>
          <label style={labelStyle}>Tipo de contrato *</label>
          <select style={selectStyle} value={form.tipo} onChange={(e) => setField("tipo", e.target.value)}>
            <option value="1">Obra Pública</option>
            <option value="2">Servicios relacionados</option>
          </select>
        </div>
        <div>
          <label style={labelStyle}>Periodo de estimación *</label>
          <select style={selectStyle} value={form.periodoEstimacion} onChange={(e) => setField("periodoEstimacion", e.target.value)}>
            <option value="1">Mensual</option>
            <option value="2">Quincenal</option>
          </select>
        </div>
        <div>
          <label style={labelStyle}>Dependencia contratante *</label>
          <input
            style={inputStyle}
            value={form.dependenciaContratante}
            onChange={(e) => setField("dependenciaContratante", e.target.value)}
            placeholder="Ej. SOPOT Edo. de México"
          />
        </div>
        <div>
          <label style={labelStyle}>Empresa contratista *</label>
          <input
            style={inputStyle}
            value={form.contratistaEmpresa}
            onChange={(e) => setField("contratistaEmpresa", e.target.value)}
            placeholder="Razón social del contratista"
          />
        </div>
        <div style={{ gridColumn: "span 2" }}>
          <label style={labelStyle}>Representante legal del contratista</label>
          <input
            style={inputStyle}
            value={form.contratistaRepresentante}
            onChange={(e) => setField("contratistaRepresentante", e.target.value)}
            placeholder="Nombre del representante legal"
          />
        </div>
      </div>

      <div style={sectionHeadStyle}>Responsables</div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "14px 20px" }}>
        <div>
          <label style={labelStyle}>Residente de obra</label>
          <input
            style={inputStyle}
            value={form.residenteNombre}
            onChange={(e) => setField("residenteNombre", e.target.value)}
            placeholder="Nombre del residente"
          />
        </div>
        <div>
          <label style={labelStyle}>Supervisor externo</label>
          <input
            style={inputStyle}
            value={form.supervisorExternoNombre}
            onChange={(e) => setField("supervisorExternoNombre", e.target.value)}
            placeholder="Nombre del supervisor"
          />
        </div>
        <div>
          <label style={labelStyle}>Superintendente</label>
          <input
            style={inputStyle}
            value={form.superintendenteNombre}
            onChange={(e) => setField("superintendenteNombre", e.target.value)}
            placeholder="Nombre del superintendente"
          />
        </div>
      </div>

      <div style={sectionHeadStyle}>Información financiera y fechas</div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "14px 20px" }}>
        <div>
          <label style={labelStyle}>Monto contratado (MXN) *</label>
          <input
            style={inputStyle}
            type="number"
            min="0"
            step="0.01"
            value={form.montoContratado}
            onChange={(e) => setField("montoContratado", e.target.value)}
            placeholder="0.00"
          />
        </div>
        <div>
          <label style={labelStyle}>Fecha de inicio *</label>
          <input
            style={inputStyle}
            type="date"
            value={form.fechaInicio}
            onChange={(e) => setField("fechaInicio", e.target.value)}
          />
        </div>
        <div>
          <label style={labelStyle}>Fecha de término *</label>
          <input
            style={inputStyle}
            type="date"
            value={form.fechaTermino}
            onChange={(e) => setField("fechaTermino", e.target.value)}
          />
        </div>
      </div>

      <div style={{ display: "flex", gap: 10, marginTop: 24 }}>
        <button
          onClick={handleSave}
          disabled={isSaving}
          style={{
            background: obra,
            color: "#fff",
            border: "none",
            borderRadius: 10,
            padding: "9px 20px",
            fontSize: 12.5,
            fontWeight: 600,
            cursor: isSaving ? "wait" : "pointer",
            fontFamily: "'IBM Plex Sans', sans-serif",
            opacity: isSaving ? 0.7 : 1,
          }}
        >
          {isSaving ? "Guardando..." : contrato ? "Guardar cambios" : "Registrar contrato"}
        </button>
        {contrato && (
          <button
            onClick={handleCancel}
            disabled={isSaving}
            style={{
              background: "transparent",
              color: muted,
              border: `1px solid ${rule}`,
              borderRadius: 3,
              padding: "9px 18px",
              fontSize: 12.5,
              cursor: "pointer",
              fontFamily: "'IBM Plex Sans', sans-serif",
            }}
          >
            Cancelar
          </button>
        )}
      </div>

      {!contrato && (
        <div style={{ marginTop: 20, fontSize: 12, color: muted }}>
          <strong>Pasos siguientes:</strong> Catálogo de conceptos → Programa de obra → Abrir bitácora → Registrar avances → Crear estimaciones
        </div>
      )}
    </div>
  );

  const renderContratoView = () => (
    <div>
      {saveSuccess && (
        <div
          style={{
            background: aprobadoSoft,
            border: `1px solid ${aprobado}`,
            borderRadius: 4,
            padding: "10px 16px",
            marginBottom: 16,
            fontSize: 12.5,
            color: aprobado,
          }}
        >
          ✓ Contrato actualizado correctamente.
        </div>
      )}
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 16 }}>
        <button
          onClick={() => { setIsEditing(true); setSaveSuccess(false); setSaveError(null); }}
          style={{
            background: "transparent",
            color: obra,
            border: `1px solid ${obra}`,
            borderRadius: 3,
            padding: "7px 16px",
            fontSize: 12,
            cursor: "pointer",
            fontFamily: "'IBM Plex Sans', sans-serif",
            fontWeight: 600,
          }}
        >
          Editar contrato
        </button>
      </div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
        <Field label="Número de contrato" value={contrato.numeroContrato} />
        <Field label="Nombre de la obra" value={contrato.nombreObra || "—"} />
        <Field label="Tipo de contrato" value={TIPO_CONTRATO[contrato.tipo] ?? String(contrato.tipo)} />
        <Field label="Periodo de estimación" value={contrato.periodoEstimacion === 1 ? "Mensual" : "Quincenal"} />
        <Field label="Monto contratado" value={fmtMXN(contrato.montoContratado)} />
        <Field label="Importe total catálogo" value={fmtMXN(contrato.importeTotalCatalogo)} />
        <Field label="Fecha de inicio" value={contrato.fechaInicio} />
        <Field label="Fecha de término" value={contrato.fechaTermino} />
        <Field label="Dependencia contratante" value={contrato.dependenciaContratante} />
        <Field label="Empresa contratista" value={contrato.contratistaEmpresa} />
        <Field label="Representante legal" value={contrato.contratistaRepresentante || "—"} />
        <div />
        <Field label="Residente de obra" value={contrato.residenteNombre || "—"} />
        <Field label="Supervisor externo" value={contrato.supervisorExternoNombre || "—"} />
        <Field label="Superintendente" value={contrato.superintendenteNombre || "—"} />
      </div>
    </div>
  );

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
              color: sec === s ? obra : C.fgMuted,
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
            color: C.fg,
          }}
        >
          {sec}
        </div>

        {sec === "Datos generales" && (
          <>
            {!dataLoaded && (
              <div style={{ fontSize: 12.5, color: muted }}>Cargando datos...</div>
            )}
            {dataLoaded && isEditing && renderContratoForm()}
            {dataLoaded && !isEditing && contrato && renderContratoView()}
          </>
        )}

        {sec === "Catálogo" && (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["Clave", "Descripción", "Unidad", "Cant.", "P.U.", "Importe"]} />
            <tbody>
              {(contrato?.conceptoContratos ?? []).map((c: any) => (
                <tr key={c.id} style={{ background: "transparent" }}>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{c.clave}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{c.descripcion}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.unidadMedida}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.cantidadContratada}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(c.precioUnitario)}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, fontWeight: 600 }}>{fmtMXN(c.importe)}</td>
                </tr>
              ))}
              <tr style={{ background: C.blue }}>
                <td colSpan={5} style={{ padding: "10px 14px", color: "#fff", fontSize: 11, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.05em" }}>
                  Importe total del contrato
                </td>
                <td style={{ padding: "10px 14px", color: "#fff", fontFamily: "JetBrains Mono", fontWeight: 700, fontSize: 13 }}>
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
                    contentStyle={{ background: C.surface2, border: `1px solid ${C.borderHi}`, borderRadius: 10, fontSize: 12, color: C.fg }}
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
