import { useState } from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip,
  ResponsiveContainer, Area, AreaChart,
} from "recharts";
import {
  LayoutDashboard, FileText, TrendingUp, BookOpen,
  GitBranch, Folder, Bell, X, ChevronRight, Plus,
  AlertTriangle, Clock, CheckCircle, Search, Settings,
  MoreHorizontal, ArrowUpRight, LogOut,
} from "lucide-react";

// ─── Palette ─────────────────────────────────────────────────────────────────
const C = {
  bg:       "#0e0e0e",
  surface:  "#1a1a1a",
  surface2: "#222222",
  border:   "rgba(255,255,255,0.07)",
  borderHi: "rgba(255,255,255,0.14)",
  fg:       "#f5f5f5",
  fgMuted:  "#888888",
  fgSub:    "#555555",
  blue:     "#4f86f7",
  blueSoft: "rgba(79,134,247,0.15)",
  red:      "#f05252",
  redSoft:  "rgba(240,82,82,0.15)",
  amber:    "#f59e0b",
  amberSoft:"rgba(245,158,11,0.15)",
  green:    "#22c55e",
  greenSoft:"rgba(34,197,94,0.15)",
  purple:   "#a855f7",
};

// ─── Shared components ────────────────────────────────────────────────────────
function Badge({ label, color, soft }: { label: string; color: string; soft: string }) {
  return (
    <span style={{
      display: "inline-flex", alignItems: "center", gap: 5,
      background: soft, color, fontSize: 11.5, fontWeight: 600,
      padding: "3px 10px", borderRadius: 999,
      letterSpacing: "0.01em",
    }}>
      <span style={{ width: 5, height: 5, borderRadius: "50%", background: color, flexShrink: 0 }} />
      {label}
    </span>
  );
}

function statusBadge(estado: string) {
  const map: Record<string, [string, string]> = {
    Borrador:        [C.blue,  C.blueSoft],
    Enviada:         [C.blue,  C.blueSoft],
    Observada:       [C.amber, C.amberSoft],
    Aprobada:        [C.green, C.greenSoft],
    Pagada:          [C.green, C.greenSoft],
    "En evaluación": [C.amber, C.amberSoft],
    Aprobado:        [C.green, C.greenSoft],
    Rechazado:       [C.red,   C.redSoft],
  };
  const [color, soft] = map[estado] ?? [C.fgMuted, "rgba(136,136,136,0.15)"];
  return <Badge label={estado} color={color} soft={soft} />;
}

function Card({ children, style = {} }: { children: React.ReactNode; style?: React.CSSProperties }) {
  return (
    <div style={{
      background: C.surface, border: `1px solid ${C.border}`,
      borderRadius: 16, overflow: "hidden", ...style,
    }}>
      {children}
    </div>
  );
}

function SectionTitle({ children }: { children: React.ReactNode }) {
  return (
    <div style={{ fontSize: 14, fontWeight: 600, color: C.fg, marginBottom: 16 }}>
      {children}
    </div>
  );
}

function Stat({ value, label, color = C.blue }: { value: string; label: string; color?: string }) {
  return (
    <div style={{ padding: "18px 20px" }}>
      <div style={{ fontSize: 26, fontWeight: 700, color, lineHeight: 1 }}>{value}</div>
      <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 6, fontWeight: 500 }}>{label}</div>
    </div>
  );
}

// ─── Mock data ────────────────────────────────────────────────────────────────
const curvaS = [
  { mes: "Mar", prog: 8,  real: 8  },
  { mes: "Abr", prog: 24, real: 22 },
  { mes: "May", prog: 45, real: 41 },
  { mes: "Jun", prog: 68, real: 63 },
  { mes: "Jul", prog: 84, real: null },
  { mes: "Ago", prog: 95, real: null },
  { mes: "Sep", prog: 100,real: null },
];

const estimaciones = [
  { id: 14, periodo: "Jun 2026", monto: 1_248_300, estado: "Enviada",  dias: "18/20" },
  { id: 13, periodo: "May 2026", monto: 980_200,   estado: "Aprobada" },
  { id: 12, periodo: "Abr 2026", monto: 1_100_000, estado: "Pagada"   },
  { id: 11, periodo: "Mar 2026", monto: 1_050_500, estado: "Pagada"   },
  { id: 10, periodo: "Feb 2026", monto: 890_000,   estado: "Pagada"   },
];

const bitacora = [
  { folio: "0001", tipo: "Apertura",   fecha: "01 mar", asunto: "Inicio formal de obra",      firmas: "3/3",  done: true  },
  { folio: "0138", tipo: "Nota",       fecha: "03 jun", asunto: "Terminación losa eje B",     firmas: "3/3",  done: true  },
  { folio: "0140", tipo: "Nota",       fecha: "09 jun", asunto: "Terminación cimentación",    firmas: "2/3",  done: false },
  { folio: "0141", tipo: "Nota",       fecha: "14 jun", asunto: "Entrega trabe principal",    firmas: "3/3",  done: true  },
  { folio: "—",    tipo: "Incidencia", fecha: "11 jun", asunto: "Falla eléctrica temporal",   firmas: "—",    done: false },
];

const convenios = [
  { id: "CM-008", tipo: "Ampliación de plazo",  monto: "—",        estado: "En evaluación" },
  { id: "CM-006", tipo: "Incremento de monto",  monto: "$450,000", estado: "Aprobado"      },
  { id: "CM-003", tipo: "Ajuste de catálogo",   monto: "$120,000", estado: "Aprobado"      },
];

const alertas = [
  { nivel: "red",   texto: "Estimación 014 vence en 2 días hábiles", ref: "EST-014" },
  { nivel: "amber", texto: "Nota 0140 — firma pendiente (Supervisión)", ref: "FOL-0140" },
  { nivel: "amber", texto: "Garantía de cumplimiento vence en 22 días", ref: "GAR-001" },
  { nivel: "gray",  texto: "Convenio CM-008 pendiente de resolución",  ref: "CM-008"  },
];

function fmtMXN(n: number) {
  return new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN", maximumFractionDigits: 0 }).format(n);
}

// ─── Alert panel ──────────────────────────────────────────────────────────────
function AlertsPanel() {
  const colorMap: Record<string, [string, string]> = {
    red:   [C.red,   C.redSoft],
    amber: [C.amber, C.amberSoft],
    gray:  [C.fgMuted, "rgba(136,136,136,0.1)"],
  };
  return (
    <Card>
      <div style={{ padding: "18px 20px 10px" }}>
        <div style={{ fontSize: 13, fontWeight: 600, color: C.fg, marginBottom: 14, display: "flex", alignItems: "center", gap: 8 }}>
          <Bell size={14} color={C.fgMuted} />
          Alertas activas
        </div>
        {alertas.map((a, i) => {
          const [color, bg] = colorMap[a.nivel];
          return (
            <div key={i} style={{ display: "flex", alignItems: "flex-start", gap: 10, padding: "10px 12px", background: bg, borderRadius: 10, marginBottom: 8 }}>
              <div style={{ width: 7, height: 7, borderRadius: "50%", background: color, flexShrink: 0, marginTop: 4 }} />
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: 12, color: C.fg, lineHeight: 1.45 }}>{a.texto}</div>
                <div style={{ fontFamily: "JetBrains Mono", fontSize: 10, color, marginTop: 3, fontWeight: 600 }}>{a.ref}</div>
              </div>
            </div>
          );
        })}
      </div>
    </Card>
  );
}

// ─── Dashboard ────────────────────────────────────────────────────────────────
function TabDashboard() {
  const stats = [
    { value: "63%",   label: "Avance físico",       color: C.blue  },
    { value: "$10.2M",label: "Ejercido de $11M",    color: C.green },
    { value: "2",     label: "Estimaciones pend.",  color: C.amber },
    { value: "1",     label: "Garantías por vencer",color: C.red   },
  ];

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: 20 }}>
      {/* KPI row */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
        {stats.map(s => (
          <Card key={s.label}>
            <div style={{ padding: "18px 20px" }}>
              <div style={{ fontSize: 28, fontWeight: 700, color: s.color, lineHeight: 1 }}>{s.value}</div>
              <div style={{ fontSize: 12, color: C.fgMuted, marginTop: 8, fontWeight: 500 }}>{s.label}</div>
            </div>
          </Card>
        ))}
      </div>

      {/* Curva S */}
      <div style={{ display: "grid", gridTemplateColumns: "1fr", gap: 16 }}>
        <Card>
          <div style={{ padding: "20px 22px 8px" }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 20 }}>
              <SectionTitle>Curva S — Avance físico</SectionTitle>
              <div style={{ display: "flex", gap: 16, fontSize: 11.5 }}>
                <span style={{ display: "flex", alignItems: "center", gap: 5, color: C.fgMuted }}>
                  <span style={{ width: 24, height: 2, background: C.blue, display: "inline-block", borderRadius: 2 }} />Programado
                </span>
                <span style={{ display: "flex", alignItems: "center", gap: 5, color: C.fgMuted }}>
                  <span style={{ width: 24, height: 2, background: C.red, display: "inline-block", borderRadius: 2, opacity: 0.8 }} />Real
                </span>
              </div>
            </div>
            <ResponsiveContainer width="100%" height={190}>
              <AreaChart data={curvaS} margin={{ top: 4, right: 4, bottom: 0, left: -20 }}>
                <defs>
                  <linearGradient id="gBlue" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={C.blue} stopOpacity={0.25} />
                    <stop offset="100%" stopColor={C.blue} stopOpacity={0} />
                  </linearGradient>
                  <linearGradient id="gRed" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={C.red} stopOpacity={0.2} />
                    <stop offset="100%" stopColor={C.red} stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.05)" />
                <XAxis dataKey="mes" tick={{ fontSize: 11, fill: C.fgMuted, fontFamily: "JetBrains Mono" }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fontSize: 11, fill: C.fgMuted, fontFamily: "JetBrains Mono" }} unit="%" axisLine={false} tickLine={false} domain={[0, 100]} />
                <Tooltip
                  contentStyle={{ background: C.surface2, border: `1px solid ${C.borderHi}`, borderRadius: 10, fontSize: 12, color: C.fg }}
                  cursor={{ stroke: C.borderHi }}
                  formatter={(v: number) => [`${v}%`]}
                />
                <Area key="prog-dash" type="monotone" dataKey="prog" name="Programado" stroke={C.blue} strokeWidth={2} fill="url(#gBlue)" dot={false} />
                <Area key="real-dash" type="monotone" dataKey="real" name="Real" stroke={C.red} strokeWidth={2} fill="url(#gRed)" dot={false} connectNulls={false} />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </Card>
      </div>

      {/* Financiero + Convenios */}
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16 }}>
        <Card>
          <div style={{ padding: "20px 22px" }}>
            <SectionTitle>Avance financiero</SectionTitle>
            <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 10 }}>
              <span style={{ fontSize: 12.5, color: C.fgMuted }}>$10,200,000 ejercidos</span>
              <span style={{ fontFamily: "JetBrains Mono", fontSize: 13, color: C.blue, fontWeight: 600 }}>93%</span>
            </div>
            <div style={{ background: C.surface2, borderRadius: 999, height: 8, overflow: "hidden" }}>
              <div style={{ background: `linear-gradient(90deg, ${C.blue}, #7ca8ff)`, height: "100%", width: "93%", borderRadius: 999 }} />
            </div>
            <div style={{ fontSize: 11.5, color: C.fgSub, marginTop: 10 }}>Contrato total: $11,000,000</div>
          </div>
        </Card>
        <Card>
          <div style={{ padding: "20px 22px" }}>
            <SectionTitle>Convenios activos</SectionTitle>
            {convenios.map((c, i) => (
              <div key={c.id} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", paddingBottom: i < convenios.length - 1 ? 10 : 0, marginBottom: i < convenios.length - 1 ? 10 : 0, borderBottom: i < convenios.length - 1 ? `1px solid ${C.border}` : "none" }}>
                <div>
                  <div style={{ fontSize: 13, fontWeight: 500, color: C.fg }}>{c.id}</div>
                  <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 2 }}>{c.tipo}</div>
                </div>
                {statusBadge(c.estado)}
              </div>
            ))}
          </div>
        </Card>
      </div>
    </div>
  );
}

// ─── Estimaciones ─────────────────────────────────────────────────────────────
function TabEstimaciones() {
  const [selected, setSelected] = useState<typeof estimaciones[0] | null>(null);
  const [obsTab, setObsTab] = useState("Carátula");
  const tabs = ["Carátula", "Generadores", "Notas vinculadas (2)", "Historial"];

  if (selected) {
    return (
      <div>
        <button onClick={() => setSelected(null)} style={{ display: "flex", alignItems: "center", gap: 6, fontSize: 13, color: C.blue, background: "none", border: "none", cursor: "pointer", padding: "0 0 18px 0" }}>
          ← Volver a bandeja
        </button>

        {/* Header card */}
        <Card style={{ marginBottom: 16 }}>
          <div style={{ padding: "20px 24px", display: "flex", alignItems: "center", justifyContent: "space-between" }}>
            <div>
              <div style={{ fontSize: 11, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.08em", color: C.fgMuted, marginBottom: 6 }}>Estimación</div>
              <div style={{ fontSize: 22, fontWeight: 700, color: C.fg }}>N.° {String(selected.id).padStart(3, "0")}</div>
              <div style={{ fontSize: 12.5, color: C.fgMuted, marginTop: 4 }}>CT-2026-014 · {selected.periodo} · Ing. R. Domínguez</div>
            </div>
            <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-end", gap: 10 }}>
              {statusBadge(selected.estado)}
              {selected.dias && (
                <div style={{ display: "flex", alignItems: "center", gap: 6, background: C.amberSoft, borderRadius: 999, padding: "5px 12px" }}>
                  <Clock size={11} color={C.amber} />
                  <span style={{ fontSize: 11.5, color: C.amber, fontWeight: 600, fontFamily: "JetBrains Mono" }}>{selected.dias} días</span>
                </div>
              )}
            </div>
          </div>
          {/* KPIs */}
          <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", borderTop: `1px solid ${C.border}` }}>
            {[
              { v: fmtMXN(selected.monto), l: "Monto estimado" },
              { v: "$9,450,000",            l: "Acumulado contrato" },
              { v: "$1,550,000",            l: "Saldo pendiente" },
            ].map((k, i) => (
              <div key={k.l} style={{ padding: "16px 22px", borderRight: i < 2 ? `1px solid ${C.border}` : "none" }}>
                <div style={{ fontSize: 18, fontWeight: 700, color: C.blue }}>{k.v}</div>
                <div style={{ fontSize: 11, color: C.fgMuted, marginTop: 4, textTransform: "uppercase", letterSpacing: "0.05em" }}>{k.l}</div>
              </div>
            ))}
          </div>
          {/* Tabs */}
          <div style={{ display: "flex", borderTop: `1px solid ${C.border}`, padding: "0 8px" }}>
            {tabs.map(t => (
              <button key={t} onClick={() => setObsTab(t)} style={{
                padding: "12px 16px", fontSize: 12.5, color: obsTab === t ? C.blue : C.fgMuted,
                background: "none", border: "none", borderBottom: `2px solid ${obsTab === t ? C.blue : "transparent"}`,
                cursor: "pointer", fontWeight: obsTab === t ? 600 : 400,
                transition: "color 0.15s",
              }}>{t}</button>
            ))}
          </div>
        </Card>

        {/* Body */}
        <div style={{ display: "grid", gridTemplateColumns: "1fr 280px", gap: 16 }}>
          {/* Conceptos */}
          <Card>
            <div style={{ padding: "20px 22px" }}>
              <SectionTitle>Conceptos de la estimación</SectionTitle>
              <div style={{ borderRadius: 10, overflow: "hidden", border: `1px solid ${C.border}` }}>
                <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
                  <thead>
                    <tr style={{ background: C.surface2 }}>
                      {["Clave", "Descripción", "Cantidad", "Importe"].map(h => (
                        <th key={h} style={{ textAlign: "left", padding: "10px 14px", color: C.fgMuted, fontSize: 11, textTransform: "uppercase", letterSpacing: "0.06em", fontWeight: 600 }}>{h}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {[
                      { clave: "03.012", desc: "Losa de cimentación f'c=250", cant: "120 m³", imp: "$486,000", flag: false },
                      { clave: "05.004", desc: "Trabe de liga eje B-C",        cant: "34 ml",   imp: "$210,300", flag: true  },
                      { clave: "06.001", desc: "Castillo K-1",                 cant: "18 pza",  imp: "$95,400",  flag: false },
                      { clave: "07.002", desc: "Muro de block hueco 15cm",    cant: "240 m²",  imp: "$456,600", flag: false },
                    ].map(r => (
                      <tr key={r.clave} style={{ background: r.flag ? C.redSoft : "transparent", borderTop: `1px solid ${C.border}` }}>
                        <td style={{ padding: "10px 14px", fontFamily: "JetBrains Mono", fontSize: 11.5, color: r.flag ? C.red : C.fgMuted }}>{r.clave}</td>
                        <td style={{ padding: "10px 14px", color: C.fg }}>{r.desc}</td>
                        <td style={{ padding: "10px 14px", fontFamily: "JetBrains Mono", fontSize: 11.5, color: C.fgMuted }}>{r.cant}</td>
                        <td style={{ padding: "10px 14px", fontFamily: "JetBrains Mono", fontSize: 12, color: C.fg, fontWeight: 500 }}>{r.imp}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <div style={{ fontSize: 11.5, color: C.fgSub, marginTop: 10 }}>Documento del contratista — solo lectura</div>
            </div>
          </Card>

          {/* Observations */}
          <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <Card>
              <div style={{ padding: "18px 18px 14px" }}>
                <div style={{ fontSize: 12, fontWeight: 600, color: C.blue, textTransform: "uppercase", letterSpacing: "0.07em", marginBottom: 12 }}>Observaciones</div>
                {[
                  { ref: "05.004", txt: "No coincide con el generador: reporta 34 ml, el generador muestra 31 ml.", who: "Ing. L. Martínez", hora: "hace 2h", color: C.red },
                  { ref: "General", txt: "Falta la firma del residente en la nota 0141.", who: "Ing. L. Martínez", hora: "hace 2h", color: C.blue },
                ].map((n, i) => (
                  <div key={i} style={{ background: C.surface2, borderRadius: 10, padding: "10px 12px", marginBottom: 8, borderLeft: `3px solid ${n.color}` }}>
                    <div style={{ fontFamily: "JetBrains Mono", fontSize: 10.5, color: n.color, fontWeight: 600, marginBottom: 4 }}>{n.ref}</div>
                    <div style={{ fontSize: 12, color: C.fg, lineHeight: 1.45 }}>{n.txt}</div>
                    <div style={{ fontSize: 10.5, color: C.fgMuted, marginTop: 5 }}>{n.who} · {n.hora}</div>
                  </div>
                ))}
                <button style={{ width: "100%", background: C.surface2, border: `1px dashed ${C.borderHi}`, borderRadius: 10, padding: "8px", fontSize: 12, color: C.fgMuted, cursor: "pointer", marginBottom: 12 }}>
                  + Nueva observación
                </button>
                <div style={{ display: "flex", gap: 8 }}>
                  <button style={{ flex: 1, background: C.amberSoft, color: C.amber, border: "none", borderRadius: 10, padding: "9px", fontSize: 12.5, fontWeight: 600, cursor: "pointer" }}>Observada</button>
                  <button style={{ flex: 1, background: C.greenSoft, color: C.green, border: "none", borderRadius: 10, padding: "9px", fontSize: 12.5, fontWeight: 600, cursor: "pointer" }}>Aprobada</button>
                </div>
              </div>
            </Card>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20 }}>
        <div style={{ flex: 1, position: "relative" }}>
          <Search size={14} style={{ position: "absolute", left: 12, top: "50%", transform: "translateY(-50%)", color: C.fgMuted }} />
          <input placeholder="Buscar folio..." style={{ width: "100%", background: C.surface, border: `1px solid ${C.border}`, borderRadius: 10, padding: "9px 12px 9px 34px", fontSize: 13, color: C.fg, outline: "none" }} />
        </div>
        <select style={{ background: C.surface, border: `1px solid ${C.border}`, borderRadius: 10, padding: "9px 12px", fontSize: 13, color: C.fgMuted, outline: "none" }}>
          <option>Todos los estados</option>
          <option>Enviada</option>
          <option>Aprobada</option>
          <option>Pagada</option>
        </select>
        <button style={{ display: "flex", alignItems: "center", gap: 6, background: C.blue, color: "#fff", border: "none", borderRadius: 10, padding: "9px 16px", fontSize: 13, fontWeight: 600, cursor: "pointer" }}>
          <Plus size={14} /> Nueva
        </button>
      </div>

      <Card>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
          <thead>
            <tr>
              {["N.°", "Periodo", "Monto", "Estado", "Días plazo", ""].map(h => (
                <th key={h} style={{ textAlign: "left", padding: "12px 18px", color: C.fgMuted, fontSize: 11, textTransform: "uppercase", letterSpacing: "0.07em", fontWeight: 600, borderBottom: `1px solid ${C.border}` }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {estimaciones.map((e, i) => (
              <tr key={e.id} style={{ borderBottom: i < estimaciones.length - 1 ? `1px solid ${C.border}` : "none" }}
                onMouseEnter={el => (el.currentTarget as HTMLTableRowElement).style.background = C.surface2}
                onMouseLeave={el => (el.currentTarget as HTMLTableRowElement).style.background = "transparent"}
              >
                <td style={{ padding: "14px 18px", fontFamily: "JetBrains Mono", fontWeight: 600, color: C.blue }}>{String(e.id).padStart(3, "0")}</td>
                <td style={{ padding: "14px 18px", color: C.fgMuted }}>{e.periodo}</td>
                <td style={{ padding: "14px 18px", fontFamily: "JetBrains Mono", color: C.fg, fontWeight: 500 }}>{fmtMXN(e.monto)}</td>
                <td style={{ padding: "14px 18px" }}>{statusBadge(e.estado)}</td>
                <td style={{ padding: "14px 18px" }}>
                  {e.dias
                    ? <span style={{ display: "flex", alignItems: "center", gap: 5, fontSize: 12.5, color: C.amber, fontFamily: "JetBrains Mono", fontWeight: 600 }}><Clock size={11} />{e.dias}</span>
                    : <span style={{ color: C.fgSub }}>—</span>}
                </td>
                <td style={{ padding: "14px 18px" }}>
                  <button onClick={() => setSelected(e)} style={{ display: "flex", alignItems: "center", gap: 4, fontSize: 12, color: C.blue, background: C.blueSoft, border: "none", borderRadius: 8, padding: "5px 10px", cursor: "pointer", fontWeight: 500 }}>
                    Ver <ArrowUpRight size={11} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>
    </div>
  );
}

// ─── Bitácora ─────────────────────────────────────────────────────────────────
function TabBitacora() {
  const [modal, setModal] = useState(false);
  return (
    <div>
      <div style={{ display: "flex", gap: 10, marginBottom: 20, alignItems: "center" }}>
        {["+ Nota", "+ Minuta", "+ Incidencia"].map((l, li) => (
          <button key={l} onClick={() => setModal(true)} style={{
            background: li === 0 ? C.blue : C.surface, color: li === 0 ? "#fff" : C.fgMuted,
            border: `1px solid ${li === 0 ? C.blue : C.border}`, borderRadius: 10,
            padding: "8px 14px", fontSize: 12.5, fontWeight: 600, cursor: "pointer",
          }}>{l}</button>
        ))}
        <button style={{ marginLeft: "auto", background: C.surface, border: `1px solid ${C.border}`, borderRadius: 10, padding: "8px 14px", fontSize: 12.5, color: C.fgMuted, cursor: "pointer" }}>
          Exportar
        </button>
      </div>

      <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
        {bitacora.map((b, i) => (
          <Card key={b.folio + i}>
            <div style={{ padding: "14px 18px", display: "flex", alignItems: "center", gap: 16 }}>
              <div style={{ width: 36, height: 36, borderRadius: 10, background: b.done ? C.greenSoft : b.tipo === "Incidencia" ? C.redSoft : C.blueSoft, display: "flex", alignItems: "center", justifyContent: "center", flexShrink: 0 }}>
                {b.tipo === "Incidencia" ? <AlertTriangle size={16} color={C.red} /> : b.done ? <CheckCircle size={16} color={C.green} /> : <Clock size={16} color={C.blue} />}
              </div>
              <div style={{ flex: 1 }}>
                <div style={{ fontSize: 13.5, fontWeight: 500, color: C.fg }}>{b.asunto}</div>
                <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 3, display: "flex", gap: 12 }}>
                  <span style={{ fontFamily: "JetBrains Mono" }}>{b.folio}</span>
                  <span>{b.tipo}</span>
                  <span>{b.fecha}</span>
                </div>
              </div>
              <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                {b.firmas !== "—" && (
                  <div style={{ display: "flex", alignItems: "center", gap: 5, fontSize: 12, fontFamily: "JetBrains Mono", color: b.firmas === "3/3" ? C.green : C.amber, fontWeight: 600 }}>
                    <CheckCircle size={12} />
                    {b.firmas}
                  </div>
                )}
                <button style={{ background: C.surface2, border: "none", borderRadius: 8, padding: "6px 8px", cursor: "pointer", color: C.fgMuted }}>
                  <MoreHorizontal size={14} />
                </button>
              </div>
            </div>
          </Card>
        ))}
      </div>

      {modal && (
        <div style={{ position: "fixed", inset: 0, background: "rgba(0,0,0,0.7)", zIndex: 100, display: "flex", alignItems: "center", justifyContent: "center", backdropFilter: "blur(4px)" }}>
          <div style={{ background: C.surface, borderRadius: 20, width: 420, border: `1px solid ${C.borderHi}`, overflow: "hidden", boxShadow: "0 32px 80px rgba(0,0,0,0.5)" }}>
            <div style={{ padding: "18px 20px 14px", display: "flex", alignItems: "center", justifyContent: "space-between", borderBottom: `1px solid ${C.border}` }}>
              <div>
                <div style={{ fontFamily: "JetBrains Mono", fontSize: 10.5, color: C.fgMuted }}>FOLIO-0142</div>
                <div style={{ fontSize: 16, fontWeight: 700, color: C.fg, marginTop: 2 }}>Nueva nota de bitácora</div>
              </div>
              <button onClick={() => setModal(false)} style={{ background: C.surface2, border: "none", borderRadius: 8, padding: 6, cursor: "pointer", color: C.fgMuted, display: "flex" }}>
                <X size={16} />
              </button>
            </div>
            <div style={{ padding: "18px 20px" }}>
              <div style={{ marginBottom: 14 }}>
                <div style={{ fontSize: 11, textTransform: "uppercase", letterSpacing: "0.06em", color: C.fgMuted, marginBottom: 8 }}>Tipo</div>
                <div style={{ display: "flex", gap: 6, flexWrap: "wrap" as const }}>
                  {["Aviso al residente", "Solicitud convenio", "Cambio procedimiento"].map((t, ti) => (
                    <span key={t} style={{ fontSize: 12, border: `1px solid ${ti === 0 ? C.blue : C.border}`, color: ti === 0 ? C.blue : C.fgMuted, background: ti === 0 ? C.blueSoft : "transparent", padding: "5px 12px", borderRadius: 999, cursor: "pointer" }}>{t}</span>
                  ))}
                </div>
              </div>
              {[
                { label: "Asunto", val: "Término de cimentación eje C-D" },
                { label: "Contenido", val: "Se concluyó la cimentación del eje C-D conforme a proyecto. Se solicita verificación para liberar concepto.", multi: true },
              ].map(f => (
                <div key={f.label} style={{ marginBottom: 14 }}>
                  <div style={{ fontSize: 11, textTransform: "uppercase", letterSpacing: "0.06em", color: C.fgMuted, marginBottom: 6 }}>{f.label}</div>
                  <div style={{ background: C.surface2, borderRadius: 10, padding: "10px 12px", fontSize: 13, color: C.fg, minHeight: f.multi ? 72 : "auto", lineHeight: 1.5 }}>{f.val}</div>
                </div>
              ))}
              <div style={{ marginBottom: 18 }}>
                <div style={{ fontSize: 11, textTransform: "uppercase", letterSpacing: "0.06em", color: C.fgMuted, marginBottom: 8 }}>Firmas</div>
                <div style={{ display: "flex", gap: 8 }}>
                  {[
                    { l: "Superintendente", done: true },
                    { l: "Residencia", done: false },
                    { l: "Supervisión", done: false },
                  ].map(s => (
                    <div key={s.l} style={{ flex: 1, border: `1.5px solid ${s.done ? C.green : C.border}`, background: s.done ? C.greenSoft : "transparent", borderRadius: 10, padding: "10px 6px", textAlign: "center", fontSize: 11, color: s.done ? C.green : C.fgMuted, fontWeight: s.done ? 600 : 400 }}>
                      {s.done ? `${s.l} ✓` : s.l}
                    </div>
                  ))}
                </div>
              </div>
              <button style={{ width: "100%", background: C.blue, color: "#fff", border: "none", borderRadius: 12, padding: 13, fontWeight: 700, fontSize: 14, cursor: "pointer" }}>
                Firmar y registrar nota
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// ─── Convenios ────────────────────────────────────────────────────────────────
function TabConvenios() {
  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20 }}>
        <button style={{ display: "flex", alignItems: "center", gap: 6, background: C.blue, color: "#fff", border: "none", borderRadius: 10, padding: "9px 16px", fontSize: 13, fontWeight: 600, cursor: "pointer" }}>
          <Plus size={14} /> Nueva solicitud
        </button>
        <div style={{ marginLeft: "auto", display: "flex", alignItems: "center", gap: 6, background: C.amberSoft, borderRadius: 999, padding: "6px 14px" }}>
          <AlertTriangle size={12} color={C.amber} />
          <span style={{ fontSize: 12, color: C.amber, fontWeight: 600 }}>Variación proyectada: 18% / límite 25%</span>
        </div>
      </div>

      <Card style={{ marginBottom: 20 }}>
        <table style={{ width: "100%", borderCollapse: "collapse" }}>
          <thead>
            <tr>
              {["ID", "Tipo", "Variación", "Estado", ""].map(h => (
                <th key={h} style={{ textAlign: "left", padding: "12px 18px", color: C.fgMuted, fontSize: 11, textTransform: "uppercase", letterSpacing: "0.07em", fontWeight: 600, borderBottom: `1px solid ${C.border}` }}>{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {convenios.map((c, i) => (
              <tr key={c.id} style={{ borderBottom: i < convenios.length - 1 ? `1px solid ${C.border}` : "none" }}>
                <td style={{ padding: "14px 18px", fontFamily: "JetBrains Mono", fontWeight: 600, color: C.blue }}>{c.id}</td>
                <td style={{ padding: "14px 18px", color: C.fg }}>{c.tipo}</td>
                <td style={{ padding: "14px 18px", fontFamily: "JetBrains Mono", color: C.fgMuted }}>{c.monto}</td>
                <td style={{ padding: "14px 18px" }}>{statusBadge(c.estado)}</td>
                <td style={{ padding: "14px 18px" }}>
                  <button style={{ fontSize: 12, color: C.blue, background: C.blueSoft, border: "none", borderRadius: 8, padding: "5px 10px", cursor: "pointer" }}>Ver →</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Card>

      <div style={{ fontSize: 13, fontWeight: 600, color: C.fg, marginBottom: 14 }}>Flujo de aprobación — CM-008</div>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12 }}>
        {[
          { n: "1", label: "Solicitada",          done: true,   active: false },
          { n: "2", label: "Dictamen Supervisión", done: true,   active: false },
          { n: "3", label: "Promovida Residencia", done: false,  active: true  },
          { n: "4", label: "Resolución Dependencia",done: false, active: false },
        ].map(s => (
          <Card key={s.n} style={{ border: `1px solid ${s.done ? C.green + "44" : s.active ? C.blue + "44" : C.border}` }}>
            <div style={{ padding: "16px 18px" }}>
              <div style={{ fontFamily: "JetBrains Mono", fontSize: 24, fontWeight: 700, color: s.done ? C.green : s.active ? C.blue : C.fgSub, opacity: 0.4 }}>{s.n}</div>
              <div style={{ fontSize: 12.5, fontWeight: s.active ? 600 : 400, color: s.done ? C.green : s.active ? C.blue : C.fgMuted, marginTop: 8 }}>{s.label}</div>
              <div style={{ fontSize: 11, marginTop: 6, color: s.done ? C.green : s.active ? C.blue : C.fgSub }}>
                {s.done ? "✓ Completado" : s.active ? "→ En proceso" : "Pendiente"}
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}

// ─── Avance ───────────────────────────────────────────────────────────────────
function TabAvance() {
  return (
    <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
      <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
        <Card>
          <div style={{ padding: "20px 22px 10px" }}>
            <SectionTitle>Curva S — Avance físico vs. programado</SectionTitle>
            <ResponsiveContainer width="100%" height={260}>
              <AreaChart data={curvaS} margin={{ top: 4, right: 4, bottom: 0, left: -20 }}>
                <defs>
                  <linearGradient id="gBlue2" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={C.blue} stopOpacity={0.3} />
                    <stop offset="100%" stopColor={C.blue} stopOpacity={0} />
                  </linearGradient>
                  <linearGradient id="gRed2" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={C.red} stopOpacity={0.2} />
                    <stop offset="100%" stopColor={C.red} stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.05)" />
                <XAxis dataKey="mes" tick={{ fontSize: 11, fill: C.fgMuted, fontFamily: "JetBrains Mono" }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fontSize: 11, fill: C.fgMuted, fontFamily: "JetBrains Mono" }} unit="%" axisLine={false} tickLine={false} domain={[0, 100]} />
                <Tooltip contentStyle={{ background: C.surface2, border: `1px solid ${C.borderHi}`, borderRadius: 10, fontSize: 12, color: C.fg }} formatter={(v: number) => [`${v}%`]} />
                <Area key="prog-avance" type="monotone" dataKey="prog" name="Programado" stroke={C.blue} strokeWidth={2} fill="url(#gBlue2)" dot={false} />
                <Area key="real-avance" type="monotone" dataKey="real" name="Real" stroke={C.red} strokeWidth={2} fill="url(#gRed2)" dot={false} connectNulls={false} />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </Card>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12 }}>
          {[
            { v: "63%", l: "Avance físico",     c: C.blue  },
            { v: "-5pp",l: "Desviación",         c: C.red   },
            { v: "Jun", l: "Periodo activo",     c: C.green },
          ].map(k => (
            <Card key={k.l}><Stat value={k.v} label={k.l} color={k.c} /></Card>
          ))}
        </div>
      </div>
    </div>
  );
}

// ─── Expediente ───────────────────────────────────────────────────────────────
function TabExpediente() {
  const secs = ["Datos generales", "Catálogo", "Programa de obra", "Garantías", "Estimaciones", "Bitácora", "Convenios"];
  const [sec, setSec] = useState("Datos generales");
  return (
    <div style={{ display: "grid", gridTemplateColumns: "190px 1fr", gap: 16 }}>
      <Card>
        <div style={{ padding: 8 }}>
          {secs.map(s => (
            <button key={s} onClick={() => setSec(s)} style={{
              display: "block", width: "100%", textAlign: "left",
              padding: "9px 12px", fontSize: 12.5, color: sec === s ? C.blue : C.fgMuted,
              fontWeight: sec === s ? 600 : 400, background: sec === s ? C.blueSoft : "transparent",
              border: "none", borderRadius: 8, cursor: "pointer", marginBottom: 2,
              transition: "background 0.15s",
            }}>{s}</button>
          ))}
        </div>
      </Card>
      <Card>
        <div style={{ padding: "22px 24px" }}>
          <div style={{ fontSize: 18, fontWeight: 700, color: C.fg, marginBottom: 22 }}>{sec}</div>
          {sec === "Datos generales" ? (
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
              {[
                { l: "Número de contrato",    v: "CT-2026-014" },
                { l: "Tipo",                  v: "A precio unitario" },
                { l: "Monto contratado",      v: "$11,000,000.00" },
                { l: "Anticipo",              v: "$1,100,000.00 (10%)" },
                { l: "Fecha de inicio",       v: "01 marzo 2026" },
                { l: "Fecha de término",      v: "30 septiembre 2026" },
                { l: "Contratista",           v: "Constructora Domínguez S.A. de C.V." },
                { l: "Dependencia",           v: "SOPOT Edo. de México" },
                { l: "Residente de obra",     v: "Ing. A. Herrera" },
                { l: "Supervisor externo",    v: "Ing. L. Martínez" },
              ].map(f => (
                <div key={f.l} style={{ background: C.surface2, borderRadius: 10, padding: "12px 14px" }}>
                  <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.06em", color: C.fgMuted, marginBottom: 4 }}>{f.l}</div>
                  <div style={{ fontSize: 13.5, color: C.fg, fontFamily: "JetBrains Mono", fontWeight: 500 }}>{f.v}</div>
                </div>
              ))}
            </div>
          ) : (
            <div style={{ color: C.fgMuted, fontSize: 13, padding: 24, background: C.surface2, borderRadius: 12, textAlign: "center", border: `1px dashed ${C.borderHi}` }}>
              {sec} — disponible en producción
            </div>
          )}
        </div>
      </Card>
    </div>
  );
}

// ─── Main App ─────────────────────────────────────────────────────────────────
type TabId = "dashboard" | "estimaciones" | "avance" | "bitacora" | "convenios" | "expediente";

const NAV: { id: TabId; label: string; icon: React.ReactNode }[] = [
  { id: "dashboard",    label: "Dashboard",    icon: <LayoutDashboard size={15} /> },
  { id: "estimaciones", label: "Estimaciones", icon: <FileText size={15} />        },
  { id: "avance",       label: "Avance",       icon: <TrendingUp size={15} />      },
  { id: "bitacora",     label: "Bitácora",     icon: <BookOpen size={15} />        },
  { id: "convenios",    label: "Convenios",    icon: <GitBranch size={15} />       },
  { id: "expediente",   label: "Expediente",   icon: <Folder size={15} />          },
];

export default function App() {
  const [tab, setTab] = useState<TabId>("dashboard");
  const [alertOpen, setAlertOpen] = useState(false);

  const alertCount = alertas.filter(a => a.nivel !== "gray").length;

  return (
    <div style={{ display: "flex", flexDirection: "column", minHeight: "100vh", background: C.bg, color: C.fg, fontFamily: "'Inter', system-ui, sans-serif" }}>

      {/* ── Top bar ── */}
      <header style={{
        position: "sticky", top: 0, zIndex: 50,
        background: C.surface, borderBottom: `1px solid ${C.border}`,
        display: "flex", alignItems: "center", gap: 0, height: 56, flexShrink: 0,
      }}>
        {/* Logo */}
        <div style={{ display: "flex", alignItems: "center", gap: 9, padding: "0 20px", borderRight: `1px solid ${C.border}`, height: "100%", flexShrink: 0 }}>
          <div style={{ width: 30, height: 30, borderRadius: 9, background: `linear-gradient(135deg, ${C.blue}, #7ca8ff)`, display: "flex", alignItems: "center", justifyContent: "center", fontWeight: 800, fontSize: 12, color: "#fff" }}>FE</div>
          <div>
            <div style={{ fontSize: 13.5, fontWeight: 700, color: C.fg, lineHeight: 1 }}>FEPI</div>
            <div style={{ fontSize: 10, color: C.fgMuted, marginTop: 1 }}>Obra Pública</div>
          </div>
        </div>

        {/* Nav tabs */}
        <nav style={{ display: "flex", alignItems: "stretch", height: "100%", flex: 1, overflow: "hidden" }}>
          {NAV.map(n => {
            const active = tab === n.id;
            return (
              <button key={n.id} onClick={() => setTab(n.id)} style={{
                display: "flex", alignItems: "center", gap: 7,
                padding: "0 16px", height: "100%",
                background: "none", border: "none",
                borderBottom: `2px solid ${active ? C.blue : "transparent"}`,
                color: active ? C.blue : C.fgMuted,
                fontWeight: active ? 600 : 400, fontSize: 13,
                cursor: "pointer", whiteSpace: "nowrap",
                transition: "color 0.15s, border-color 0.15s",
              }}
                onMouseEnter={el => { if (!active) (el.currentTarget as HTMLButtonElement).style.color = C.fg; }}
                onMouseLeave={el => { if (!active) (el.currentTarget as HTMLButtonElement).style.color = C.fgMuted; }}
              >
                {n.icon}
                {n.label}
              </button>
            );
          })}
        </nav>

        {/* Right actions */}
        <div style={{ display: "flex", alignItems: "center", gap: 8, padding: "0 16px", borderLeft: `1px solid ${C.border}`, height: "100%", flexShrink: 0 }}>
          <div style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: C.fgMuted, background: C.surface2, borderRadius: 7, padding: "4px 9px", maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
            CT-2026-014
          </div>

          {/* Bell */}
          <button onClick={() => setAlertOpen(v => !v)} style={{
            position: "relative", display: "flex", alignItems: "center", justifyContent: "center",
            width: 34, height: 34, borderRadius: 10,
            background: alertOpen ? C.redSoft : C.surface2,
            border: `1px solid ${alertOpen ? C.red + "55" : C.border}`,
            color: alertOpen ? C.red : C.fgMuted, cursor: "pointer", flexShrink: 0,
            transition: "background 0.15s",
          }}>
            <Bell size={15} />
            {alertCount > 0 && (
              <span style={{ position: "absolute", top: 5, right: 5, width: 7, height: 7, background: C.red, borderRadius: "50%", border: `1.5px solid ${C.surface}` }} />
            )}
          </button>

          {/* User chip */}
          <div style={{ display: "flex", alignItems: "center", gap: 8, background: C.surface2, borderRadius: 10, padding: "5px 10px 5px 6px", border: `1px solid ${C.border}` }}>
            <div style={{ width: 24, height: 24, borderRadius: "50%", background: `linear-gradient(135deg, ${C.purple}, ${C.blue})`, display: "flex", alignItems: "center", justifyContent: "center", fontSize: 9, fontWeight: 700, color: "#fff", flexShrink: 0 }}>RD</div>
            <div style={{ lineHeight: 1 }}>
              <div style={{ fontSize: 12, fontWeight: 600, color: C.fg, whiteSpace: "nowrap" }}>Ing. R. Domínguez</div>
              <div style={{ fontSize: 10, color: C.fgMuted, marginTop: 1 }}>Superintendente</div>
            </div>
          </div>
        </div>
      </header>

      {/* ── Body: main + alert panel side by side ── */}
      <div style={{ display: "flex", flex: 1, overflow: "hidden" }}>

        {/* Main content */}
        <main style={{ flex: 1, minWidth: 0, overflowY: "auto", padding: "28px 28px 48px" }}>
          {/* Page title */}
          <div style={{ display: "flex", alignItems: "center", gap: 6, fontSize: 12, color: C.fgMuted, marginBottom: 6 }}>
            <span style={{ fontFamily: "JetBrains Mono", color: C.blue, fontSize: 11 }}>CT-2026-014</span>
            <ChevronRight size={11} />
            <span>Pavimentación Av. Reforma Norte</span>
            <ChevronRight size={11} />
            <span style={{ color: C.fg, fontWeight: 500 }}>{NAV.find(n => n.id === tab)?.label}</span>
          </div>
          <h1 style={{ fontSize: 22, fontWeight: 700, color: C.fg, marginBottom: 24, lineHeight: 1 }}>
            {NAV.find(n => n.id === tab)?.label}
          </h1>

          {tab === "dashboard"    && <TabDashboard />}
          {tab === "estimaciones" && <TabEstimaciones />}
          {tab === "avance"       && <TabAvance />}
          {tab === "bitacora"     && <TabBitacora />}
          {tab === "convenios"    && <TabConvenios />}
          {tab === "expediente"   && <TabExpediente />}
        </main>

        {/* ── Alert panel — pushes layout, no overlay ── */}
        {alertOpen && (
          <aside style={{
            width: 300, flexShrink: 0,
            borderLeft: `1px solid ${C.border}`,
            background: C.surface,
            overflowY: "auto",
            padding: "20px 16px",
          }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 18 }}>
              <div style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 13, fontWeight: 700, color: C.fg }}>
                <Bell size={14} color={C.red} />
                Alertas activas
                <span style={{ background: C.red, color: "#fff", fontSize: 10, fontWeight: 700, borderRadius: 999, padding: "2px 7px" }}>{alertCount}</span>
              </div>
              <button onClick={() => setAlertOpen(false)} style={{ background: C.surface2, border: "none", borderRadius: 8, padding: 6, cursor: "pointer", color: C.fgMuted, display: "flex" }}>
                <X size={14} />
              </button>
            </div>
            <AlertsPanel />
          </aside>
        )}
      </div>
    </div>
  );
}
