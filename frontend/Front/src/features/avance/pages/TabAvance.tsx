import { getCurvaS, registrarAvance, getConceptosContrato } from "../services/avanceService";
import { useEffect, useState } from "react";
import { Camera, CheckCircle } from "lucide-react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { Modal } from "../../../components/Modal";
import { AlertsPanel } from "../../../components/AlertsPanel";
import {
  obra,
  folio,
  rule,
  muted,
  observado,
  aprobado,
  aprobadoSoft,
  paper2,
} from "../../../styles/theme";

interface TabAvanceProps {
  rol: string;
}

export function TabAvance({ rol }: TabAvanceProps) {
  const [curvaS, setCurvaS] = useState<any[]>([]);
  const [showRegistro, setShowRegistro] = useState(false);
  const [conceptos, setConceptos] = useState<any[]>([]);
  const [conceptoSeleccionado, setConceptoSeleccionado] = useState<any>(null);
  const [avance, setAvance] = useState({
    descripcion: "",
    cantidadEjecutada: "",
    foto: null as string | null,
  });

  const puedeRegistrar = rol === "Residente" || rol === "Supervisor";

  async function cargarDatos() {
    const curva = await getCurvaS(1);
    setCurvaS(curva.puntos ?? []);

    const conceptosData = await getConceptosContrato(1);
    setConceptos(conceptosData ?? []);
    setConceptoSeleccionado(conceptosData?.[0] ?? null);
}

  useEffect(() => {
    cargarDatos();
  }, []);

  const curvaGrafica = curvaS.map((x) => ({
    mes: x.periodo.substring(5),
    programado: x.porcentajeProgramado,
    real: x.porcentajeReal,
  }));

  const ultimoPunto = curvaS[curvaS.length - 1];

  const avanceReal = ultimoPunto?.porcentajeReal ?? 0;
  const avanceProgramado = ultimoPunto?.porcentajeProgramado ?? 0;
  const desviacion = avanceReal - avanceProgramado;

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
              <PrimaryBtn onClick={() => setShowRegistro(true)}>
                + Registrar avance
              </PrimaryBtn>
            )}
          </div>

          <ResponsiveContainer width="100%" height={260}>
            <LineChart
              data={curvaGrafica}
              margin={{ top: 4, right: 8, bottom: 0, left: -20 }}
            >
              <CartesianGrid strokeDasharray="3 3" stroke={rule} />
              <XAxis
                dataKey="mes"
                tick={{
                  fontSize: 11,
                  fill: muted,
                  fontFamily: "JetBrains Mono",
                }}
              />
              <YAxis
                tick={{
                  fontSize: 11,
                  fill: muted,
                  fontFamily: "JetBrains Mono",
                }}
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
                formatter={(value) =>
                  value !== null ? [`${value}%`] : ["—"]
                }
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
            {
              label: "Avance físico actual",
              value: `${avanceReal.toFixed(2)}%`,
              note: `Programado: ${avanceProgramado.toFixed(2)}%`,
              color: observado,
            },
            {
              label: "Desviación del periodo",
              value: `${desviacion.toFixed(2)} pp`,
              note: desviacion < 0 ? "Atraso respecto al programa" : "Adelanto respecto al programa",
              color: desviacion < 0 ? folio : aprobado,
            },
            {
              label: "Periodo activo",
              value: ultimoPunto?.periodo ?? "—",
              note: "Según programa de obra",
              color: obra,
            },
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
              <div style={{ fontSize: 11, color: muted, marginTop: 6 }}>
                {k.note}
              </div>
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
              onChange={(v) =>
                setAvance((p) => ({ ...p, descripcion: v }))
              }
              rows={3}
            />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Concepto</SectionLabel>
            <select
              value={conceptoSeleccionado?.id ?? ""}
              onChange={(e) => {
              const seleccionado = conceptos.find(
                (c) => c.id === Number(e.target.value)
              );
              setConceptoSeleccionado(seleccionado ?? null);
              }}
              style={{
                width: "100%",
                padding: "10px",
                border: `1px solid ${rule}`,
                borderRadius: 4,
                background: "#FAF8F2",
              fontSize: 13,
              }}
            >
            {conceptos.map((c) => (
              <option key={c.id} value={c.id}>
              {c.clave} — {c.descripcion}
              </option>
              ))}
            </select>
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>
  Cantidad ejecutada hoy {conceptoSeleccionado ? `(${conceptoSeleccionado.unidadMedida})` : ""}
</SectionLabel>
            <TextInput
              placeholder="Ej: 25"
              value={avance.cantidadEjecutada}
              onChange={(v) =>
                setAvance((p) => ({ ...p, cantidadEjecutada: v }))
              }
            />
          </div>

          <div style={{ marginBottom: 20 }}>
            <SectionLabel>Fotografía de evidencia</SectionLabel>
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
                  setAvance((p) => ({
                    ...p,
                    foto: target.files?.[0]?.name || "foto.jpg",
                  }));
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
                  <span style={{ fontSize: 12 }}>
                    Toca para adjuntar fotografía
                  </span>
                </div>
              )}
            </div>
          </div>

          <div style={{ display: "flex", gap: 10 }}>
            <SecondaryBtn onClick={() => setShowRegistro(false)} style={{ flex: 1 }}>
              Cancelar
            </SecondaryBtn>

            <PrimaryBtn
              onClick={async () => {
                await registrarAvance({
                  contratoId: 1,
                  fecha: new Date().toISOString().substring(0, 10),
                  conceptoContratoId: conceptoSeleccionado?.id,
                  cantidadEjecutada: Number(avance.cantidadEjecutada),
                  descripcionActividad: avance.descripcion,
                  actorId: 1,
                  urlsEvidencia: avance.foto ? [avance.foto] : [],
                });

                await cargarDatos();

                setAvance({
                  descripcion: "",
                  cantidadEjecutada: "",
                  foto: null,
                });

                setShowRegistro(false);
              }}
disabled={
  !conceptoSeleccionado ||
  !avance.descripcion ||
  !avance.cantidadEjecutada ||
  !avance.foto
}
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