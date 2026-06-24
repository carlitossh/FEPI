import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";

interface ModalMinutaProps {
  onClose: () => void;
  onGuardar: (data: { fecha: string; participantes: string; acuerdos: string }) => void;
}

export function ModalMinuta({ onClose, onGuardar }: ModalMinutaProps) {
  const [form, setForm] = useState({ fecha: "", participantes: "", acuerdos: "" });
  const completo = Object.values(form).every((v) => v.trim());

  return (
    <Modal title="Nueva minuta de reunión" onClose={onClose} width={460}>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Fecha de la reunión *</SectionLabel>
        <TextInput
          placeholder="DD-MM-YYYY"
          value={form.fecha}
          onChange={(v) => setForm((p) => ({ ...p, fecha: v }))}
        />
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Participantes *</SectionLabel>
        <TextInput
          placeholder="Ej: Residente, Supervisor, Superintendente"
          value={form.participantes}
          onChange={(v) => setForm((p) => ({ ...p, participantes: v }))}
        />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Acuerdos y contenido *</SectionLabel>
        <TextArea
          placeholder="Describe los acuerdos de la reunión..."
          value={form.acuerdos}
          onChange={(v) => setForm((p) => ({ ...p, acuerdos: v }))}
          rows={4}
        />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn onClick={() => onGuardar(form)} disabled={!completo} style={{ flex: 1 }}>
          Registrar minuta
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
