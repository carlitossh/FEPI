import { useState } from "react";
import { Upload, CheckCircle } from "lucide-react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { mockContrato } from "../../dashboard/mock/mockContrato";
import {
  obra, obraSoft, rule, paper2, aprobado, aprobadoSoft,
  folio, folioSoft, observado, observadoSoft, muted,
} from "../../../styles/theme";

interface ModalNuevoConvenioProps {
  onClose: () => void;
  onCrear: (data: { tipo: string; monto: string; justificacion: string; doc: string | null }) => void;
}

export function ModalNuevoConvenio({ onClose, onCrear }: ModalNuevoConvenioProps) {
  const [form, setForm] = useState({
    tipo: "Ampliación de plazo",
    monto: "",
    justificacion: "",
    doc: null as string | null,
  });

  const tipos = [
    "Incremento de monto",
    "Ampliación de plazo",
    "Ajuste de catálogo",
    "Reducción de monto",
  ];

  const completo = form.justificacion.trim() && form.doc;

  const variacionActual = 18;
  const variacionNueva = form.monto
    ? variacionActual + (parseFloat(form.monto) / mockContrato.monto) * 100
    : variacionActual;

  return (
    <Modal
      title="Nueva solicitud de convenio"
      subtitle="HU-15 · Superintendente"
      onClose={onClose}
      width={500}
    >
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo de modificación *</SectionLabel>
        {tipos.map((t) => (
          <div
            key={t}
            onClick={() => setForm((p) => ({ ...p, tipo: t }))}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 10,
              padding: "8px 12px",
              border: `1.5px solid ${form.tipo === t ? obra : rule}`,
              borderRadius: 3,
              marginBottom: 6,
              cursor: "pointer",
              background: form.tipo === t ? obraSoft : paper2,
            }}
          >
            <div
              style={{
                width: 14,
                height: 14,
                borderRadius: "50%",
                border: `2px solid ${form.tipo === t ? obra : rule}`,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              {form.tipo === t && (
                <div style={{ width: 7, height: 7, borderRadius: "50%", background: obra }} />
              )}
            </div>
            <span style={{ fontSize: 12.5 }}>{t}</span>
          </div>
        ))}
      </div>

      {(form.tipo === "Incremento de monto" || form.tipo === "Reducción de monto") && (
        <div style={{ marginBottom: 14 }}>
          <SectionLabel>Monto de variación</SectionLabel>
          <TextInput
            placeholder="Ej: 500000"
            value={form.monto}
            onChange={(v) => setForm((p) => ({ ...p, monto: v }))}
          />
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              marginTop: 8,
              padding: "8px 12px",
              background: variacionNueva > 25 ? folioSoft : observadoSoft,
              borderRadius: 3,
            }}
          >
            <span style={{ fontSize: 12, color: muted }}>Variación acumulada proyectada</span>
            <span
              style={{
                fontFamily: "JetBrains Mono",
                fontSize: 12,
                fontWeight: 700,
                color: variacionNueva > 25 ? folio : observado,
              }}
            >
              {variacionNueva.toFixed(1)}% / 25% límite
            </span>
          </div>
        </div>
      )}

      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Justificación *</SectionLabel>
        <TextArea
          placeholder="Justifica la necesidad del convenio..."
          value={form.justificacion}
          onChange={(v) => setForm((p) => ({ ...p, justificacion: v }))}
          rows={4}
        />
      </div>

      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Documentación soporte (requerida) *</SectionLabel>
        <div
          style={{
            border: `2px dashed ${form.doc ? aprobado : rule}`,
            borderRadius: 4,
            padding: 16,
            textAlign: "center",
            cursor: "pointer",
            background: form.doc ? aprobadoSoft : "#FAF8F2",
          }}
          onClick={() => {
            const input = document.createElement("input");
            input.type = "file";
            input.multiple = true;
            input.onchange = (e) => {
              const target = e.target as HTMLInputElement;
              setForm((p) => ({ ...p, doc: target.files?.[0]?.name || "doc.pdf" }));
            };
            input.click();
          }}
        >
          {form.doc ? (
            <div style={{ color: aprobado, fontWeight: 600 }}>
              <CheckCircle size={16} /> {form.doc}
            </div>
          ) : (
            <div style={{ color: muted }}>
              <Upload size={20} />
              <br />
              <span style={{ fontSize: 12 }}>Adjuntar documentación soporte</span>
            </div>
          )}
        </div>
      </div>

      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn onClick={() => onCrear(form)} disabled={!completo} style={{ flex: 1 }}>
          Enviar solicitud
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
