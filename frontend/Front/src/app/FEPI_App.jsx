import { useState, useRef } from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from "recharts";
import {
  Bell, FileText, TrendingUp, BookOpen, GitBranch, Folder,
  LayoutDashboard, ChevronRight, X, Upload, Download, Search,
  CheckCircle, Clock, AlertTriangle, Camera, Plus, Shield,
  Users, BarChart2, Eye, ChevronDown, Check, Pencil, DollarSign
} from "lucide-react";

// ─── API Layer (swap these for real endpoints) ────────────────────────────────
const API_BASE = "/api";

const api = {
  // Estimaciones
  getEstimaciones: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/estimaciones`),
  getEstimacion: (id) => fetch(`${API_BASE}/estimaciones/${id}`),
  crearEstimacion: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/estimaciones`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  actualizarEstimacion: (id, data) => fetch(`${API_BASE}/estimaciones/${id}`, { method: "PATCH", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  enviarEstimacion: (id) => fetch(`${API_BASE}/estimaciones/${id}/enviar`, { method: "POST" }),
  aprobarEstimacion: (id) => fetch(`${API_BASE}/estimaciones/${id}/aprobar`, { method: "POST" }),
  rechazarEstimacion: (id, motivo) => fetch(`${API_BASE}/estimaciones/${id}/rechazar`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ motivo }) }),
  observarEstimacion: (id) => fetch(`${API_BASE}/estimaciones/${id}/observar`, { method: "POST" }),
  registrarPago: (id, data) => fetch(`${API_BASE}/estimaciones/${id}/pago`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  addObservacion: (id, data) => fetch(`${API_BASE}/estimaciones/${id}/observaciones`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  addConcepto: (id, data) => fetch(`${API_BASE}/estimaciones/${id}/conceptos`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  getHistorial: (id) => fetch(`${API_BASE}/estimaciones/${id}/historial`),
  adjuntarDoc: (id, formData) => fetch(`${API_BASE}/estimaciones/${id}/documentos`, { method: "POST", body: formData }),
  vincularBitacora: (id, folios) => fetch(`${API_BASE}/estimaciones/${id}/bitacora`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ folios }) }),
  // Bitácora
  getBitacora: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/bitacora`),
  crearNota: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/bitacora/notas`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  crearMinuta: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/bitacora/minutas`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  crearIncidencia: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/bitacora/incidencias`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  firmarNota: (id, actor) => fetch(`${API_BASE}/bitacora/notas/${id}/firmar`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ actor }) }),
  abrirBitacora: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/bitacora/abrir`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  // Convenios
  getConvenios: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/convenios`),
  crearConvenio: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/convenios`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  dictaminarConvenio: (id, data) => fetch(`${API_BASE}/convenios/${id}/dictamen`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  promoverConvenio: (id) => fetch(`${API_BASE}/convenios/${id}/promover`, { method: "POST" }),
  resolverConvenio: (id, data) => fetch(`${API_BASE}/convenios/${id}/resolucion`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  // Avance
  getAvance: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/avance`),
  registrarAvance: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/avance`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
  // Contrato / Expediente
  getContrato: (id) => fetch(`${API_BASE}/contratos/${id}`),
  getAlertas: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/alertas`),
  // Finiquito
  calcularFiniquito: (contratoId) => fetch(`${API_BASE}/contratos/${contratoId}/finiquito`),
  registrarCierre: (contratoId, data) => fetch(`${API_BASE}/contratos/${contratoId}/cierre`, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(data) }),
};

// ─── Palette tokens ───────────────────────────────────────────────────────────
const ink = "#1A2238";
const paper = "#F7F5EF";
const paper2 = "#FFFFFF";
const rule = "#D8D2C2";
const folio = "#8C2F2F";
const folioSoft = "#F3E4E0";
const obra = "#1F3864";
const obraSoft = "#DCE6F2";
const aprobado = "#2D6A4F";
const aprobadoSoft = "#DEEFE5";
const observado = "#B8860B";
const observadoSoft = "#F6EAD0";
const muted = "#6B6557";
const pagado = "#5B4FCF";
const pagadoSoft = "#EDEAFF";

// ─── Types ────────────────────────────────────────────────────────────────────
// EstadoEst: "Borrador" | "Enviada" | "Observada" | "Aprobada" | "Pagada"
// EstadoConv: "En evaluación" | "Dictaminada" | "Promovida" | "Aprobado" | "Rechazado"
// RolUsuario: "Superintendente" | "Supervisor" | "Residente" | "Financiero" | "Dependencia" | "Administrador"

// ─── Mock data (replace with API calls) ───────────────────────────────────────
const mockEstimaciones = [
  { id: 14, periodo: "Jun 2026", monto: 1_248_300, estado: "Enviada", dias: "18/20", conceptos: [
    { clave: "03.012", desc: "Losa de cimentación f'c=250", unidad: "m³", cantEjecutada: 120, precioUnitario: 4050, importe: 486000 },
    { clave: "05.004", desc: "Trabe de liga eje B-C", unidad: "ml", cantEjecutada: 34, precioUnitario: 6185, importe: 210290, flag: true },
    { clave: "06.001", desc: "Castillo K-1", unidad: "pza", cantEjecutada: 18, precioUnitario: 5300, importe: 95400 },
    { clave: "07.002", desc: "Muro de block hueco 15cm", unidad: "m²", cantEjecutada: 240, precioUnitario: 1903, importe: 456720 },
  ], notasVinculadas: ["0138", "0140", "0141"], observaciones: [
    { ref: "Concepto 05.004", txt: "El cálculo de la trabe no coincide con el generador adjunto: reporta 34 ml, el generador muestra 31 ml.", quien: "Ing. L. Martínez", hora: "hace 2 horas", color: observado },
    { ref: "General", txt: "Falta la firma del residente en la nota de bitácora 0141 referenciada.", quien: "Ing. L. Martínez", hora: "hace 2 horas", color: obra },
  ], historial: [
    { estadoAnterior: "—", estadoNuevo: "Borrador", fecha: "01-jun-2026 09:15", usuario: "Ing. R. Domínguez" },
    { estadoAnterior: "Borrador", estadoNuevo: "Enviada", fecha: "03-jun-2026 14:22", usuario: "Ing. R. Domínguez" },
  ]},
  { id: 13, periodo: "May 2026", monto: 980_200, estado: "Aprobada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 12, periodo: "Abr 2026", monto: 1_100_000, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 11, periodo: "Mar 2026", monto: 1_050_500, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
  { id: 10, periodo: "Feb 2026", monto: 890_000, estado: "Pagada", conceptos: [], notasVinculadas: [], observaciones: [], historial: [] },
];

const mockBitacora = [
  { id: 1, folio: "0001", tipo: "Apertura", fecha: "01-mar-2026", asunto: "Inicio formal de obra", contenido: "Se abre formalmente la bitácora del contrato CT-2026-014.", firmas: { superintendente: true, residente: true, supervisor: true }, folioRef: null },
  { id: 2, folio: "0138", tipo: "Nota", fecha: "03-jun-2026", asunto: "Terminación losa eje B", contenido: "Se concluye la losa del eje B conforme a proyecto.", firmas: { superintendente: true, residente: true, supervisor: true }, folioRef: "0001" },
  { id: 3, folio: "0140", tipo: "Nota", fecha: "09-jun-2026", asunto: "Terminación cimentación", contenido: "Se concluye la cimentación del eje C-D.", firmas: { superintendente: true, residente: true, supervisor: false }, folioRef: null },
  { id: 4, folio: "0141", tipo: "Nota", fecha: "14-jun-2026", asunto: "Entrega trabe principal", contenido: "Se entrega la trabe principal para revisión.", firmas: { superintendente: true, residente: true, supervisor: true }, folioRef: "0138" },
  { id: 5, folio: "INC-01", tipo: "Incidencia", fecha: "11-jun-2026", asunto: "Falla eléctrica temporal", contenido: "Se registró falla eléctrica en el área de cimbra.", firmas: { superintendente: false, residente: false, supervisor: false }, foto: true, folioRef: null },
];

const mockConvenios = [
  { id: "CM-008", tipo: "Ampliación de plazo", monto: null, justificacion: "Condiciones climatológicas atípicas en mayo impiden cumplir con el programa.", estado: "Promovida", dictamen: "Procedente", dictamenJust: "Se comprueba con registros meteorológicos adjuntos.", pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"] },
  { id: "CM-006", tipo: "Incremento de monto", monto: 450000, justificacion: "Volúmenes adicionales de excavación.", estado: "Aprobado", dictamen: "Procedente", dictamenJust: "", pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"] },
  { id: "CM-003", tipo: "Ajuste de catálogo", monto: 120000, justificacion: "Cambio de especificación en concreto.", estado: "Aprobado", dictamen: "Procedente", dictamenJust: "", pasos: ["Solicitada", "Dictaminada", "Promovida", "Resolución"] },
];

const mockCatalogoConceptos = [
  { clave: "01.001", desc: "Limpieza de terreno", unidad: "m²", precioUnitario: 45 },
  { clave: "02.001", desc: "Excavación manual", unidad: "m³", precioUnitario: 320 },
  { clave: "03.012", desc: "Losa de cimentación f'c=250", unidad: "m³", precioUnitario: 4050 },
  { clave: "05.004", desc: "Trabe de liga eje B-C", unidad: "ml", precioUnitario: 6185 },
  { clave: "06.001", desc: "Castillo K-1", unidad: "pza", precioUnitario: 5300 },
  { clave: "07.002", desc: "Muro de block hueco 15cm", unidad: "m²", precioUnitario: 1903 },
];

const mockAlertas = [
  { nivel: "rojo", texto: "Estimación 014 — vence en 2 días hábiles", ref: "EST-014" },
  { nivel: "amarillo", texto: "Nota 0140 — firma pendiente de Supervisión", ref: "FOL-0140" },
  { nivel: "amarillo", texto: "Garantía de cumplimiento — vence en 22 días", ref: "GAR-001" },
  { nivel: "gris", texto: "Convenio CM-008 — pendiente de resolución", ref: "CM-008" },
];

const mockCurvaS = [
  { mes: "Mar", programado: 8, real: 8 },
  { mes: "Abr", programado: 24, real: 22 },
  { mes: "May", programado: 45, real: 41 },
  { mes: "Jun", programado: 68, real: 63 },
  { mes: "Jul", programado: 84, real: null },
  { mes: "Ago", programado: 95, real: null },
  { mes: "Sep", programado: 100, real: null },
];

const mockContrato = {
  id: "CT-2026-014",
  numero: "CT-2026-014",
  tipo: "A precio unitario",
  monto: 11_000_000,
  anticipo: 1_100_000,
  fechaInicio: "01 marzo 2026",
  fechaTermino: "30 septiembre 2026",
  contratista: "Constructora Domínguez S.A. de C.V.",
  dependencia: "SOPOT Edo. de México",
  residente: "Ing. A. Herrera",
  supervisor: "Ing. L. Martínez",
  superintendente: "Ing. R. Domínguez",
  descripcion: "Pavimentación Av. Reforma Norte",
  garantias: [
    { tipo: "Cumplimiento", monto: 550_000, vencimiento: "30-sep-2026", estado: "vigente", diasRestantes: 99 },
    { tipo: "Anticipo", monto: 1_100_000, vencimiento: "30-jul-2026", estado: "vigente", diasRestantes: 37 },
    { tipo: "Vicios ocultos", monto: 110_000, vencimiento: "30-mar-2027", estado: "vigente", diasRestantes: 280 },
  ],
};

// ─── Helpers ──────────────────────────────────────────────────────────────────
function fmtMXN(n) {
  return new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN", maximumFractionDigits: 0 }).format(n);
}

function contarFirmas(firmas) {
  const vals = Object.values(firmas);
  return `${vals.filter(Boolean).length}/${vals.length}`;
}

function EstadoBadge({ estado }) {
  const map = {
    Borrador: { bg: "#E8ECF2", color: "#4A5568" },
    Enviada: { bg: obraSoft, color: obra },
    Observada: { bg: observadoSoft, color: observado },
    Aprobada: { bg: aprobadoSoft, color: aprobado },
    Pagada: { bg: pagadoSoft, color: pagado },
    "En evaluación": { bg: observadoSoft, color: observado },
    Dictaminada: { bg: obraSoft, color: obra },
    Promovida: { bg: observadoSoft, color: "#7A5A00" },
    Aprobado: { bg: aprobadoSoft, color: aprobado },
    Rechazado: { bg: folioSoft, color: folio },
    vigente: { bg: aprobadoSoft, color: aprobado },
    "por vencer": { bg: observadoSoft, color: observado },
    vencida: { bg: folioSoft, color: folio },
  };
  const s = map[estado] ?? { bg: "#eee", color: "#666" };
  return (
    <span style={{ background: s.bg, color: s.color, fontSize: 10.5, fontWeight: 600, padding: "3px 9px", borderRadius: 11, fontFamily: "'IBM Plex Sans', sans-serif", letterSpacing: "0.02em", whiteSpace: "nowrap" }}>
      {estado}
    </span>
  );
}

function SectionLabel({ children }) {
  return <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.06em", color: muted, fontWeight: 600, marginBottom: 6 }}>{children}</div>;
}

function Card({ children, style = {} }) {
  return <div style={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 4, ...style }}>{children}</div>;
}

function PrimaryBtn({ children, onClick, disabled, style = {} }) {
  return (
    <button onClick={onClick} disabled={disabled} style={{ background: disabled ? "#C9C2AC" : obra, color: "#fff", border: "none", borderRadius: 3, padding: "8px 16px", fontSize: 12.5, fontWeight: 600, cursor: disabled ? "not-allowed" : "pointer", fontFamily: "'IBM Plex Sans', sans-serif", ...style }}>
      {children}
    </button>
  );
}

function SecondaryBtn({ children, onClick, style = {}, color }) {
  return (
    <button onClick={onClick} style={{ background: "transparent", color: color || ink, border: `1px solid ${rule}`, borderRadius: 3, padding: "7px 14px", fontSize: 12, fontWeight: 500, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", ...style }}>
      {children}
    </button>
  );
}

function DangerBtn({ children, onClick, style = {} }) {
  return (
    <button onClick={onClick} style={{ background: folio, color: "#fff", border: "none", borderRadius: 3, padding: "8px 16px", fontSize: 12.5, fontWeight: 600, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", ...style }}>
      {children}
    </button>
  );
}

function Field({ label, value }) {
  return (
    <div>
      <SectionLabel>{label}</SectionLabel>
      <div style={{ fontSize: 13, color: ink, fontFamily: "JetBrains Mono", fontWeight: 500 }}>{value}</div>
    </div>
  );
}

function TextInput({ placeholder, value, onChange, style = {} }) {
  return (
    <input value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder} style={{ border: `1px solid ${rule}`, background: "#FAF8F2", borderRadius: 3, padding: "8px 12px", fontSize: 12.5, color: ink, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", width: "100%", boxSizing: "border-box", ...style }} />
  );
}

function TextArea({ placeholder, value, onChange, rows = 3 }) {
  return (
    <textarea value={value} onChange={e => onChange(e.target.value)} placeholder={placeholder} rows={rows} style={{ border: `1px solid ${rule}`, background: "#FAF8F2", borderRadius: 3, padding: "8px 12px", fontSize: 12.5, color: ink, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", width: "100%", boxSizing: "border-box", resize: "vertical" }} />
  );
}

function Modal({ title, subtitle, onClose, children, width = 480 }) {
  return (
    <div style={{ position: "fixed", inset: 0, background: "rgba(26,34,56,0.45)", zIndex: 100, display: "flex", alignItems: "center", justifyContent: "center", padding: 20 }}>
      <div style={{ background: paper2, borderRadius: 5, width, maxWidth: "100%", boxShadow: "0 20px 60px rgba(26,34,56,0.25)", overflow: "hidden", maxHeight: "90vh", display: "flex", flexDirection: "column" }}>
        <div style={{ background: obra, color: "#fff", padding: "14px 20px", display: "flex", justifyContent: "space-between", alignItems: "center", flexShrink: 0 }}>
          <div>
            {subtitle && <div style={{ fontFamily: "JetBrains Mono", fontSize: 10.5, opacity: 0.7, marginBottom: 2 }}>{subtitle}</div>}
            <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 16 }}>{title}</div>
          </div>
          <button onClick={onClose} style={{ background: "none", border: "none", color: "#fff", cursor: "pointer", padding: 4 }}><X size={18} /></button>
        </div>
        <div style={{ padding: 20, overflowY: "auto" }}>{children}</div>
      </div>
    </div>
  );
}

function TableHeader({ cols }) {
  return (
    <thead>
      <tr style={{ background: ink }}>
        {cols.map(h => (
          <th key={h} style={{ textAlign: "left", padding: "10px 14px", color: paper, fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.06em", fontWeight: 600, whiteSpace: "nowrap" }}>{h}</th>
        ))}
      </tr>
    </thead>
  );
}

function AlertsPanel() {
  return (
    <Card style={{ padding: "18px 16px", height: "fit-content" }}>
      <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 14, display: "flex", alignItems: "center", gap: 6 }}>
        <Bell size={12} /> Alertas activas
      </div>
      {mockAlertas.map((a, i) => {
        const color = a.nivel === "rojo" ? folio : a.nivel === "amarillo" ? observado : "#999";
        const bg = a.nivel === "rojo" ? folioSoft : a.nivel === "amarillo" ? observadoSoft : "#F5F5F5";
        return (
          <div key={i} style={{ display: "flex", alignItems: "flex-start", gap: 8, padding: "9px 10px", background: bg, borderRadius: 3, marginBottom: 8, borderLeft: `3px solid ${color}` }}>
            <div style={{ width: 7, height: 7, borderRadius: "50%", background: color, flexShrink: 0, marginTop: 4 }} />
            <div>
              <div style={{ fontSize: 11.5, color: ink, lineHeight: 1.4 }}>{a.texto}</div>
              <div style={{ fontFamily: "JetBrains Mono", fontSize: 10, color, marginTop: 2, fontWeight: 600 }}>{a.ref}</div>
            </div>
          </div>
        );
      })}
    </Card>
  );
}

// ─── HU-01 / HU-02 / HU-03 / HU-04 / HU-05 / HU-06: Estimaciones ────────────
function TabEstimaciones({ rol }) {
  const [estimaciones, setEstimaciones] = useState(mockEstimaciones);
  const [selected, setSelected] = useState(null);
  const [filtroEstado, setFiltroEstado] = useState("Todos");
  const [busqueda, setBusqueda] = useState("");
  const [obsTab, setObsTab] = useState("Carátula");
  const [showNuevaEst, setShowNuevaEst] = useState(false);
  const [showAddConcepto, setShowAddConcepto] = useState(false);
  const [showVincularBitacora, setShowVincularBitacora] = useState(false);
  const [showObservacion, setShowObservacion] = useState(false);
  const [showRechazo, setShowRechazo] = useState(false);
  const [showPago, setShowPago] = useState(false);
  const [showHistorial, setShowHistorial] = useState(false);
  const [nuevaObs, setNuevaObs] = useState("");
  const [motivoRechazo, setMotivoRechazo] = useState("");
  const [datoPago, setDatoPago] = useState({ referencia: "", banco: "", fecha: "", monto: "" });
  const fileRef = useRef();

  const puedeEnviar = rol === "Superintendente";
  const puedeObservar = rol === "Supervisor";
  const puedeAprobarRechazar = rol === "Residente";
  const puedePagar = rol === "Financiero";
  const esBorrador = selected?.estado === "Borrador";
  const esEnviada = selected?.estado === "Enviada";
  const esAprobada = selected?.estado === "Aprobada";

  const filtradas = estimaciones
    .filter(e => filtroEstado === "Todos" || e.estado === filtroEstado)
    .filter(e => busqueda === "" || String(e.id).includes(busqueda) || e.periodo.toLowerCase().includes(busqueda.toLowerCase()));

  // Solo financiero ve solo aprobadas
  const visibles = puedePagar ? estimaciones.filter(e => e.estado === "Aprobada") : filtradas;

  function handleEnviar() {
    // api.enviarEstimacion(selected.id)
    setEstimaciones(prev => prev.map(e => e.id === selected.id ? { ...e, estado: "Enviada" } : e));
    setSelected(prev => ({ ...prev, estado: "Enviada" }));
  }
  function handleAprobar() {
    // api.aprobarEstimacion(selected.id)
    setEstimaciones(prev => prev.map(e => e.id === selected.id ? { ...e, estado: "Aprobada" } : e));
    setSelected(prev => ({ ...prev, estado: "Aprobada" }));
  }
  function handleRechazar() {
    if (!motivoRechazo.trim()) return;
    // api.rechazarEstimacion(selected.id, motivoRechazo)
    setEstimaciones(prev => prev.map(e => e.id === selected.id ? { ...e, estado: "Borrador" } : e));
    setSelected(prev => ({ ...prev, estado: "Borrador" }));
    setShowRechazo(false);
    setMotivoRechazo("");
  }
  function handleObservar() {
    // api.observarEstimacion(selected.id)
    setEstimaciones(prev => prev.map(e => e.id === selected.id ? { ...e, estado: "Observada" } : e));
    setSelected(prev => ({ ...prev, estado: "Observada" }));
  }
  function handlePago() {
    if (!datoPago.referencia || !datoPago.banco || !datoPago.fecha || !datoPago.monto) return;
    // api.registrarPago(selected.id, datoPago)
    setEstimaciones(prev => prev.map(e => e.id === selected.id ? { ...e, estado: "Pagada" } : e));
    setSelected(prev => ({ ...prev, estado: "Pagada" }));
    setShowPago(false);
    setDatoPago({ referencia: "", banco: "", fecha: "", monto: "" });
  }
  function handleNuevaObs() {
    if (!nuevaObs.trim()) return;
    const obs = { ref: "General", txt: nuevaObs, quien: `${rol}`, hora: "ahora", color: obra };
    // api.addObservacion(selected.id, obs)
    setSelected(prev => ({ ...prev, observaciones: [...(prev.observaciones || []), obs] }));
    setNuevaObs("");
    setShowObservacion(false);
  }

  const detailTabs = ["Carátula", "Generadores", `Notas vinculadas (${selected?.notasVinculadas?.length || 0})`, "Historial"];

  if (selected) {
    const totalEst = selected.conceptos?.reduce((a, c) => a + (c.importe || 0), 0) || selected.monto;
    const acumulado = 9_450_000;
    const saldo = mockContrato.monto - acumulado;
    return (
      <div>
        <button onClick={() => setSelected(null)} style={{ display: "flex", alignItems: "center", gap: 6, fontSize: 12, color: obra, background: "none", border: "none", cursor: "pointer", padding: "0 0 16px 0", fontFamily: "'IBM Plex Sans', sans-serif" }}>
          ← Volver a bandeja
        </button>
        {/* Header */}
        <div style={{ background: ink, color: paper, borderRadius: "4px 4px 0 0", overflow: "hidden" }}>
          <div style={{ background: folio, display: "flex", justifyContent: "space-between", padding: "5px 20px", fontSize: 10.5, letterSpacing: "0.06em" }}>
            <span>SELLO DIGITAL · FOLIO-EST-{String(selected.id).padStart(3, "0")}</span>
            <span>Plazo legal: Art. 55 LOPSRM — 20 días hábiles</span>
          </div>
          <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "14px 20px" }}>
            <div>
              <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 18 }}>Estimación N.° {String(selected.id).padStart(3, "0")}</div>
              <div style={{ fontSize: 11, color: "#C7C2B0", marginTop: 2 }}>Contrato {mockContrato.id} · Periodo {selected.periodo} · Superintendente: Ing. R. Domínguez</div>
            </div>
            <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
              <EstadoBadge estado={selected.estado} />
              {selected.estado === "Aprobada" && (
                <button onClick={() => setShowHistorial(true)} style={{ fontSize: 11, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", display: "flex", alignItems: "center", gap: 4 }}>
                  <Clock size={11} /> Historial
                </button>
              )}
            </div>
          </div>
        </div>
        {/* Tabs */}
        <div style={{ display: "flex", background: "#EFEAE0", borderBottom: `1px solid ${rule}` }}>
          {detailTabs.map(t => (
            <button key={t} onClick={() => setObsTab(t)} style={{ padding: "10px 18px", fontSize: 12.5, color: obsTab === t ? obra : muted, border: "none", borderBottom: `3px solid ${obsTab === t ? obra : "transparent"}`, background: obsTab === t ? paper2 : "transparent", fontWeight: obsTab === t ? 600 : 400, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}>
              {t}
            </button>
          ))}
        </div>
        {/* Body */}
        {obsTab === "Carátula" && (
          <div style={{ display: "grid", gridTemplateColumns: "1fr 290px", border: `1px solid ${rule}`, borderTop: "none", background: paper2 }}>
            <div style={{ padding: "20px 22px", borderRight: `1px solid ${rule}` }}>
              {selected.dias && (
                <div style={{ display: "flex", alignItems: "center", gap: 12, background: observadoSoft, border: `1px solid ${rule}`, borderLeft: `3px solid ${observado}`, borderRadius: 3, padding: "10px 14px", marginBottom: 18 }}>
                  <div style={{ width: 38, height: 38, borderRadius: "50%", border: `4px solid ${observado}`, display: "flex", alignItems: "center", justifyContent: "center", fontSize: 11, fontWeight: 700, color: observado, flexShrink: 0, fontFamily: "JetBrains Mono" }}>{selected.dias}</div>
                  <div style={{ fontSize: 12, color: muted }}>Quedan <strong style={{ color: ink }}>2 días hábiles</strong> antes del vencimiento del plazo legal.</div>
                </div>
              )}
              <div style={{ display: "flex", gap: 12, marginBottom: 20 }}>
                {[
                  { v: fmtMXN(totalEst), l: "Monto estimado" },
                  { v: fmtMXN(acumulado), l: "Acumulado del contrato" },
                  { v: fmtMXN(saldo), l: "Saldo pendiente" },
                ].map(k => (
                  <div key={k.l} style={{ flex: 1, border: `1px solid ${rule}`, borderRadius: 4, padding: "12px 14px" }}>
                    <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 16, fontWeight: 700, color: obra }}>{k.v}</div>
                    <div style={{ fontSize: 10, textTransform: "uppercase", letterSpacing: "0.04em", color: muted, marginTop: 2 }}>{k.l}</div>
                  </div>
                ))}
              </div>
              {/* Tabla de conceptos */}
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
                <TableHeader cols={["Clave", "Descripción", "Unidad", "Cant.", "P.U.", "Importe"]} />
                <tbody>
                  {(selected.conceptos || []).map(r => (
                    <tr key={r.clave} style={{ background: r.flag ? folioSoft : "transparent" }}>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, borderLeft: r.flag ? `3px solid ${folio}` : "none" }}>{r.clave}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontSize: 12.5 }}>{r.desc}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{r.unidad}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{r.cantEjecutada}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(r.precioUnitario)}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, fontWeight: 600, color: r.flag ? folio : ink }}>{fmtMXN(r.importe)}</td>
                    </tr>
                  ))}
                  {(selected.conceptos || []).length === 0 && (
                    <tr><td colSpan={6} style={{ padding: "20px 14px", color: muted, fontSize: 12, textAlign: "center" }}>Sin conceptos registrados</td></tr>
                  )}
                </tbody>
              </table>
              {/* Resumen por partida */}
              {(selected.conceptos || []).length > 0 && (
                <div style={{ marginTop: 16, padding: "12px 14px", background: "#FAF8F2", border: `1px solid ${rule}`, borderRadius: 3 }}>
                  <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.05em", color: muted, marginBottom: 8 }}>Resumen — total estimado</div>
                  <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 20, fontWeight: 700, color: obra }}>{fmtMXN(totalEst)}</div>
                </div>
              )}
              {/* Acciones según rol y estado */}
              <div style={{ display: "flex", gap: 10, marginTop: 20, flexWrap: "wrap" }}>
                {esBorrador && puedeEnviar && (
                  <>
                    <PrimaryBtn onClick={() => setShowAddConcepto(true)}>+ Añadir concepto</PrimaryBtn>
                    <SecondaryBtn onClick={() => setShowVincularBitacora(true)}>Vincular bitácora</SecondaryBtn>
                    <SecondaryBtn onClick={() => fileRef.current?.click()}>
                      <Upload size={12} style={{ marginRight: 4 }} /> Adjuntar docs
                    </SecondaryBtn>
                    <input ref={fileRef} type="file" multiple style={{ display: "none" }} onChange={(e) => { /* api.adjuntarDoc */ }} />
                    <PrimaryBtn onClick={handleEnviar} style={{ background: folio, marginLeft: "auto" }}>Enviar estimación →</PrimaryBtn>
                  </>
                )}
                {esEnviada && puedeAprobarRechazar && (
                  <>
                    <PrimaryBtn onClick={handleAprobar} style={{ background: aprobado }}>✓ Aprobar</PrimaryBtn>
                    <DangerBtn onClick={() => setShowRechazo(true)}>Rechazar</DangerBtn>
                  </>
                )}
                {esEnviada && puedeObservar && (
                  <>
                    <SecondaryBtn onClick={() => setShowObservacion(true)}>+ Nueva observación</SecondaryBtn>
                    <SecondaryBtn onClick={handleObservar} color={observado}>Marcar como Observada</SecondaryBtn>
                    <PrimaryBtn onClick={handleAprobar} style={{ background: aprobado }}>Aprobar</PrimaryBtn>
                  </>
                )}
                {esAprobada && puedePagar && (
                  <PrimaryBtn onClick={() => setShowPago(true)} style={{ background: pagado }}>
                    <DollarSign size={13} style={{ marginRight: 4 }} /> Registrar pago
                  </PrimaryBtn>
                )}
                <SecondaryBtn onClick={() => setShowHistorial(true)}>
                  <Clock size={12} style={{ marginRight: 4 }} /> Ver historial
                </SecondaryBtn>
              </div>
              <p style={{ fontSize: 11, color: muted, marginTop: 8 }}>Documento del contratista — acceso restringido por rol.</p>
            </div>
            {/* Panel de observaciones */}
            <div style={{ padding: "20px 18px", background: "#FAF8F2" }}>
              <div style={{ fontSize: 11.5, fontWeight: 700, color: obra, textTransform: "uppercase", letterSpacing: "0.06em", marginBottom: 14 }}>Observaciones ({(selected.observaciones || []).length})</div>
              {(selected.observaciones || []).length === 0 && (
                <div style={{ fontSize: 12, color: muted, textAlign: "center", padding: "20px 0" }}>Sin observaciones registradas</div>
              )}
              {(selected.observaciones || []).map((n, i) => (
                <div key={i} style={{ background: paper2, border: `1px solid ${rule}`, borderLeft: `3px solid ${n.color}`, borderRadius: 3, padding: "10px 12px", marginBottom: 10 }}>
                  <div style={{ fontFamily: "JetBrains Mono", fontSize: 10.5, color: n.color, fontWeight: 700, marginBottom: 4 }}>{n.ref}</div>
                  <div style={{ fontSize: 12.5 }}>{n.txt}</div>
                  <div style={{ fontSize: 10.5, color: muted, marginTop: 6 }}>{n.quien} · {n.hora}</div>
                </div>
              ))}
              {(puedeObservar || puedeAprobarRechazar) && esEnviada && (
                <button onClick={() => setShowObservacion(true)} style={{ width: "100%", background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "8px 12px", fontSize: 12, color: ink, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", marginTop: 8 }}>
                  + Nueva observación
                </button>
              )}
            </div>
          </div>
        )}
        {obsTab === "Generadores" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 4px 4px" }}>
            <SectionLabel>Generadores de cantidad</SectionLabel>
            {(selected.conceptos || []).map(c => (
              <div key={c.clave} style={{ border: `1px solid ${rule}`, borderRadius: 3, padding: "12px 16px", marginBottom: 10 }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 6 }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra, fontWeight: 600 }}>{c.clave}</span>
                  <span style={{ fontSize: 12.5 }}>{c.desc}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, color: ink }}>{c.cantEjecutada} {c.unidad}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, fontWeight: 600 }}>{fmtMXN(c.importe)}</span>
                </div>
                <div style={{ background: "#FAF8F2", border: `1px dashed ${rule}`, borderRadius: 2, padding: "8px 12px", fontSize: 12, color: muted }}>
                  Generador adjunto — evidencia fotográfica y mediciones de campo.
                </div>
              </div>
            ))}
          </Card>
        )}
        {obsTab.startsWith("Notas vinculadas") && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 4px 4px" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
              <SectionLabel>Notas de bitácora vinculadas</SectionLabel>
              {esBorrador && puedeEnviar && (
                <SecondaryBtn onClick={() => setShowVincularBitacora(true)}>+ Vincular nota</SecondaryBtn>
              )}
            </div>
            {(selected.notasVinculadas || []).length === 0 && (
              <div style={{ color: muted, fontSize: 12, textAlign: "center", padding: 20 }}>Sin notas vinculadas</div>
            )}
            {(selected.notasVinculadas || []).map(folio => {
              const nota = mockBitacora.find(b => b.folio === folio);
              return nota ? (
                <div key={folio} style={{ display: "flex", gap: 16, padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, marginBottom: 8, alignItems: "center" }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra, fontWeight: 600, minWidth: 50 }}>{nota.folio}</span>
                  <span style={{ fontSize: 12.5 }}>{nota.asunto}</span>
                  <span style={{ fontSize: 11, color: muted, marginLeft: "auto" }}>{nota.fecha}</span>
                </div>
              ) : null;
            })}
          </Card>
        )}
        {obsTab === "Historial" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 4px 4px" }}>
            <SectionLabel>Historial de cambios</SectionLabel>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
              <TableHeader cols={["Estado anterior", "Estado nuevo", "Fecha y hora", "Usuario"]} />
              <tbody>
                {(selected.historial || []).map((h, i) => (
                  <tr key={i} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{h.estadoAnterior}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}><EstadoBadge estado={h.estadoNuevo} /></td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{h.fecha}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 12 }}>{h.usuario}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <p style={{ fontSize: 11, color: muted, marginTop: 10 }}>Solo lectura. Visible para todos los roles asignados al contrato.</p>
          </Card>
        )}

        {/* Modals */}
        {showAddConcepto && (
          <ModalAddConcepto
            onClose={() => setShowAddConcepto(false)}
            onAdd={(concepto) => {
              // api.addConcepto(selected.id, concepto)
              setSelected(prev => ({ ...prev, conceptos: [...(prev.conceptos || []), concepto] }));
              setShowAddConcepto(false);
            }}
          />
        )}
        {showVincularBitacora && (
          <ModalVincularBitacora
            notas={mockBitacora}
            vinculadas={selected.notasVinculadas || []}
            onClose={() => setShowVincularBitacora(false)}
            onVincular={(folios) => {
              // api.vincularBitacora(selected.id, folios)
              setSelected(prev => ({ ...prev, notasVinculadas: folios }));
              setShowVincularBitacora(false);
            }}
          />
        )}
        {showObservacion && (
          <Modal title="Nueva observación" onClose={() => setShowObservacion(false)} width={420}>
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Referencia (concepto o general)</SectionLabel>
              <TextInput placeholder="Ej: Concepto 05.004 o General" value="" onChange={() => {}} />
            </div>
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Observación</SectionLabel>
              <TextArea placeholder="Describe la observación..." value={nuevaObs} onChange={setNuevaObs} rows={4} />
            </div>
            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowObservacion(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <PrimaryBtn onClick={handleNuevaObs} style={{ flex: 1 }}>Registrar</PrimaryBtn>
            </div>
          </Modal>
        )}
        {showRechazo && (
          <Modal title="Rechazar estimación" onClose={() => setShowRechazo(false)} width={420}>
            <div style={{ background: folioSoft, border: `1px solid ${folio}`, borderRadius: 3, padding: "10px 14px", marginBottom: 16, fontSize: 12.5, color: folio }}>
              Se requiere un motivo antes de confirmar el rechazo.
            </div>
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Motivo de rechazo</SectionLabel>
              <TextArea placeholder="Describe el motivo del rechazo..." value={motivoRechazo} onChange={setMotivoRechazo} rows={4} />
            </div>
            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowRechazo(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <DangerBtn onClick={handleRechazar} style={{ flex: 1 }}>Confirmar rechazo</DangerBtn>
            </div>
          </Modal>
        )}
        {showPago && (
          <Modal
  title="Registrar pago"
  subtitle={"Estimación N.° " + String(selected.id).padStart(3, "0")}
  onClose={() => setShowPago(false)}
  width={440}
>
            <div style={{ display: "grid", gap: 14, marginBottom: 20 }}>
              <div><SectionLabel>Referencia de pago</SectionLabel><TextInput placeholder="Ej: CLC-2026-08741" value={datoPago.referencia} onChange={v => setDatoPago(p => ({ ...p, referencia: v }))} /></div>
              <div><SectionLabel>Banco / institución</SectionLabel><TextInput placeholder="Ej: Banorte" value={datoPago.banco} onChange={v => setDatoPago(p => ({ ...p, banco: v }))} /></div>
              <div><SectionLabel>Fecha de pago</SectionLabel><TextInput placeholder="DD-MM-YYYY" value={datoPago.fecha} onChange={v => setDatoPago(p => ({ ...p, fecha: v }))} /></div>
              <div><SectionLabel>Monto pagado</SectionLabel><TextInput placeholder="$0.00" value={datoPago.monto} onChange={v => setDatoPago(p => ({ ...p, monto: v }))} /></div>
            </div>
            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowPago(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <PrimaryBtn onClick={handlePago} style={{ flex: 1, background: pagado }} disabled={!datoPago.referencia || !datoPago.banco || !datoPago.fecha || !datoPago.monto}>Confirmar pago</PrimaryBtn>
            </div>
          </Modal>
        )}
      </div>
    );
  }

  // Bandeja
  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20, flexWrap: "wrap" }}>
        {puedeEnviar && (
          <PrimaryBtn onClick={() => setShowNuevaEst(true)}>+ Nueva estimación</PrimaryBtn>
        )}
        <div style={{ position: "relative" }}>
          <Search size={13} style={{ position: "absolute", left: 10, top: "50%", transform: "translateY(-50%)", color: muted }} />
          <input value={busqueda} onChange={e => setBusqueda(e.target.value)} placeholder="Buscar folio o periodo..." style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "7px 12px 7px 30px", fontSize: 12.5, color: ink, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", width: 220 }} />
        </div>
        {!puedePagar && (
          <select value={filtroEstado} onChange={e => setFiltroEstado(e.target.value)} style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "7px 12px", fontSize: 12, color: muted, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none" }}>
            {["Todos", "Borrador", "Enviada", "Observada", "Aprobada", "Pagada"].map(o => <option key={o}>{o}</option>)}
          </select>
        )}
        {puedePagar && <div style={{ fontSize: 11.5, color: muted }}>Mostrando solo estimaciones <strong>Aprobadas</strong> pendientes de pago.</div>}
      </div>
      <Card style={{ overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["N.°", "Periodo", "Monto", "Estado", "Días plazo", ""]} />
          <tbody>
            {visibles.map((e, i) => (
              <tr key={e.id} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }} onMouseEnter={el => (el.currentTarget.style.background = obraSoft)} onMouseLeave={el => (el.currentTarget.style.background = i % 2 === 1 ? "#FAF8F2" : paper2)}>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontWeight: 600, color: obra }}>{String(e.id).padStart(3, "0")}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{e.periodo}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono" }}>{fmtMXN(e.monto)}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}><EstadoBadge estado={e.estado} /></td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>
                  {e.dias ? <span style={{ color: observado, fontWeight: 600 }}>{e.dias} ⚑</span> : "—"}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <button onClick={() => setSelected(e)} style={{ fontSize: 11.5, color: obra, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}>Ver →</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>

      {showNuevaEst && (
        <ModalNuevaEstimacion
          onClose={() => setShowNuevaEst(false)}
          onCrear={(data) => {
            const nueva = { id: Math.max(...estimaciones.map(e => e.id)) + 1, periodo: data.periodo, monto: 0, estado: "Borrador", conceptos: [], notasVinculadas: [], observaciones: [], historial: [{ estadoAnterior: "—", estadoNuevo: "Borrador", fecha: new Date().toLocaleString("es-MX"), usuario: "Usuario actual" }] };
            // api.crearEstimacion(mockContrato.id, data)
            setEstimaciones(prev => [nueva, ...prev]);
            setShowNuevaEst(false);
            setSelected(nueva);
          }}
        />
      )}
    </div>
  );
}

function ModalNuevaEstimacion({ onClose, onCrear }) {
  const [periodo, setPeriodo] = useState("");
  return (
    <Modal title="Nueva estimación" subtitle="Se asignará número correlativo automático" onClose={onClose} width={420}>
      <div style={{ marginBottom: 16 }}>
        <SectionLabel>Periodo de la estimación</SectionLabel>
        <TextInput placeholder="Ej: Jul 2026" value={periodo} onChange={setPeriodo} />
      </div>
      <div style={{ background: obraSoft, border: `1px solid ${obra}`, borderRadius: 3, padding: "10px 14px", marginBottom: 20, fontSize: 12.5 }}>
        <strong>Datos de carátula precargados:</strong> Contrato {mockContrato.id} · {mockContrato.descripcion}
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onCrear({ periodo })} disabled={!periodo.trim()} style={{ flex: 1 }}>Crear borrador</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalAddConcepto({ onClose, onAdd }) {
  const [selected, setSelected] = useState(null);
  const [cant, setCant] = useState("");
  return (
    <Modal title="Añadir concepto del catálogo" onClose={onClose} width={520}>
      <div style={{ marginBottom: 16 }}>
        <SectionLabel>Selecciona un concepto</SectionLabel>
        {mockCatalogoConceptos.map(c => (
          <div key={c.clave} onClick={() => setSelected(c)} style={{ display: "flex", alignItems: "center", gap: 12, padding: "10px 14px", border: `1.5px solid ${selected?.clave === c.clave ? obra : rule}`, borderRadius: 3, marginBottom: 6, cursor: "pointer", background: selected?.clave === c.clave ? obraSoft : paper2 }}>
            <div style={{ width: 16, height: 16, borderRadius: "50%", border: `2px solid ${selected?.clave === c.clave ? obra : rule}`, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0 }}>
              {selected?.clave === c.clave && <div style={{ width: 8, height: 8, borderRadius: "50%", background: obra }} />}
            </div>
            <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra, minWidth: 60 }}>{c.clave}</span>
            <span style={{ fontSize: 12.5, flex: 1 }}>{c.desc}</span>
            <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>{c.unidad} · {fmtMXN(c.precioUnitario)}</span>
          </div>
        ))}
      </div>
      {selected && (
        <div style={{ marginBottom: 20 }}>
          <SectionLabel>Cantidad ejecutada en este periodo ({selected.unidad})</SectionLabel>
          <TextInput placeholder={`Cantidad en ${selected.unidad}`} value={cant} onChange={setCant} />
          {cant && <div style={{ fontSize: 12, color: muted, marginTop: 6 }}>Importe estimado: <strong>{fmtMXN(parseFloat(cant) * selected.precioUnitario)}</strong></div>}
        </div>
      )}
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => {
          if (!selected || !cant) return;
          onAdd({ ...selected, cantEjecutada: parseFloat(cant), importe: parseFloat(cant) * selected.precioUnitario });
        }} disabled={!selected || !cant} style={{ flex: 1 }}>Añadir concepto</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalVincularBitacora({ notas, vinculadas, onClose, onVincular }) {
  const [sel, setSel] = useState([...vinculadas]);
  const [busqueda, setBusqueda] = useState("");
  const [filtroTipo, setFiltroTipo] = useState("Todos");
  const filtradas = notas.filter(n => n.tipo !== "Incidencia" || n.tipo === "Incidencia").filter(n => filtroTipo === "Todos" || n.tipo === filtroTipo).filter(n => busqueda === "" || n.asunto.toLowerCase().includes(busqueda.toLowerCase()) || n.folio.includes(busqueda));
  return (
    <Modal title="Vincular notas de bitácora" onClose={onClose} width={520}>
      <div style={{ display: "flex", gap: 8, marginBottom: 14 }}>
        <TextInput placeholder="Buscar asunto o folio..." value={busqueda} onChange={setBusqueda} style={{ flex: 1 }} />
        <select value={filtroTipo} onChange={e => setFiltroTipo(e.target.value)} style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "8px 10px", fontSize: 12, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none" }}>
          {["Todos", "Nota", "Apertura", "Incidencia"].map(o => <option key={o}>{o}</option>)}
        </select>
      </div>
      <div style={{ maxHeight: 280, overflowY: "auto", marginBottom: 16 }}>
        {filtradas.map(n => {
          const isChecked = sel.includes(n.folio);
          return (
            <div key={n.id} onClick={() => setSel(prev => isChecked ? prev.filter(f => f !== n.folio) : [...prev, n.folio])} style={{ display: "flex", alignItems: "center", gap: 12, padding: "9px 12px", border: `1px solid ${isChecked ? obra : rule}`, borderRadius: 3, marginBottom: 6, cursor: "pointer", background: isChecked ? obraSoft : paper2 }}>
              <div style={{ width: 16, height: 16, borderRadius: 3, border: `2px solid ${isChecked ? obra : rule}`, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0, background: isChecked ? obra : "transparent" }}>
                {isChecked && <Check size={10} color="#fff" />}
              </div>
              <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: obra, minWidth: 50 }}>{n.folio}</span>
              <span style={{ fontSize: 12 }}>{n.asunto}</span>
              <span style={{ fontSize: 11, color: muted, marginLeft: "auto" }}>{n.fecha}</span>
            </div>
          );
        })}
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onVincular(sel)} style={{ flex: 1 }}>Vincular selección</PrimaryBtn>
      </div>
    </Modal>
  );
}

// ─── HU-07: Avance físico y Curva S ──────────────────────────────────────────
function TabAvance({ rol }) {
  const [showRegistro, setShowRegistro] = useState(false);
  const [avance, setAvance] = useState({ descripcion: "", porcentaje: "", foto: null });
  const puedeRegistrar = rol === "Residente" || rol === "Supervisor";
  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", gap: 24 }}>
      <div>
        <Card style={{ padding: "20px 22px", marginBottom: 20 }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
            <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 600, fontSize: 15 }}>Curva S — Avance físico vs. programado</div>
            {puedeRegistrar && <PrimaryBtn onClick={() => setShowRegistro(true)}>+ Registrar avance</PrimaryBtn>}
          </div>
          <ResponsiveContainer width="100%" height={260}>
            <LineChart data={mockCurvaS} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={rule} />
              <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} />
              <YAxis tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} unit="%" domain={[0, 100]} />
              <Tooltip contentStyle={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 3, fontSize: 12 }} formatter={(v) => v !== null ? [`${v}%`] : ["—"]} />
              <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
              <Line dataKey="programado" name="Programado" stroke={obra} strokeWidth={2.5} dot={{ fill: obra, r: 3 }} />
              <Line dataKey="real" name="Real" stroke={folio} strokeWidth={2.5} strokeDasharray="5 4" dot={{ fill: folio, r: 3 }} connectNulls={false} />
            </LineChart>
          </ResponsiveContainer>
        </Card>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 14 }}>
          {[
            { label: "Avance físico actual", value: "63%", note: "Programado: 68%", color: observado },
            { label: "Desviación del periodo", value: "-5 pp", note: "8% de atraso relativo", color: folio },
            { label: "Periodo activo", value: "Jun 2026", note: "Mes 4 de 7", color: obra },
          ].map(k => (
            <Card key={k.label} style={{ padding: "16px 18px" }}>
              <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 24, fontWeight: 700, color: k.color }}>{k.value}</div>
              <div style={{ fontSize: 10, textTransform: "uppercase", letterSpacing: "0.05em", color: muted, marginTop: 4 }}>{k.label}</div>
              <div style={{ fontSize: 11, color: muted, marginTop: 6 }}>{k.note}</div>
            </Card>
          ))}
        </div>
      </div>
      <AlertsPanel />

      {showRegistro && (
        <Modal title="Registrar avance físico" subtitle="HU-07 · Avance diario" onClose={() => setShowRegistro(false)} width={440}>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Descripción de actividades</SectionLabel>
            <TextArea placeholder="Describe las actividades ejecutadas hoy..." value={avance.descripcion} onChange={v => setAvance(p => ({ ...p, descripcion: v }))} rows={3} />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Porcentaje de avance acumulado (%)</SectionLabel>
            <TextInput placeholder="Ej: 65" value={avance.porcentaje} onChange={v => setAvance(p => ({ ...p, porcentaje: v }))} />
          </div>
          <div style={{ marginBottom: 20 }}>
            <SectionLabel>Fotografía de evidencia (requerida)</SectionLabel>
            <div style={{ border: `2px dashed ${avance.foto ? aprobado : rule}`, borderRadius: 4, padding: "20px", textAlign: "center", cursor: "pointer", background: avance.foto ? aprobadoSoft : "#FAF8F2" }} onClick={() => {
              const input = document.createElement("input"); input.type = "file"; input.accept = "image/*";
              input.onchange = e => setAvance(p => ({ ...p, foto: e.target.files[0]?.name || "foto.jpg" }));
              input.click();
            }}>
              {avance.foto ? (
                <div style={{ color: aprobado, fontSize: 13, fontWeight: 600 }}><CheckCircle size={20} style={{ marginBottom: 4 }} /><br />{avance.foto}</div>
              ) : (
                <div style={{ color: muted }}><Camera size={24} style={{ marginBottom: 8 }} /><br /><span style={{ fontSize: 12 }}>Toca para adjuntar fotografía</span></div>
              )}
            </div>
          </div>
          <div style={{ display: "flex", gap: 10 }}>
            <SecondaryBtn onClick={() => setShowRegistro(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
            <PrimaryBtn onClick={() => setShowRegistro(false)} disabled={!avance.descripcion || !avance.porcentaje || !avance.foto} style={{ flex: 1 }}>Guardar registro</PrimaryBtn>
          </div>
        </Modal>
      )}
    </div>
  );
}

// ─── HU-09 / HU-10 / HU-12 / HU-14: Bitácora electrónica ─────────────────────
function TabBitacora({ rol }) {
  const [notas, setNotas] = useState(mockBitacora);
  const [modalTipo, setModalTipo] = useState(null);
  const [busqueda, setBusqueda] = useState("");
  const [filtroTipo, setFiltroTipo] = useState("Todos");
  const [abierta, setAbierta] = useState(true);
  const [showApertura, setShowApertura] = useState(false);
  const [detalle, setDetalle] = useState(null);

  const puedeCrearNota = ["Residente", "Supervisor", "Superintendente"].includes(rol);
  const puedeCrearMinuta = rol === "Supervisor";
  const puedeCrearIncidencia = rol === "Supervisor";
  const puedeRegistrarSolicitudes = rol === "Superintendente";

  const filtradas = notas
    .filter(n => filtroTipo === "Todos" || n.tipo === filtroTipo)
    .filter(n => busqueda === "" || n.asunto.toLowerCase().includes(busqueda.toLowerCase()) || n.folio.includes(busqueda));

  function handleFirmar(nota, actor) {
    // api.firmarNota(nota.id, actor)
    setNotas(prev => prev.map(n => n.id === nota.id ? { ...n, firmas: { ...n.firmas, [actor.toLowerCase()]: true } } : n));
    setDetalle(prev => prev ? { ...prev, firmas: { ...prev.firmas, [actor.toLowerCase()]: true } } : null);
  }

  const actorFirma = rol === "Superintendente" ? "superintendente" : rol === "Residente" ? "residente" : rol === "Supervisor" ? "supervisor" : null;

  return (
    <div>
      {!abierta && (
        <div style={{ background: observadoSoft, border: `1px solid ${observado}`, borderRadius: 4, padding: "14px 18px", marginBottom: 20, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <div style={{ fontSize: 13, color: observado, fontWeight: 600 }}>La bitácora no ha sido abierta. No es posible crear notas.</div>
          {rol === "Residente" && <PrimaryBtn onClick={() => setShowApertura(true)}>Abrir bitácora →</PrimaryBtn>}
        </div>
      )}
      <div style={{ display: "flex", gap: 10, marginBottom: 20, flexWrap: "wrap" }}>
        {puedeCrearNota && abierta && <PrimaryBtn onClick={() => setModalTipo("nota")}>+ Nota</PrimaryBtn>}
        {puedeCrearMinuta && abierta && <SecondaryBtn onClick={() => setModalTipo("minuta")}>+ Minuta</SecondaryBtn>}
        {puedeCrearIncidencia && abierta && <SecondaryBtn onClick={() => setModalTipo("incidencia")}>+ Incidencia</SecondaryBtn>}
        {puedeRegistrarSolicitudes && abierta && <SecondaryBtn onClick={() => setModalTipo("solicitud")}>+ Solicitud/Aviso</SecondaryBtn>}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          <div style={{ position: "relative" }}>
            <Search size={13} style={{ position: "absolute", left: 10, top: "50%", transform: "translateY(-50%)", color: muted }} />
            <input value={busqueda} onChange={e => setBusqueda(e.target.value)} placeholder="Buscar asunto o folio..." style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "7px 12px 7px 28px", fontSize: 12, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", width: 200 }} />
          </div>
          <select value={filtroTipo} onChange={e => setFiltroTipo(e.target.value)} style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "7px 12px", fontSize: 12, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none" }}>
            {["Todos", "Apertura", "Nota", "Minuta", "Incidencia", "Solicitud"].map(o => <option key={o}>{o}</option>)}
          </select>
        </div>
      </div>

      <Card style={{ overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["Folio", "Tipo", "Fecha", "Asunto", "Firmas", "Folio ref.", ""]} />
          <tbody>
            {filtradas.map((b, i) => {
              const fCount = contarFirmas(b.firmas);
              const todas = fCount === "3/3";
              const ninguna = fCount === "0/3";
              return (
                <tr key={b.id} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }} onMouseEnter={el => (el.currentTarget.style.background = obraSoft)} onMouseLeave={el => (el.currentTarget.style.background = i % 2 === 1 ? "#FAF8F2" : paper2)}>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontWeight: 600, color: obra }}>{b.folio}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}><EstadoBadge estado={b.tipo} /></td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{b.fecha}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{b.asunto}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>
                    {b.tipo === "Incidencia" ? <span style={{ color: muted }}>—</span> : (
                      <span style={{ color: todas ? aprobado : ninguna ? folio : observado, fontWeight: 600 }}>{fCount} {!todas && "⏳"}</span>
                    )}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: muted }}>{b.folioRef || "—"}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    <button onClick={() => setDetalle(b)} style={{ fontSize: 11.5, color: obra, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}>Ver →</button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </Card>

      {detalle && (
        <Modal title={`Nota ${detalle.folio} — ${detalle.asunto}`} onClose={() => setDetalle(null)} width={500}>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Tipo</SectionLabel>
            <EstadoBadge estado={detalle.tipo} />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Fecha</SectionLabel>
            <div style={{ fontFamily: "JetBrains Mono", fontSize: 12.5 }}>{detalle.fecha}</div>
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Contenido</SectionLabel>
            <div style={{ background: "#FAF8F2", border: `1px solid ${rule}`, borderRadius: 3, padding: "12px 14px", fontSize: 13, lineHeight: 1.6 }}>{detalle.contenido}</div>
          </div>
          {detalle.folioRef && (
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Folio de referencia</SectionLabel>
              <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, color: obra }}>{detalle.folioRef}</span>
            </div>
          )}
          {detalle.tipo !== "Incidencia" && (
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Firmas</SectionLabel>
              <div style={{ display: "flex", gap: 8 }}>
                {[
                  { key: "superintendente", label: "Superintendente" },
                  { key: "residente", label: "Residente" },
                  { key: "supervisor", label: "Supervisión" },
                ].map(s => {
                  const done = detalle.firmas[s.key];
                  const esMiTurno = actorFirma === s.key && !done;
                  return (
                    <div key={s.key} onClick={() => esMiTurno && handleFirmar(detalle, s.key)} style={{ flex: 1, border: `1.5px solid ${done ? aprobado : esMiTurno ? obra : rule}`, background: done ? aprobadoSoft : esMiTurno ? obraSoft : "#FAF8F2", borderRadius: 3, padding: "10px 6px", textAlign: "center", fontSize: 11, color: done ? aprobado : esMiTurno ? obra : muted, fontWeight: done ? 700 : 400, cursor: esMiTurno ? "pointer" : "default" }}>
                      {done ? `✓ ${s.label}` : esMiTurno ? `✍ Firmar` : s.label}
                    </div>
                  );
                })}
              </div>
            </div>
          )}
          {detalle.tipo === "Incidencia" && (
            <PrimaryBtn onClick={() => { /* generar nota desde incidencia */ }} style={{ width: "100%", marginBottom: 10 }}>
              Generar nota de bitácora desde esta incidencia
            </PrimaryBtn>
          )}
        </Modal>
      )}

      {showApertura && (
        <ModalAperturaBitacora onClose={() => setShowApertura(false)} onAbrir={(data) => {
          // api.abrirBitacora(mockContrato.id, data)
          setAbierta(true);
          setShowApertura(false);
        }} />
      )}

      {modalTipo === "nota" && (
        <ModalNota
          tipo="Nota"
          onClose={() => setModalTipo(null)}
          siguienteFolio={`${String(notas.filter(n => n.folio.match(/^\d+$/)).length + 1).padStart(4, "0")}`}
          onGuardar={(data) => {
            const nueva = { id: Date.now(), folio: data.folio, tipo: "Nota", fecha: new Date().toLocaleDateString("es-MX"), asunto: data.asunto, contenido: data.contenido, firmas: { superintendente: rol === "Superintendente", residente: false, supervisor: false }, folioRef: data.folioRef || null };
            // api.crearNota(mockContrato.id, data)
            setNotas(prev => [...prev, nueva]);
            setModalTipo(null);
          }}
        />
      )}

      {modalTipo === "minuta" && (
        <ModalMinuta onClose={() => setModalTipo(null)} onGuardar={(data) => {
          // api.crearMinuta(mockContrato.id, data)
          setNotas(prev => [...prev, { id: Date.now(), folio: `MIN-${Date.now()}`, tipo: "Minuta", fecha: data.fecha, asunto: data.acuerdos.substring(0, 40) + "...", contenido: data.acuerdos, firmas: { superintendente: false, residente: false, supervisor: true }, folioRef: null }]);
          setModalTipo(null);
        }} />
      )}

      {modalTipo === "incidencia" && (
        <ModalIncidencia onClose={() => setModalTipo(null)} onGuardar={(data) => {
          // api.crearIncidencia(mockContrato.id, data)
          setNotas(prev => [...prev, { id: Date.now(), folio: `INC-${String(notas.filter(n => n.folio.startsWith("INC")).length + 1).padStart(2, "0")}`, tipo: "Incidencia", fecha: data.fecha, asunto: data.descripcion.substring(0, 50), contenido: data.descripcion, firmas: { superintendente: false, residente: false, supervisor: false }, folioRef: null, foto: true }]);
          setModalTipo(null);
        }} />
      )}

      {modalTipo === "solicitud" && (
        <ModalSolicitud onClose={() => setModalTipo(null)} onGuardar={(data) => {
          setNotas(prev => [...prev, { id: Date.now(), folio: String(notas.length + 1).padStart(4, "0"), tipo: "Solicitud", fecha: new Date().toLocaleDateString("es-MX"), asunto: `${data.tipoSolicitud}: ${data.asunto}`, contenido: data.descripcion, firmas: { superintendente: true, residente: false, supervisor: false }, folioRef: null }]);
          setModalTipo(null);
        }} />
      )}
    </div>
  );
}

function ModalAperturaBitacora({ onClose, onAbrir }) {
  const [form, setForm] = useState({ lugar: "", fecha: "", descripcion: "" });
  const campos = [
    { key: "lugar", label: "Lugar de obra", placeholder: "Ej: Av. Reforma Norte, Km 4+200" },
    { key: "fecha", label: "Fecha de apertura", placeholder: "DD-MM-YYYY" },
    { key: "descripcion", label: "Descripción del contrato", placeholder: "Descripción del objeto del contrato" },
  ];
  const completo = campos.every(c => form[c.key].trim());
  return (
    <Modal title="Apertura de bitácora" subtitle="Se asignará folio único automático" onClose={onClose} width={460}>
      {campos.map(c => (
        <div key={c.key} style={{ marginBottom: 14 }}>
          <SectionLabel>{c.label} *</SectionLabel>
          <TextInput placeholder={c.placeholder} value={form[c.key]} onChange={v => setForm(p => ({ ...p, [c.key]: v }))} />
        </div>
      ))}
      <div style={{ background: obraSoft, border: `1px solid ${obra}`, borderRadius: 3, padding: "10px 14px", fontSize: 12.5, marginBottom: 20 }}>
        Se generará automáticamente la <strong>Nota de Apertura (Folio 0001)</strong> con los datos capturados.
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onAbrir(form)} disabled={!completo} style={{ flex: 1 }}>Abrir bitácora</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalNota({ tipo, onClose, siguienteFolio, onGuardar }) {
  const [form, setForm] = useState({ folio: siguienteFolio, asunto: "", contenido: "", tipoNota: "Aviso al residente", folioRef: "" });
  const tipos = ["Aviso al residente", "Solicitud de convenio", "Cambio de procedimiento", "Concepto terminado", "Instrucción"];
  const completo = form.asunto.trim() && form.contenido.trim();
  return (
    <Modal title={`Nueva ${tipo}`} subtitle={`FOLIO-${form.folio} (autoasignado)`} onClose={onClose} width={480}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo</SectionLabel>
        <div style={{ display: "flex", gap: 6, flexWrap: "wrap" }}>
          {tipos.map(t => (
            <span key={t} onClick={() => setForm(p => ({ ...p, tipoNota: t }))} style={{ fontSize: 11.5, border: `1px solid ${form.tipoNota === t ? obra : rule}`, color: form.tipoNota === t ? "#fff" : ink, background: form.tipoNota === t ? obra : "transparent", padding: "5px 10px", borderRadius: 13, cursor: "pointer" }}>{t}</span>
          ))}
        </div>
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Asunto *</SectionLabel>
        <TextInput placeholder="Asunto de la nota" value={form.asunto} onChange={v => setForm(p => ({ ...p, asunto: v }))} />
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Contenido *</SectionLabel>
        <TextArea placeholder="Describe el contenido de la nota..." value={form.contenido} onChange={v => setForm(p => ({ ...p, contenido: v }))} rows={4} />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Folio de referencia (opcional)</SectionLabel>
        <TextInput placeholder="Ej: 0138" value={form.folioRef} onChange={v => setForm(p => ({ ...p, folioRef: v }))} />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!completo} style={{ flex: 1, background: folio }}>Firmar y registrar nota</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalMinuta({ onClose, onGuardar }) {
  const [form, setForm] = useState({ fecha: "", participantes: "", acuerdos: "" });
  const completo = Object.values(form).every(v => v.trim());
  return (
    <Modal title="Nueva minuta de reunión" onClose={onClose} width={460}>
      <div style={{ marginBottom: 14 }}><SectionLabel>Fecha de la reunión *</SectionLabel><TextInput placeholder="DD-MM-YYYY" value={form.fecha} onChange={v => setForm(p => ({ ...p, fecha: v }))} /></div>
      <div style={{ marginBottom: 14 }}><SectionLabel>Participantes *</SectionLabel><TextInput placeholder="Ej: Residente, Supervisor, Superintendente" value={form.participantes} onChange={v => setForm(p => ({ ...p, participantes: v }))} /></div>
      <div style={{ marginBottom: 20 }}><SectionLabel>Acuerdos y contenido *</SectionLabel><TextArea placeholder="Describe los acuerdos de la reunión..." value={form.acuerdos} onChange={v => setForm(p => ({ ...p, acuerdos: v }))} rows={4} /></div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!completo} style={{ flex: 1 }}>Registrar minuta</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalIncidencia({ onClose, onGuardar }) {
  const [form, setForm] = useState({ fecha: "", descripcion: "", foto: null });
  const completo = form.fecha.trim() && form.descripcion.trim() && form.foto;
  return (
    <Modal title="Registrar incidencia" onClose={onClose} width={440}>
      <div style={{ marginBottom: 14 }}><SectionLabel>Fecha *</SectionLabel><TextInput placeholder="DD-MM-YYYY" value={form.fecha} onChange={v => setForm(p => ({ ...p, fecha: v }))} /></div>
      <div style={{ marginBottom: 14 }}><SectionLabel>Descripción *</SectionLabel><TextArea placeholder="Describe la incidencia..." value={form.descripcion} onChange={v => setForm(p => ({ ...p, descripcion: v }))} rows={3} /></div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Fotografía (requerida) *</SectionLabel>
        <div style={{ border: `2px dashed ${form.foto ? aprobado : rule}`, borderRadius: 4, padding: 20, textAlign: "center", cursor: "pointer", background: form.foto ? aprobadoSoft : "#FAF8F2" }} onClick={() => {
          const input = document.createElement("input"); input.type = "file"; input.accept = "image/*";
          input.onchange = e => setForm(p => ({ ...p, foto: e.target.files[0]?.name || "foto.jpg" }));
          input.click();
        }}>
          {form.foto ? <div style={{ color: aprobado, fontWeight: 600 }}><CheckCircle size={18} /><br />{form.foto}</div> : <div style={{ color: muted }}><Camera size={22} /><br /><span style={{ fontSize: 12 }}>Adjuntar foto</span></div>}
        </div>
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!completo} style={{ flex: 1 }}>Registrar incidencia</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalSolicitud({ onClose, onGuardar }) {
  const [form, setForm] = useState({ tipoSolicitud: "Solicitud de convenio", asunto: "", descripcion: "" });
  const tipos = ["Solicitud de modificación", "Cambio de procedimiento", "Solicitud de convenio", "Aviso de terminación"];
  return (
    <Modal title="Solicitud / Aviso en bitácora" subtitle="HU-14 · Superintendente" onClose={onClose} width={460}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo *</SectionLabel>
        {tipos.map(t => (
          <div key={t} onClick={() => setForm(p => ({ ...p, tipoSolicitud: t }))} style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 12px", border: `1.5px solid ${form.tipoSolicitud === t ? obra : rule}`, borderRadius: 3, marginBottom: 6, cursor: "pointer", background: form.tipoSolicitud === t ? obraSoft : paper2 }}>
            <div style={{ width: 14, height: 14, borderRadius: "50%", border: `2px solid ${form.tipoSolicitud === t ? obra : rule}`, display: "flex", alignItems: "center", justifyContent: "center" }}>
              {form.tipoSolicitud === t && <div style={{ width: 7, height: 7, borderRadius: "50%", background: obra }} />}
            </div>
            <span style={{ fontSize: 12.5 }}>{t}</span>
          </div>
        ))}
      </div>
      <div style={{ marginBottom: 14 }}><SectionLabel>Asunto *</SectionLabel><TextInput placeholder="Asunto de la nota" value={form.asunto} onChange={v => setForm(p => ({ ...p, asunto: v }))} /></div>
      <div style={{ marginBottom: 20 }}><SectionLabel>Descripción *</SectionLabel><TextArea placeholder="Describe el contenido..." value={form.descripcion} onChange={v => setForm(p => ({ ...p, descripcion: v }))} rows={3} /></div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!form.asunto || !form.descripcion} style={{ flex: 1 }}>Registrar</PrimaryBtn>
      </div>
    </Modal>
  );
}

// ─── HU-15 / HU-16 / HU-17 / HU-18: Convenios ────────────────────────────────
function TabConvenios({ rol }) {
  const [convenios, setConvenios] = useState(mockConvenios);
  const [showNuevo, setShowNuevo] = useState(false);
  const [detalle, setDetalle] = useState(null);
  const [showDictamen, setShowDictamen] = useState(false);
  const [showResolucion, setShowResolucion] = useState(false);

  const puedeCrear = rol === "Superintendente";
  const puedeDictaminar = rol === "Supervisor";
  const puedePromover = rol === "Residente";
  const puedeResolver = rol === "Dependencia";

  const pasoActivo = (conv) => {
    if (!conv) return 0;
    const map = { "En evaluación": 0, "Dictaminada": 1, "Promovida": 2, "Aprobado": 3, "Rechazado": 3 };
    return map[conv.estado] ?? 0;
  };

  function handlePromover(id) {
    // api.promoverConvenio(id)
    setConvenios(prev => prev.map(c => c.id === id ? { ...c, estado: "Promovida" } : c));
    setDetalle(prev => prev ? { ...prev, estado: "Promovida" } : null);
  }

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20 }}>
        {puedeCrear && <PrimaryBtn onClick={() => setShowNuevo(true)}>+ Nueva solicitud</PrimaryBtn>}
        <div style={{ marginLeft: "auto", fontFamily: "JetBrains Mono", fontSize: 11.5, color: muted }}>
          Variación acumulada: <strong style={{ color: observado }}>18%</strong> / límite legal 25%
        </div>
      </div>
      <Card style={{ overflow: "hidden", marginBottom: 24 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["ID", "Tipo", "Monto", "Estado", "Dictamen", ""]} />
          <tbody>
            {convenios.map((c, i) => (
              <tr key={c.id} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }} onMouseEnter={el => (el.currentTarget.style.background = obraSoft)} onMouseLeave={el => (el.currentTarget.style.background = i % 2 === 1 ? "#FAF8F2" : paper2)}>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontWeight: 600, color: obra }}>{c.id}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{c.tipo}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.monto ? fmtMXN(c.monto) : "—"}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}><EstadoBadge estado={c.estado} /></td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 12 }}>{c.dictamen || "—"}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <button onClick={() => setDetalle(c)} style={{ fontSize: 11.5, color: obra, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}>Ver →</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>

      {/* Flujo de aprobación del primer convenio en evaluación */}
      {convenios.filter(c => ["En evaluación", "Dictaminada", "Promovida"].includes(c.estado)).map(c => (
        <div key={c.id} style={{ marginBottom: 24 }}>
          <div style={{ fontSize: 12, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.06em", color: muted, marginBottom: 14 }}>
            Flujo de aprobación — {c.id}
          </div>
          <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
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
                <div key={s.n} style={{ border: `1.5px solid ${done ? aprobado : active ? obra : rule}`, background: done ? aprobadoSoft : active ? obraSoft : paper2, borderRadius: 4, padding: "14px 16px", color: done ? aprobado : active ? obra : muted }}>
                  <div style={{ fontFamily: "JetBrains Mono", fontSize: 22, fontWeight: 700, opacity: 0.2 }}>{s.n}</div>
                  <div style={{ fontSize: 12.5, fontWeight: active ? 600 : 400, marginTop: 4 }}>{s.label}</div>
                  {done && <div style={{ fontSize: 10.5, marginTop: 6 }}>✓ Completado</div>}
                  {active && <div style={{ fontSize: 10.5, marginTop: 6 }}>→ En proceso</div>}
                </div>
              );
            })}
          </div>
        </div>
      ))}

      {/* Detalle */}
      {detalle && (
        <Modal title={`Convenio ${detalle.id}`} subtitle={detalle.tipo} onClose={() => setDetalle(null)} width={520}>
          <div style={{ marginBottom: 14 }}><SectionLabel>Tipo de modificación</SectionLabel><div style={{ fontSize: 13 }}>{detalle.tipo}</div></div>
          <div style={{ marginBottom: 14 }}><SectionLabel>Monto de variación</SectionLabel><div style={{ fontFamily: "JetBrains Mono", fontSize: 13 }}>{detalle.monto ? fmtMXN(detalle.monto) : "Sin impacto económico"}</div></div>
          <div style={{ marginBottom: 14 }}><SectionLabel>Justificación</SectionLabel><div style={{ background: "#FAF8F2", border: `1px solid ${rule}`, borderRadius: 3, padding: "10px 14px", fontSize: 12.5, lineHeight: 1.6 }}>{detalle.justificacion}</div></div>
          <div style={{ marginBottom: 14 }}><SectionLabel>Estado</SectionLabel><EstadoBadge estado={detalle.estado} /></div>
          {detalle.dictamen && (
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Dictamen de supervisión</SectionLabel>
              <div style={{ display: "flex", alignItems: "center", gap: 8, padding: "10px 14px", background: aprobadoSoft, borderRadius: 3, border: `1px solid ${aprobado}` }}>
                <CheckCircle size={14} color={aprobado} />
                <span style={{ fontSize: 12.5, color: aprobado, fontWeight: 600 }}>{detalle.dictamen}</span>
                {detalle.dictamenJust && <span style={{ fontSize: 12, color: muted }}>— {detalle.dictamenJust}</span>}
              </div>
            </div>
          )}
          <div style={{ display: "flex", gap: 10, flexWrap: "wrap", marginTop: 6 }}>
            {puedeDictaminar && detalle.estado === "En evaluación" && (
              <PrimaryBtn onClick={() => setShowDictamen(true)}>Emitir dictamen</PrimaryBtn>
            )}
            {puedePromover && detalle.estado === "Dictaminada" && (
              <PrimaryBtn onClick={() => handlePromover(detalle.id)}>Promover ante Dependencia</PrimaryBtn>
            )}
            {puedeResolver && detalle.estado === "Promovida" && (
              <PrimaryBtn onClick={() => setShowResolucion(true)} style={{ background: aprobado }}>Emitir resolución</PrimaryBtn>
            )}
          </div>
        </Modal>
      )}

      {showDictamen && (
        <ModalDictamen
          onClose={() => setShowDictamen(false)}
          onGuardar={(data) => {
            // api.dictaminarConvenio(detalle.id, data)
            setConvenios(prev => prev.map(c => c.id === detalle.id ? { ...c, estado: "Dictaminada", dictamen: data.procedencia, dictamenJust: data.justificacion } : c));
            setDetalle(prev => prev ? { ...prev, estado: "Dictaminada", dictamen: data.procedencia, dictamenJust: data.justificacion } : null);
            setShowDictamen(false);
          }}
        />
      )}

      {showResolucion && (
        <ModalResolucion
          onClose={() => setShowResolucion(false)}
          onGuardar={(data) => {
            // api.resolverConvenio(detalle.id, data)
            setConvenios(prev => prev.map(c => c.id === detalle.id ? { ...c, estado: data.resolucion === "Aprobado" ? "Aprobado" : "Rechazado" } : c));
            setDetalle(null);
            setShowResolucion(false);
          }}
        />
      )}

      {showNuevo && <ModalNuevoConvenio onClose={() => setShowNuevo(false)} onCrear={(data) => {
        // api.crearConvenio(mockContrato.id, data)
        setConvenios(prev => [{ id: `CM-${String(prev.length + 1).padStart(3, "0")}`, ...data, estado: "En evaluación", dictamen: null, dictamenJust: "", pasos: [] }, ...prev]);
        setShowNuevo(false);
      }} />}
    </div>
  );
}

function ModalNuevoConvenio({ onClose, onCrear }) {
  const [form, setForm] = useState({ tipo: "Ampliación de plazo", monto: "", justificacion: "", doc: null });
  const tipos = ["Incremento de monto", "Ampliación de plazo", "Ajuste de catálogo", "Reducción de monto"];
  const completo = form.justificacion.trim() && form.doc;

  // Variación acumulada simulada
  const variacionActual = 18;
  const variacionNueva = form.monto ? variacionActual + (parseFloat(form.monto) / mockContrato.monto * 100) : variacionActual;

  return (
    <Modal title="Nueva solicitud de convenio" subtitle="HU-15 · Superintendente" onClose={onClose} width={500}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo de modificación *</SectionLabel>
        {tipos.map(t => (
          <div key={t} onClick={() => setForm(p => ({ ...p, tipo: t }))} style={{ display: "flex", alignItems: "center", gap: 10, padding: "8px 12px", border: `1.5px solid ${form.tipo === t ? obra : rule}`, borderRadius: 3, marginBottom: 6, cursor: "pointer", background: form.tipo === t ? obraSoft : paper2 }}>
            <div style={{ width: 14, height: 14, borderRadius: "50%", border: `2px solid ${form.tipo === t ? obra : rule}`, display: "flex", alignItems: "center", justifyContent: "center" }}>
              {form.tipo === t && <div style={{ width: 7, height: 7, borderRadius: "50%", background: obra }} />}
            </div>
            <span style={{ fontSize: 12.5 }}>{t}</span>
          </div>
        ))}
      </div>
      {(form.tipo === "Incremento de monto" || form.tipo === "Reducción de monto") && (
        <div style={{ marginBottom: 14 }}>
          <SectionLabel>Monto de variación</SectionLabel>
          <TextInput placeholder="Ej: 500000" value={form.monto} onChange={v => setForm(p => ({ ...p, monto: v }))} />
          <div style={{ display: "flex", justifyContent: "space-between", marginTop: 8, padding: "8px 12px", background: variacionNueva > 25 ? folioSoft : observadoSoft, borderRadius: 3 }}>
            <span style={{ fontSize: 12, color: muted }}>Variación acumulada proyectada</span>
            <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, fontWeight: 700, color: variacionNueva > 25 ? folio : observado }}>{variacionNueva.toFixed(1)}% / 25% límite</span>
          </div>
        </div>
      )}
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Justificación *</SectionLabel>
        <TextArea placeholder="Justifica la necesidad del convenio..." value={form.justificacion} onChange={v => setForm(p => ({ ...p, justificacion: v }))} rows={4} />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Documentación soporte (requerida) *</SectionLabel>
        <div style={{ border: `2px dashed ${form.doc ? aprobado : rule}`, borderRadius: 4, padding: 16, textAlign: "center", cursor: "pointer", background: form.doc ? aprobadoSoft : "#FAF8F2" }} onClick={() => {
          const input = document.createElement("input"); input.type = "file"; input.multiple = true;
          input.onchange = e => setForm(p => ({ ...p, doc: e.target.files[0]?.name || "doc.pdf" }));
          input.click();
        }}>
          {form.doc ? <div style={{ color: aprobado, fontWeight: 600 }}><CheckCircle size={16} /> {form.doc}</div> : <div style={{ color: muted }}><Upload size={20} /><br /><span style={{ fontSize: 12 }}>Adjuntar documentación soporte</span></div>}
        </div>
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onCrear(form)} disabled={!completo} style={{ flex: 1 }}>Enviar solicitud</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalDictamen({ onClose, onGuardar }) {
  const [form, setForm] = useState({ procedencia: "Procedente", justificacion: "" });
  return (
    <Modal title="Emitir dictamen técnico" subtitle="HU-16 · Supervisor externo" onClose={onClose} width={440}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Procedencia *</SectionLabel>
        <div style={{ display: "flex", gap: 10 }}>
          {["Procedente", "No procedente"].map(p => (
            <div key={p} onClick={() => setForm(prev => ({ ...prev, procedencia: p }))} style={{ flex: 1, padding: "12px", border: `2px solid ${form.procedencia === p ? (p === "Procedente" ? aprobado : folio) : rule}`, borderRadius: 4, cursor: "pointer", textAlign: "center", background: form.procedencia === p ? (p === "Procedente" ? aprobadoSoft : folioSoft) : paper2, color: form.procedencia === p ? (p === "Procedente" ? aprobado : folio) : ink, fontWeight: form.procedencia === p ? 700 : 400, fontSize: 13 }}>
              {p}
            </div>
          ))}
        </div>
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Justificación escrita *</SectionLabel>
        <TextArea placeholder="Fundamenta tu dictamen..." value={form.justificacion} onChange={v => setForm(p => ({ ...p, justificacion: v }))} rows={4} />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!form.justificacion.trim()} style={{ flex: 1, background: form.procedencia === "Procedente" ? aprobado : folio }}>Confirmar dictamen</PrimaryBtn>
      </div>
    </Modal>
  );
}

function ModalResolucion({ onClose, onGuardar }) {
  const [form, setForm] = useState({ resolucion: "Aprobado", motivo: "" });
  const requiereMotivo = form.resolucion === "Rechazado";
  return (
    <Modal title="Resolución institucional" subtitle="HU-18 · Dependencia" onClose={onClose} width={440}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Resolución *</SectionLabel>
        <div style={{ display: "flex", gap: 10 }}>
          {["Aprobado", "Rechazado"].map(r => (
            <div key={r} onClick={() => setForm(prev => ({ ...prev, resolucion: r }))} style={{ flex: 1, padding: "12px", border: `2px solid ${form.resolucion === r ? (r === "Aprobado" ? aprobado : folio) : rule}`, borderRadius: 4, cursor: "pointer", textAlign: "center", background: form.resolucion === r ? (r === "Aprobado" ? aprobadoSoft : folioSoft) : paper2, color: form.resolucion === r ? (r === "Aprobado" ? aprobado : folio) : ink, fontWeight: form.resolucion === r ? 700 : 400, fontSize: 13 }}>
              {r}
            </div>
          ))}
        </div>
      </div>
      {requiereMotivo && (
        <div style={{ marginBottom: 14 }}>
          <SectionLabel>Motivo de rechazo *</SectionLabel>
          <TextArea placeholder="Indica el motivo de la resolución de rechazo..." value={form.motivo} onChange={v => setForm(p => ({ ...p, motivo: v }))} rows={3} />
        </div>
      )}
      {form.resolucion === "Aprobado" && (
        <div style={{ background: aprobadoSoft, border: `1px solid ${aprobado}`, borderRadius: 3, padding: "10px 14px", marginBottom: 16, fontSize: 12.5, color: aprobado }}>
          Al aprobar, el sistema actualizará automáticamente el catálogo de conceptos, el programa de obra y el monto contratado.
        </div>
      )}
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={requiereMotivo && !form.motivo.trim()} style={{ flex: 1, background: form.resolucion === "Aprobado" ? aprobado : folio }}>Confirmar resolución</PrimaryBtn>
      </div>
    </Modal>
  );
}

// ─── HU-19 / HU-20: Entrega-Recepción y Finiquito ────────────────────────────
function TabFiniquito({ rol }) {
  const [showCierre, setShowCierre] = useState(false);
  const [cierreRegistrado, setCierreRegistrado] = useState(false);
  const [finiquitoCalc, setFiniquitoCalc] = useState(false);
  const puedeRegistrarCierre = rol === "Residente";
  const puedeVerFiniquito = rol === "Financiero" || rol === "Dependencia" || rol === "Residente";

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
          <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 600, fontSize: 15, marginBottom: 16 }}>Entrega-Recepción de obra</div>
          {cierreRegistrado ? (
            <div style={{ background: aprobadoSoft, border: `1px solid ${aprobado}`, borderRadius: 3, padding: "14px 18px", display: "flex", alignItems: "center", gap: 12 }}>
              <CheckCircle size={22} color={aprobado} />
              <div>
                <div style={{ fontSize: 13, fontWeight: 600, color: aprobado }}>Cierre físico registrado</div>
                <div style={{ fontSize: 12, color: muted, marginTop: 2 }}>Fecha de entrega: 25-jun-2026 · Documentación adjunta</div>
              </div>
            </div>
          ) : (
            <div style={{ textAlign: "center", padding: "20px 0" }}>
              <div style={{ color: muted, fontSize: 12.5, marginBottom: 16 }}>No se ha registrado el cierre físico de obra.</div>
              {puedeRegistrarCierre && <PrimaryBtn onClick={() => setShowCierre(true)}>Registrar entrega-recepción</PrimaryBtn>}
            </div>
          )}
        </Card>

        {/* Finiquito */}
        <Card style={{ padding: "20px 22px" }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
            <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 600, fontSize: 15 }}>Finiquito del contrato</div>
            {puedeVerFiniquito && !finiquitoCalc && (
              <PrimaryBtn onClick={() => { /* api.calcularFiniquito */ setFiniquitoCalc(true); }}>
                Calcular finiquito
              </PrimaryBtn>
            )}
          </div>
          {finiquitoCalc ? (
            <div>
              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14, marginBottom: 16 }}>
                {[
                  { l: "Total estimaciones pagadas", v: datos.pagado, color: aprobado },
                  { l: "Estimaciones pendientes", v: datos.pendiente, color: observado },
                  { l: "Deductivas aplicadas", v: -datos.deductivas, color: folio },
                  { l: "Retenciones", v: -datos.retenciones, color: pagado },
                ].map(k => (
                  <div key={k.l} style={{ border: `1px solid ${rule}`, borderRadius: 3, padding: "12px 14px" }}>
                    <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 18, fontWeight: 700, color: k.color }}>{fmtMXN(k.v)}</div>
                    <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.04em", color: muted, marginTop: 2 }}>{k.l}</div>
                  </div>
                ))}
              </div>
              <div style={{ background: ink, color: paper, borderRadius: 4, padding: "16px 20px", display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <span style={{ fontSize: 12, textTransform: "uppercase", letterSpacing: "0.08em" }}>Monto final del contrato</span>
                <span style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 24, fontWeight: 700 }}>{fmtMXN(datos.total)}</span>
              </div>
              <button style={{ marginTop: 12, width: "100%", background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "9px", fontSize: 12, color: muted, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", display: "flex", alignItems: "center", justifyContent: "center", gap: 6 }}>
                <Download size={13} /> Descargar reporte de finiquito
              </button>
            </div>
          ) : (
            <div style={{ color: muted, fontSize: 12.5, textAlign: "center", padding: "20px 0" }}>
              {puedeVerFiniquito ? "Presiona 'Calcular finiquito' para consolidar la información del contrato." : "Solo el Área Financiera puede calcular el finiquito."}
            </div>
          )}
        </Card>
      </div>
      <AlertsPanel />

      {showCierre && (
        <Modal title="Registrar entrega-recepción" subtitle="HU-19 · Residente de obra" onClose={() => setShowCierre(false)} width={460}>
          {[
            { key: "fecha", label: "Fecha de entrega *", placeholder: "DD-MM-YYYY" },
            { key: "acta", label: "Número de acta *", placeholder: "Ej: ACTA-2026-001" },
          ].map(f => (
            <div key={f.key} style={{ marginBottom: 14 }}><SectionLabel>{f.label}</SectionLabel><TextInput placeholder={f.placeholder} value="" onChange={() => {}} /></div>
          ))}
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Estado de garantías al momento de entrega</SectionLabel>
            {mockContrato.garantias.map(g => (
              <div key={g.tipo} style={{ display: "flex", justifyContent: "space-between", padding: "8px 12px", border: `1px solid ${rule}`, borderRadius: 3, marginBottom: 6, fontSize: 12 }}>
                <span>{g.tipo}</span>
                <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{g.vencimiento}</span>
                <EstadoBadge estado={g.diasRestantes < 30 ? "por vencer" : "vigente"} />
              </div>
            ))}
          </div>
          <div style={{ marginBottom: 20 }}>
            <SectionLabel>Documentación / fotografía (requerida) *</SectionLabel>
            <div style={{ border: `2px dashed ${rule}`, borderRadius: 4, padding: 16, textAlign: "center", cursor: "pointer", background: "#FAF8F2" }} onClick={() => {}}>
              <Upload size={20} style={{ color: muted }} /><br /><span style={{ fontSize: 12, color: muted }}>Adjuntar acta y fotos</span>
            </div>
          </div>
          <div style={{ display: "flex", gap: 10 }}>
            <SecondaryBtn onClick={() => setShowCierre(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
            <PrimaryBtn onClick={() => { setCierreRegistrado(true); setShowCierre(false); }} style={{ flex: 1 }}>Registrar cierre</PrimaryBtn>
          </div>
        </Modal>
      )}
    </div>
  );
}

// ─── HU-22 / HU-23 / HU-24: Expediente digital ───────────────────────────────
function TabExpediente({ rol }) {
  const secciones = ["Datos generales", "Catálogo", "Programa de obra", "Garantías", "Estimaciones", "Bitácora", "Convenios", "Documentación"];
  const [sec, setSec] = useState("Datos generales");

  return (
    <div style={{ display: "grid", gridTemplateColumns: "200px 1fr", gap: 20 }}>
      <Card style={{ overflow: "hidden", height: "fit-content" }}>
        {secciones.map(s => (
          <button key={s} onClick={() => setSec(s)} style={{ display: "block", width: "100%", textAlign: "left", padding: "10px 14px", fontSize: 12.5, color: sec === s ? obra : ink, fontWeight: sec === s ? 600 : 400, background: sec === s ? obraSoft : "transparent", border: "none", borderBottom: `1px solid ${rule}`, borderLeft: `3px solid ${sec === s ? obra : "transparent"}`, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}>
            {s}
          </button>
        ))}
      </Card>
      <Card style={{ padding: "22px 24px" }}>
        <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 18, marginBottom: 20, color: ink }}>{sec}</div>

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
            ].map(([l, v]) => <Field key={l} label={l} value={v} />)}
          </div>
        )}

        {sec === "Catálogo" && (
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
            <TableHeader cols={["Clave", "Descripción", "Unidad", "P.U.", ""]} />
            <tbody>
              {mockCatalogoConceptos.map((c, i) => (
                <tr key={c.clave} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{c.clave}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{c.desc}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{c.unidad}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(c.precioUnitario)}</td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    <button style={{ fontSize: 11.5, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "3px 8px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4 }}><Download size={11} /> Descargar</button>
                  </td>
                </tr>
              ))}
              <tr style={{ background: ink }}>
                <td colSpan={3} style={{ padding: "10px 14px", color: paper, fontSize: 11, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.05em" }}>Importe total del contrato</td>
                <td colSpan={2} style={{ padding: "10px 14px", color: paper, fontFamily: "JetBrains Mono", fontWeight: 700, fontSize: 13 }}>{fmtMXN(mockContrato.monto)}</td>
              </tr>
            </tbody>
          </table>
        )}

        {sec === "Garantías" && (
          <div>
            {mockContrato.garantias.map(g => (
              <div key={g.tipo} style={{ border: `1px solid ${rule}`, borderRadius: 4, padding: "16px 18px", marginBottom: 12, display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <div>
                  <div style={{ fontWeight: 600, fontSize: 13, marginBottom: 4 }}>{g.tipo}</div>
                  <div style={{ fontSize: 12, color: muted }}>{fmtMXN(g.monto)} · Vence: {g.vencimiento}</div>
                </div>
                <div style={{ textAlign: "right" }}>
                  <EstadoBadge estado={g.diasRestantes < 30 ? "por vencer" : "vigente"} />
                  <div style={{ fontSize: 11, color: g.diasRestantes < 30 ? folio : muted, marginTop: 6, fontFamily: "JetBrains Mono" }}>{g.diasRestantes} días restantes</div>
                </div>
              </div>
            ))}
          </div>
        )}

        {sec === "Programa de obra" && (
          <div>
            <div style={{ marginBottom: 16 }}>
              <ResponsiveContainer width="100%" height={200}>
                <LineChart data={mockCurvaS} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke={rule} />
                  <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted }} />
                  <YAxis tick={{ fontSize: 11, fill: muted }} unit="%" domain={[0, 100]} />
                  <Tooltip contentStyle={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 3, fontSize: 12 }} formatter={(v) => v !== null ? [`${v}%`] : ["—"]} />
                  <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
                  <Line dataKey="programado" name="Programado" stroke={obra} strokeWidth={2} dot={false} />
                  <Line dataKey="real" name="Real" stroke={folio} strokeWidth={2} strokeDasharray="5 4" dot={false} connectNulls={false} />
                </LineChart>
              </ResponsiveContainer>
            </div>
            <p style={{ fontSize: 12, color: muted }}>El programa se actualiza automáticamente al aprobarse convenios modificatorios.</p>
          </div>
        )}

        {(sec === "Estimaciones" || sec === "Bitácora" || sec === "Convenios") && (
          <div style={{ color: muted, fontSize: 12.5 }}>
            <p style={{ marginBottom: 12 }}>Vista unificada del expediente — navega a la pestaña correspondiente para el detalle.</p>
            <div style={{ display: "grid", gap: 8 }}>
              {sec === "Estimaciones" && mockEstimaciones.slice(0, 3).map(e => (
                <div key={e.id} style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>EST-{String(e.id).padStart(3, "0")}</span>
                  <span style={{ fontSize: 12 }}>{e.periodo}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(e.monto)}</span>
                  <EstadoBadge estado={e.estado} />
                  <button style={{ fontSize: 11.5, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "3px 8px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4 }}><Download size={11} /> PDF</button>
                </div>
              ))}
              {sec === "Bitácora" && mockBitacora.slice(0, 4).map(b => (
                <div key={b.id} style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{b.folio}</span>
                  <span style={{ fontSize: 12 }}>{b.asunto}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>{b.fecha}</span>
                  <button style={{ fontSize: 11.5, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "3px 8px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4 }}><Download size={11} /> PDF</button>
                </div>
              ))}
              {sec === "Convenios" && mockConvenios.map(c => (
                <div key={c.id} style={{ display: "flex", justifyContent: "space-between", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, alignItems: "center" }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{c.id}</span>
                  <span style={{ fontSize: 12 }}>{c.tipo}</span>
                  <EstadoBadge estado={c.estado} />
                  <button style={{ fontSize: 11.5, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "3px 8px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4 }}><Download size={11} /> PDF</button>
                </div>
              ))}
            </div>
          </div>
        )}

        {sec === "Documentación" && (
          <div>
            {["Contrato original firmado", "Programa de obra original", "Acta de inicio", "Dictamen técnico previo"].map(doc => (
              <div key={doc} style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "10px 14px", border: `1px solid ${rule}`, borderRadius: 3, marginBottom: 8 }}>
                <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                  <FileText size={15} color={muted} />
                  <span style={{ fontSize: 12.5 }}>{doc}</span>
                </div>
                <button style={{ fontSize: 11.5, color: obra, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", display: "flex", alignItems: "center", gap: 4, fontFamily: "'IBM Plex Sans', sans-serif" }}>
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

// ─── HU-25: Administración de roles ──────────────────────────────────────────
function TabAdmin() {
  const usuarios = [
    { nombre: "Ing. R. Domínguez", email: "r.dominguez@constructora.mx", rol: "Superintendente", activo: true },
    { nombre: "Ing. A. Herrera", email: "a.herrera@sopot.gob.mx", rol: "Residente", activo: true },
    { nombre: "Ing. L. Martínez", email: "l.martinez@supervision.mx", rol: "Supervisor", activo: true },
    { nombre: "Lic. P. Gutiérrez", email: "p.gutierrez@sopot.gob.mx", rol: "Financiero", activo: true },
    { nombre: "Dir. C. Flores", email: "c.flores@sopot.gob.mx", rol: "Dependencia", activo: false },
  ];
  const roles = ["Superintendente", "Residente", "Supervisor", "Financiero", "Dependencia"];
  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
        <PrimaryBtn onClick={() => {}}>+ Invitar usuario</PrimaryBtn>
        <div style={{ fontSize: 12, color: muted }}>Los usuarios sin rol asignado no tienen acceso al sistema.</div>
      </div>
      <Card style={{ overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["Nombre", "Correo", "Rol en contrato", "Acceso", "Acciones"]} />
          <tbody>
            {usuarios.map((u, i) => (
              <tr key={u.email} style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                    <div style={{ width: 28, height: 28, borderRadius: "50%", background: obra, color: "#fff", display: "flex", alignItems: "center", justifyContent: "center", fontSize: 11, fontWeight: 700, flexShrink: 0 }}>
                      {u.nombre.split(" ").map(n => n[0]).join("").slice(0, 2)}
                    </div>
                    {u.nombre}
                  </div>
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 12, color: muted, fontFamily: "JetBrains Mono" }}>{u.email}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <select defaultValue={u.rol} style={{ border: `1px solid ${rule}`, background: paper2, borderRadius: 3, padding: "5px 8px", fontSize: 12, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none" }}>
                    {roles.map(r => <option key={r}>{r}</option>)}
                  </select>
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <EstadoBadge estado={u.activo ? "vigente" : "vencida"} />
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <SecondaryBtn onClick={() => {}} style={{ fontSize: 11 }}>{u.activo ? "Suspender" : "Activar"}</SecondaryBtn>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>
      <div style={{ marginTop: 20, background: observadoSoft, border: `1px solid ${observado}`, borderRadius: 4, padding: "12px 16px", fontSize: 12.5, color: observado }}>
        <strong>Restricciones por rol:</strong> Cada usuario solo puede realizar las acciones correspondientes a su rol asignado en este contrato.
      </div>
    </div>
  );
}

// ─── HU-26: Dashboard ─────────────────────────────────────────────────────────
function TabDashboard() {
  const totalContratado = mockContrato.monto;
  const ejercido = 10_200_000;
  const pctFisico = 63;
  const pctProg = 68;
  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", gap: 24 }}>
      <div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 14, marginBottom: 24 }}>
          {[
            { label: "Avance físico", value: `${pctFisico}%`, note: `Prog. ${pctProg}%`, color: obra },
            { label: "Monto ejercido", value: "$10.2M", note: `de ${fmtMXN(totalContratado)} contratados`, color: obra },
            { label: "Est. pendientes", value: "2", note: "014 vence pronto", color: observado },
            { label: "Garantías activas", value: "3", note: "1 por vencer", color: folio },
          ].map(k => (
            <Card key={k.label} style={{ padding: "14px 16px" }}>
              <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 22, fontWeight: 700, color: k.color }}>{k.value}</div>
              <div style={{ fontSize: 10, textTransform: "uppercase", letterSpacing: "0.05em", color: muted, marginTop: 2 }}>{k.label}</div>
              <div style={{ fontSize: 11, color: muted, marginTop: 6 }}>{k.note}</div>
            </Card>
          ))}
        </div>
        <Card style={{ padding: "18px 20px", marginBottom: 20 }}>
          <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 16 }}>Curva S — Avance físico</div>
          <ResponsiveContainer width="100%" height={200}>
            <LineChart data={mockCurvaS} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={rule} />
              <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} />
              <YAxis tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} unit="%" />
              <Tooltip contentStyle={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 3, fontSize: 12 }} formatter={(v) => v !== null ? [`${v}%`] : ["—"]} />
              <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
              <Line dataKey="programado" name="Programado" stroke={obra} strokeWidth={2} dot={false} />
              <Line dataKey="real" name="Real" stroke={folio} strokeWidth={2} strokeDasharray="5 4" dot={false} connectNulls={false} />
            </LineChart>
          </ResponsiveContainer>
        </Card>
        <Card style={{ padding: "16px 20px", marginBottom: 20 }}>
          <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 12 }}>Avance financiero</div>
          <div style={{ fontSize: 12, color: muted, marginBottom: 6, display: "flex", justifyContent: "space-between" }}>
            <span>{fmtMXN(ejercido)} ejercidos</span>
            <span style={{ fontFamily: "JetBrains Mono", color: obra, fontWeight: 600 }}>{Math.round(ejercido / totalContratado * 100)}%</span>
          </div>
          <div style={{ background: rule, borderRadius: 2, height: 8, overflow: "hidden" }}>
            <div style={{ background: obra, height: "100%", width: `${ejercido / totalContratado * 100}%`, borderRadius: 2 }} />
          </div>
          <div style={{ fontSize: 11, color: muted, marginTop: 8 }}>Contrato total: {fmtMXN(totalContratado)}</div>
        </Card>
        {/* Estimaciones y convenios */}
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14 }}>
          <Card style={{ padding: "14px 16px" }}>
            <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 10 }}>Estimaciones</div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <div>
                <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 28, fontWeight: 700, color: observado }}>2</div>
                <div style={{ fontSize: 11, color: muted }}>pendientes de resolución</div>
              </div>
              <div style={{ fontSize: 12, color: muted, textAlign: "right" }}>
                <div>Pagadas: <strong style={{ color: aprobado }}>3</strong></div>
                <div>Aprobadas: <strong style={{ color: obra }}>1</strong></div>
              </div>
            </div>
          </Card>
          <Card style={{ padding: "14px 16px" }}>
            <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 10 }}>Convenios activos</div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <div>
                <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 28, fontWeight: 700, color: folio }}>1</div>
                <div style={{ fontSize: 11, color: muted }}>en proceso (CM-008)</div>
              </div>
              <div style={{ fontSize: 12, color: muted, textAlign: "right" }}>
                <div>Aprobados: <strong style={{ color: aprobado }}>2</strong></div>
                <div>Variación: <strong style={{ color: observado }}>18%</strong></div>
              </div>
            </div>
          </Card>
        </div>
      </div>
      <AlertsPanel />
    </div>
  );
}

// ─── Main App ─────────────────────────────────────────────────────────────────
const TABS = [
  { id: "dashboard", label: "Dashboard", icon: <LayoutDashboard size={13} /> },
  { id: "estimaciones", label: "Estimaciones", icon: <FileText size={13} /> },
  { id: "avance", label: "Avance / Curva S", icon: <TrendingUp size={13} /> },
  { id: "bitacora", label: "Bitácora", icon: <BookOpen size={13} /> },
  { id: "convenios", label: "Convenios", icon: <GitBranch size={13} /> },
  { id: "expediente", label: "Expediente", icon: <Folder size={13} /> },
  { id: "finiquito", label: "Cierre / Finiquito", icon: <Shield size={13} /> },
  { id: "admin", label: "Usuarios", icon: <Users size={13} /> },
];

const ROLES = ["Superintendente", "Supervisor", "Residente", "Financiero", "Dependencia", "Administrador"];

export default function App() {
  const [activeTab, setActiveTab] = useState("dashboard");
  const [alertOpen, setAlertOpen] = useState(false);
  const [rol, setRol] = useState("Superintendente");
  const [showRolPicker, setShowRolPicker] = useState(false);

  return (
    <div style={{ minHeight: "100vh", background: paper, fontFamily: "'IBM Plex Sans', sans-serif" }}>
      {/* Top bar */}
      <div style={{ position: "sticky", top: 0, zIndex: 50, background: ink, color: paper, padding: "0 28px", display: "flex", alignItems: "center", gap: 14, height: 56, borderBottom: `3px solid ${folio}` }}>
        <div style={{ width: 32, height: 32, border: `2px solid ${paper}`, borderRadius: "50%", display: "flex", alignItems: "center", justifyContent: "center", fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 12 }}>FE</div>
        <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 16, letterSpacing: "0.02em" }}>FEPI</div>
        <div style={{ width: 1, height: 24, background: "rgba(255,255,255,0.15)" }} />
        <div style={{ fontSize: 12, color: "#C7C2B0" }}>
          Contrato <span style={{ fontFamily: "JetBrains Mono", color: paper }}>{mockContrato.id}</span>
          <span style={{ marginLeft: 8 }}>· {mockContrato.descripcion}</span>
        </div>
        <div style={{ marginLeft: "auto", display: "flex", alignItems: "center", gap: 14 }}>
          {/* Rol selector */}
          <div style={{ position: "relative" }}>
            <button onClick={() => setShowRolPicker(!showRolPicker)} style={{ background: "rgba(255,255,255,0.1)", border: "1px solid rgba(255,255,255,0.2)", borderRadius: 3, color: paper, padding: "5px 10px", fontSize: 11, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", display: "flex", alignItems: "center", gap: 5 }}>
              <span style={{ color: "#C7C2B0" }}>Rol:</span> {rol} <ChevronDown size={11} />
            </button>
            {showRolPicker && (
              <div style={{ position: "absolute", top: "110%", right: 0, background: paper2, border: `1px solid ${rule}`, borderRadius: 4, boxShadow: "0 8px 24px rgba(0,0,0,0.12)", zIndex: 200, minWidth: 160 }}>
                {ROLES.map(r => (
                  <button key={r} onClick={() => { setRol(r); setShowRolPicker(false); }} style={{ display: "block", width: "100%", textAlign: "left", padding: "9px 14px", fontSize: 12.5, color: r === rol ? obra : ink, background: r === rol ? obraSoft : "transparent", border: "none", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", fontWeight: r === rol ? 600 : 400 }}>
                    {r === rol && "✓ "}{r}
                  </button>
                ))}
              </div>
            )}
          </div>
          <button onClick={() => setAlertOpen(!alertOpen)} style={{ position: "relative", background: "none", border: "none", color: "#C7C2B0", cursor: "pointer", padding: 4 }}>
            <Bell size={16} />
            <span style={{ position: "absolute", top: 0, right: 0, width: 8, height: 8, background: folio, borderRadius: "50%", border: `1.5px solid ${ink}` }} />
          </button>
        </div>
      </div>

      {/* Tab bar */}
      <div style={{ position: "sticky", top: 56, zIndex: 40, background: "#EFEAE0", borderBottom: `1px solid ${rule}`, display: "flex", paddingLeft: 28, overflowX: "auto" }}>
        {TABS.map(t => (
          <button key={t.id} onClick={() => setActiveTab(t.id)} style={{ display: "flex", alignItems: "center", gap: 6, padding: "11px 18px", fontSize: 12.5, color: activeTab === t.id ? obra : muted, background: activeTab === t.id ? paper2 : "transparent", border: "none", borderBottom: `3px solid ${activeTab === t.id ? obra : "transparent"}`, fontWeight: activeTab === t.id ? 600 : 400, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", whiteSpace: "nowrap" }}>
            {t.icon}{t.label}
          </button>
        ))}
      </div>

      {/* Main content */}
      <div style={{ maxWidth: 1200, margin: "0 auto", padding: "28px 28px 60px" }}>
        <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 20 }}>
          <div style={{ fontSize: 10.5, letterSpacing: "0.12em", textTransform: "uppercase", color: folio, fontWeight: 700 }}>
            {TABS.find(t => t.id === activeTab)?.label}
          </div>
          <ChevronRight size={12} color={rule} />
          <div style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>{mockContrato.id}</div>
          <div style={{ marginLeft: "auto", fontSize: 11, color: muted, background: obraSoft, border: `1px solid ${obra}`, borderRadius: 10, padding: "2px 10px", fontWeight: 600 }}>
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
        <div style={{ position: "fixed", top: 56, right: 0, bottom: 0, width: 300, background: paper2, borderLeft: `1px solid ${rule}`, zIndex: 60, boxShadow: "-8px 0 30px rgba(26,34,56,0.1)", padding: 20, overflowY: "auto" }}>
          <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 16 }}>
            <div style={{ fontSize: 12, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: ink }}>Alertas activas</div>
            <button onClick={() => setAlertOpen(false)} style={{ background: "none", border: "none", cursor: "pointer", color: muted }}><X size={16} /></button>
          </div>
          <AlertsPanel />
        </div>
      )}

      {/* Footer */}
      <div style={{ borderTop: `1px solid ${rule}`, padding: "20px 28px", textAlign: "center", fontSize: 11.5, color: muted, fontFamily: "JetBrains Mono" }}>
        FEPI · Sistema de Seguimiento en Ejecución de Obra Pública · v2.0 · Junio 2026
      </div>
    </div>
  );
}
