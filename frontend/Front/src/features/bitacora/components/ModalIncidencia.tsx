import { useState } from "react";
import { Camera, CheckCircle } from "lucide-react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { aprobado, aprobadoSoft, rule, muted } from "../../../styles/theme";

interface ModalIncidenciaProps {
  onClose: () => void;
  onGuardar: (data: { fecha: string; descripcion: string; foto: string | null }) => void;
}

export function ModalIncidencia({ onClose, onGuardar }: ModalIncidenciaProps) {
  const [form, setForm] = useState({ fecha: "", descripcion: "", foto: null as string | null });
  const completo = form.fecha.trim() && form.descripcion.trim() && form.foto;

  return (
    <Modal title="Registrar incidencia" onClose={onClose} width={440}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Fecha *</SectionLabel>
        <TextInput
          placeholder="DD-MM-YYYY"
          value={form.fecha}
          onChange={(v) => setForm((p) => ({ ...p, fecha: v }))}
        />
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Descripción *</SectionLabel>
        <TextArea
          placeholder="Describe la incidencia..."
          value={form.descripcion}
          onChange={(v) => setForm((p) => ({ ...p, descripcion: v }))}
          rows={3}
        />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Fotografía (requerida) *</SectionLabel>
        <div
          style={{
            border: `2px dashed ${form.foto ? aprobado : rule}`,
            borderRadius: 4,
            padding: 20,
            textAlign: "center",
            cursor: "pointer",
            background: form.foto ? aprobadoSoft : "#FAF8F2",
          }}
          onClick={() => {
            const input = document.createElement("input");
            input.type = "file";
            input.accept = "image/*";
            input.onchange = (e) => {
              const target = e.target as HTMLInputElement;
              setForm((p) => ({ ...p, foto: target.files?.[0]?.name || "foto.jpg" }));
            };
            input.click();
          }}
        >
          {form.foto ? (
            <div style={{ color: aprobado, fontWeight: 600 }}>
              <CheckCircle size={18} />
              <br />
              {form.foto}
            </div>
          ) : (
            <div style={{ color: muted }}>
              <Camera size={22} />
              <br />
              <span style={{ fontSize: 12 }}>Adjuntar foto</span>
            </div>
          )}
        </div>
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!completo} style={{ flex: 1 }}>
          Registrar incidencia
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
