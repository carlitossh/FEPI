import { useEffect, useState } from "react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { fmtMXN } from "../../../imports/fmtMXN";
import { obra, folio, rule, muted, observado, aprobado, paper2 } from "../../../styles/theme";
import { getContrato, getCurvaS } from "../services/dashboardService";

export function TabDashboard() {
  const [contrato, setContrato] = useState<any>(null);
  const [curvaS, setCurvaS] = useState<any[]>([]);

  useEffect(() => {
    async function cargarDashboard() {
      const contratoData = await getContrato(1);
      setContrato(contratoData);

      const curvaData = await getCurvaS(1);
      setCurvaS(curvaData.puntos ?? []);
    }

    cargarDashboard();
  }, []);

  const totalContratado = contrato?.montoContratado ?? 0;
  const ejercido = 10_200_000;
  const pctFisico = 63;
  const pctProg = 68;

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
            { label: "Avance físico", value: `${pctFisico}%`, note: `Prog. ${pctProg}%`, color: obra },
            { label: "Monto ejercido", value: "$10.2M", note: `de ${fmtMXN(totalContratado)} contratados`, color: obra },
            { label: "Est. pendientes", value: "2", note: "014 vence pronto", color: observado },
            { label: "Garantías activas", value: "3", note: "1 por vencer", color: folio },
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
                width: `${totalContratado > 0 ? (ejercido / totalContratado) * 100 : 0}%`,
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
                  2
                </div>
                <div style={{ fontSize: 11, color: muted }}>pendientes de resolución</div>
              </div>
              <div style={{ fontSize: 12, color: muted, textAlign: "right" }}>
                <div>Pagadas: <strong style={{ color: aprobado }}>3</strong></div>
                <div>Aprobadas: <strong style={{ color: obra }}>1</strong></div>
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
                  1
                </div>
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