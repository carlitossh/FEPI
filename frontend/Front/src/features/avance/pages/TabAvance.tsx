import { useState } from "react";
import { Camera, CheckCircle } from "lucide-react";
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { Modal } from "../../../components/Modal";
import { AlertsPanel } from "../../../components/AlertsPanel";
import { mockCurvaS } from "../../dashboard/mock/mockCurvaS";
import {
  obra, folio, rule, muted, observado, aprobado, aprobadoSoft, paper2,
} from "../../../styles/theme";

interface TabAvanceProps {
  rol: string;
}

export function TabAvance({ rol }: TabAvanceProps) {
  const [showRegistro, setShowRegistro] = useState(false);
  const [avance, setAvance] = useState({ descripcion: "", porcentaje: "", foto: null as string | null });
  const puedeRegistrar = rol === "Residente" || rol === "Supervisor";

  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 320px", gap: 24 }}>
      <div>
        <Card style={{ padding: "20px 22px", marginBottom: 20 }}>
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              marginBottom: 16,
            }}
          >
            <div
              style={{
                fontFamily: "'IBM Plex Serif', serif",
                fontWeight: 600,
                fontSize: 15,
              }}
            >
              Curva S — Avance físico vs. programado
            </div>
            {puedeRegistrar && (
              <PrimaryBtn onClick={() => setShowRegistro(true)}>+ Registrar avance</PrimaryBtn>
            )}
          </div>
          <ResponsiveContainer width="100%" height={260}>
            <LineChart data={mockCurvaS} margin={{ top: 4, right: 8, bottom: 0, left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={rule} />
              <XAxis
                dataKey="mes"
                tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }}
              />
              <YAxis
                tick={{ fontSize: 11, fill: muted, fontFamily: "JetBrains Mono" }}
                unit="%"
                domain={[0, 100]}
              />
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
                strokeWidth={2.5}
                dot={{ fill: obra, r: 3 }}
              />
              <Line
                dataKey="real"
                name="Real"
                stroke={folio}
                strokeWidth={2.5}
                strokeDasharray="5 4"
                dot={{ fill: folio, r: 3 }}
                connectNulls={false}
              />
            </LineChart>
          </ResponsiveContainer>
        </Card>

        <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 14 }}>
          {[
            { label: "Avance físico actual", value: "63%", note: "Programado: 68%", color: observado },
            { label: "Desviación del periodo", value: "-5 pp", note: "8% de atraso relativo", color: folio },
            { label: "Periodo activo", value: "Jun 2026", note: "Mes 4 de 7", color: obra },
          ].map((k) => (
            <Card key={k.label} style={{ padding: "16px 18px" }}>
              <div
                style={{
                  fontFamily: "'IBM Plex Serif', serif",
                  fontSize: 24,
                  fontWeight: 700,
                  color: k.color,
                }}
              >
                {k.value}
              </div>
              <div
                style={{
                  fontSize: 10,
                  textTransform: "uppercase",
                  letterSpacing: "0.05em",
                  color: muted,
                  marginTop: 4,
                }}
              >
                {k.label}
              </div>
              <div style={{ fontSize: 11, color: muted, marginTop: 6 }}>{k.note}</div>
            </Card>
          ))}
        </div>
      </div>

      <AlertsPanel />

      {showRegistro && (
        <Modal
          title="Registrar avance físico"
          subtitle="HU-07 · Avance diario"
          onClose={() => setShowRegistro(false)}
          width={440}
        >
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Descripción de actividades</SectionLabel>
            <TextArea
              placeholder="Describe las actividades ejecutadas hoy..."
              value={avance.descripcion}
              onChange={(v) => setAvance((p) => ({ ...p, descripcion: v }))}
              rows={3}
            />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Porcentaje de avance acumulado (%)</SectionLabel>
            <TextInput
              placeholder="Ej: 65"
              value={avance.porcentaje}
              onChange={(v) => setAvance((p) => ({ ...p, porcentaje: v }))}
            />
          </div>
          <div style={{ marginBottom: 20 }}>
            <SectionLabel>Fotografía de evidencia (requerida)</SectionLabel>
            <div
              style={{
                border: `2px dashed ${avance.foto ? aprobado : rule}`,
                borderRadius: 4,
                padding: "20px",
                textAlign: "center",
                cursor: "pointer",
                background: avance.foto ? aprobadoSoft : "#FAF8F2",
              }}
              onClick={() => {
                const input = document.createElement("input");
                input.type = "file";
                input.accept = "image/*";
                input.onchange = (e) => {
                  const target = e.target as HTMLInputElement;
                  setAvance((p) => ({ ...p, foto: target.files?.[0]?.name || "foto.jpg" }));
                };
                input.click();
              }}
            >
              {avance.foto ? (
                <div style={{ color: aprobado, fontSize: 13, fontWeight: 600 }}>
                  <CheckCircle size={20} style={{ marginBottom: 4 }} />
                  <br />
                  {avance.foto}
                </div>
              ) : (
                <div style={{ color: muted }}>
                  <Camera size={24} style={{ marginBottom: 8 }} />
                  <br />
                  <span style={{ fontSize: 12 }}>Toca para adjuntar fotografía</span>
                </div>
              )}
            </div>
          </div>
          <div style={{ display: "flex", gap: 10 }}>
            <SecondaryBtn onClick={() => setShowRegistro(false)} style={{ flex: 1 }}>
              Cancelar
            </SecondaryBtn>
            <PrimaryBtn
              onClick={() => setShowRegistro(false)}
              disabled={!avance.descripcion || !avance.porcentaje || !avance.foto}
              style={{ flex: 1 }}
            >
              Guardar registro
            </PrimaryBtn>
          </div>
        </Modal>
      )}
    </div>
  );
}
