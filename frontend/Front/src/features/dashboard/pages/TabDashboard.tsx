import { useEffect, useState } from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { fmtMXN } from "../../../imports/fmtMXN";
import { obra, folio, rule, muted, observado, aprobado, paper2 } from "../../../styles/theme";
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
  const ejercido = dash?.montoEjercido ?? 0;
  const pctFisico = dash?.avanceFisicoPct ?? 0;
  const pctProg = dash?.avanceProgramadoPct ?? 0;
  const estPendientes = dash?.estimacionesPendientes ?? 0;
  const conveniosActivos = dash?.conveniosActivos ?? 0;
  const garantiasPorVencer = dash?.garantiasPorVencer ?? 0;

  const curvaGrafica = curvaS.map((x) => ({
    mes: x.periodo.substring(5),
    programado: x.porcentajeProgramado,
    real: x.porcentajeReal,
  }));

  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", gap: 24 }}>
      <div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 14, marginBottom: 24 }}>
          {[
            { label: "Avance físico", value: `${pctFisico.toFixed(1)}%`, note: `Prog. ${pctProg.toFixed(1)}%`, color: obra },
            { label: "Monto ejercido", value: fmtMXN(ejercido), note: `de ${fmtMXN(totalContratado)} contratados`, color: obra },
            { label: "Est. pendientes", value: String(estPendientes), note: "sin resolver", color: observado },
            { label: "Garantías por vencer", value: String(garantiasPorVencer), note: `${conveniosActivos} convenio(s) activo(s)`, color: folio },
          ].map((k) => (
            <Card key={k.label} style={{ padding: "14px 16px" }}>
              <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 22, fontWeight: 700, color: k.color }}>
                {k.value}
              </div>
              <div style={{ fontSize: 10, textTransform: "uppercase", letterSpacing: "0.05em", color: muted, marginTop: 2 }}>
                {k.label}
              </div>
              <div style={{ fontSize: 11, color: muted, marginTop: 6 }}>{k.note}</div>
            </Card>
          ))}
        </div>

        <Card style={{ padding: "18px 20px", marginBottom: 20 }}>
          <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 16 }}>
            Curva S — Avance físico
          </div>

          <ResponsiveContainer width="100%" height={200}>
            <LineChart data={curvaGrafica} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={rule} />
              <XAxis dataKey="mes" tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} />
              <YAxis tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }} unit="%" />
              <Tooltip
                contentStyle={{ background: paper2, border: `1px solid ${rule}`, borderRadius: 3, fontSize: 12 }}
                formatter={(value) => (value !== null ? [`${value}%`] : ["—"])}
              />
              <Legend iconSize={10} wrapperStyle={{ fontSize: 11 }} />
              <Line dataKey="programado" name="Programado" stroke={obra} strokeWidth={2} dot={false} />
              <Line dataKey="real" name="Real" stroke={folio} strokeWidth={2} strokeDasharray="5 4" dot={false} connectNulls={false} />
            </LineChart>
          </ResponsiveContainer>
        </Card>

        <Card style={{ padding: "16px 20px", marginBottom: 20 }}>
          <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 12 }}>
            Avance financiero
          </div>
          <div style={{ fontSize: 12, color: muted, marginBottom: 6, display: "flex", justifyContent: "space-between" }}>
            <span>{fmtMXN(ejercido)} ejercidos</span>
            <span style={{ fontFamily: "JetBrains Mono", color: obra, fontWeight: 600 }}>
              {totalContratado > 0 ? Math.round((ejercido / totalContratado) * 100) : 0}%
            </span>
          </div>
          <div style={{ background: rule, borderRadius: 2, height: 8, overflow: "hidden" }}>
            <div
              style={{
                background: obra,
                height: "100%",
                width: `${totalContratado > 0 ? Math.min((ejercido / totalContratado) * 100, 100) : 0}%`,
                borderRadius: 2,
              }}
            />
          </div>
          <div style={{ fontSize: 11, color: muted, marginTop: 8 }}>
            Contrato total: {fmtMXN(totalContratado)}
          </div>
        </Card>

        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14 }}>
          <Card style={{ padding: "14px 16px" }}>
            <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 10 }}>
              Estimaciones
            </div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <div>
                <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 28, fontWeight: 700, color: observado }}>
                  {estPendientes}
                </div>
                <div style={{ fontSize: 11, color: muted }}>pendientes de resolución</div>
              </div>
            </div>
          </Card>

          <Card style={{ padding: "14px 16px" }}>
            <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.07em", color: muted, marginBottom: 10 }}>
              Convenios activos
            </div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <div>
                <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 28, fontWeight: 700, color: folio }}>
                  {conveniosActivos}
                </div>
                <div style={{ fontSize: 11, color: muted }}>en proceso</div>
              </div>
              <div style={{ fontSize: 12, color: muted, textAlign: "right" }}>
                <div>Garantías por vencer: <strong style={{ color: aprobado }}>{garantiasPorVencer}</strong></div>
              </div>
            </div>
          </Card>
        </div>
      </div>

      <AlertsPanel rol={rol} contratoId={1} />
    </div>
  );
}
