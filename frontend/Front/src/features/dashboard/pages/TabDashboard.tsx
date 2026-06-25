import { useEffect, useState } from "react";
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { fmtMXN } from "../../../imports/fmtMXN";
import { C } from "../../../styles/theme";
import { getDashboard, getCurvaS } from "../services/dashboardService";

interface TabDashboardProps {
  rol: string;
}

export function TabDashboard({ rol }: TabDashboardProps) {
  const [dash, setDash] = useState<any>(null);
  const [curvaS, setCurvaS] = useState<any[]>([]);

  useEffect(() => {
    getDashboard(1).then(setDash).catch(() => {});
    getCurvaS(1).then((d) => setCurvaS(d.puntos ?? [])).catch(() => {});
  }, []);

  const totalContratado = dash?.montoContratado ?? 0;
  const ejercido        = dash?.montoEjercido ?? 0;
  const montoPagado     = dash?.montoPagadoTotal ?? 0;
  const pctFisico       = dash?.avanceFisicoPct ?? 0;
  const pctProg         = dash?.avanceProgramadoPct ?? 0;
  const estPendientes   = dash?.estimacionesPendientes ?? 0;
  const conveniosActivos    = dash?.conveniosActivos ?? 0;
  const garantiasPorVencer  = dash?.garantiasPorVencer ?? 0;

  const desviacion   = pctFisico - pctProg;
  const absDesviacion = Math.abs(desviacion);
  const desvPequena  = absDesviacion < 5;
  const colorDesv    = desvPequena ? C.amber : desviacion < 0 ? C.red : C.green;
  const labelDesv    = desviacion < 0 ? "Atraso" : desviacion > 0 ? "Adelanto" : "Al día";

  const curvaGrafica = curvaS.map((x) => ({
    mes: x.periodo.substring(5),
    programado: x.porcentajeProgramado,
    real: x.porcentajeReal,
  }));

  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 300px", gap: 24 }}>
      <div>
        {/* KPI row */}
        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 12, marginBottom: 20 }}>
          {/* Avance físico */}
          <Card style={{ padding: "18px 20px" }}>
            <div style={{ fontSize: 28, fontWeight: 700, color: C.blue, lineHeight: 1 }}>
              {pctFisico.toFixed(1)}%
            </div>
            <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 8, fontWeight: 500 }}>Avance físico</div>
            <div style={{ fontSize: 11, color: C.fgSub, marginTop: 4 }}>Prog.: {pctProg.toFixed(1)}%</div>
          </Card>

          {/* Desviación */}
          <Card style={{ padding: "18px 20px" }}>
            <div style={{ fontSize: 28, fontWeight: 700, color: colorDesv, lineHeight: 1 }}>
              {absDesviacion.toFixed(1)} pp
            </div>
            <div style={{ fontSize: 11.5, color: colorDesv, marginTop: 8, fontWeight: 700 }}>{labelDesv}</div>
            <div style={{ fontSize: 11, color: C.fgSub, marginTop: 4 }}>
              {desvPequena ? "Variación menor a 5 pp" : "Respecto a lo programado"}
            </div>
          </Card>

          {/* Estimaciones pendientes */}
          <Card style={{ padding: "18px 20px" }}>
            <div style={{ fontSize: 28, fontWeight: 700, color: C.amber, lineHeight: 1 }}>
              {estPendientes}
            </div>
            <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 8, fontWeight: 500 }}>Est. pendientes</div>
            <div style={{ fontSize: 11, color: C.fgSub, marginTop: 4 }}>sin resolver</div>
          </Card>

          {/* Garantías por vencer */}
          <Card style={{ padding: "18px 20px" }}>
            <div style={{ fontSize: 28, fontWeight: 700, color: C.red, lineHeight: 1 }}>
              {garantiasPorVencer}
            </div>
            <div style={{ fontSize: 11.5, color: C.fgMuted, marginTop: 8, fontWeight: 500 }}>Garantías por vencer</div>
            <div style={{ fontSize: 11, color: C.fgSub, marginTop: 4 }}>{conveniosActivos} convenio(s) activo(s)</div>
          </Card>
        </div>

        {/* Curva S */}
        <Card style={{ marginBottom: 20 }}>
          <div style={{ padding: "20px 22px 10px" }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 16 }}>
              <div style={{ fontSize: 14, fontWeight: 600, color: C.fg }}>Curva S — Avance físico</div>
              <div style={{ display: "flex", gap: 16, fontSize: 11.5 }}>
                <span style={{ display: "flex", alignItems: "center", gap: 5, color: C.fgMuted }}>
                  <span style={{ width: 20, height: 2, background: C.blue, display: "inline-block", borderRadius: 2 }} />
                  Programado
                </span>
                <span style={{ display: "flex", alignItems: "center", gap: 5, color: C.fgMuted }}>
                  <span style={{ width: 20, height: 2, background: C.red, display: "inline-block", borderRadius: 2 }} />
                  Real
                </span>
              </div>
            </div>

            <ResponsiveContainer width="100%" height={200}>
              <AreaChart data={curvaGrafica} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
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
                  formatter={(value) => (value !== null ? [`${value}%`] : ["—"])}
                />
                <Area dataKey="programado" name="Programado" stroke={C.blue} strokeWidth={2} fill="url(#gBlue)" dot={false} />
                <Area dataKey="real" name="Real" stroke={C.red} strokeWidth={2} fill="url(#gRed)" dot={false} connectNulls={false} />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </Card>

        {/* Avance financiero */}
        <Card style={{ padding: "20px 22px", marginBottom: 20 }}>
          <div style={{ fontSize: 14, fontWeight: 600, color: C.fg, marginBottom: 16 }}>Avance financiero</div>

          <div style={{ marginBottom: 14 }}>
            <div style={{ fontSize: 12, color: C.fgMuted, marginBottom: 6, display: "flex", justifyContent: "space-between" }}>
              <span>Aprobado (residencia)</span>
              <span style={{ fontFamily: "JetBrains Mono", color: C.blue, fontWeight: 600 }}>
                {fmtMXN(ejercido)} · {totalContratado > 0 ? Math.round((ejercido / totalContratado) * 100) : 0}%
              </span>
            </div>
            <div style={{ background: C.surface2, borderRadius: 999, height: 8, overflow: "hidden" }}>
              <div style={{
                background: `linear-gradient(90deg, ${C.blue}, #7ca8ff)`,
                height: "100%",
                width: `${totalContratado > 0 ? Math.min((ejercido / totalContratado) * 100, 100) : 0}%`,
                borderRadius: 999,
                transition: "width 0.3s ease",
              }} />
            </div>
          </div>

          <div style={{ marginBottom: 10 }}>
            <div style={{ fontSize: 12, color: C.fgMuted, marginBottom: 6, display: "flex", justifyContent: "space-between" }}>
              <span>Pagado</span>
              <span style={{ fontFamily: "JetBrains Mono", color: C.green, fontWeight: 600 }}>
                {fmtMXN(montoPagado)} · {totalContratado > 0 ? Math.round((montoPagado / totalContratado) * 100) : 0}%
              </span>
            </div>
            <div style={{ background: C.surface2, borderRadius: 999, height: 8, overflow: "hidden" }}>
              <div style={{
                background: `linear-gradient(90deg, ${C.green}, #4ade80)`,
                height: "100%",
                width: `${totalContratado > 0 ? Math.min((montoPagado / totalContratado) * 100, 100) : 0}%`,
                borderRadius: 999,
                transition: "width 0.3s ease",
              }} />
            </div>
          </div>

          <div style={{ fontSize: 11.5, color: C.fgSub, marginTop: 8 }}>
            Contrato total: {fmtMXN(totalContratado)}
          </div>
        </Card>
      </div>

      <AlertsPanel rol={rol} contratoId={1} />
    </div>
  );
}
