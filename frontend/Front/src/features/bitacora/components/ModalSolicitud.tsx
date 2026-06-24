import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { obra, obraSoft, rule, paper2 } from "../../../styles/theme";

interface ModalSolicitudProps {
  onClose: () => void;
  onGuardar: (data: { tipoSolicitud: string; asunto: string; descripcion: string }) => void;
}

export function ModalSolicitud({ onClose, onGuardar }: ModalSolicitudProps) {
  const [form, setForm] = useState({
    tipoSolicitud: "Solicitud de convenio",
    asunto: "",
    descripcion: "",
  });

  const tipos = [
    "Solicitud de modificación",
    "Cambio de procedimiento",
    "Solicitud de convenio",
    "Aviso de terminación",
  ];

  return (
    <Modal
      title="Solicitud / Aviso en bitácora"
      subtitle="HU-14 · Superintendente"
      onClose={onClose}
      width={460}
    >
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo *</SectionLabel>
        {tipos.map((t) => (
          <div
            key={t}
            onClick={() => setForm((p) => ({ ...p, tipoSolicitud: t }))}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 10,
              padding: "8px 12px",
              border: `1.5px solid ${form.tipoSolicitud === t ? obra : rule}`,
              borderRadius: 3,
              marginBottom: 6,
              cursor: "pointer",
              background: form.tipoSolicitud === t ? obraSoft : paper2,
            }}
          >
            <div
              style={{
                width: 14,
                height: 14,
                borderRadius: "50%",
                border: `2px solid ${form.tipoSolicitud === t ? obra : rule}`,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              {form.tipoSolicitud === t && (
                <div style={{ width: 7, height: 7, borderRadius: "50%", background: obra }} />
              )}
            </div>
            <span style={{ fontSize: 12.5 }}>{t}</span>
          </div>
        ))}
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Asunto *</SectionLabel>
        <TextInput
          placeholder="Asunto de la nota"
          value={form.asunto}
          onChange={(v) => setForm((p) => ({ ...p, asunto: v }))}
        />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Descripción *</SectionLabel>
        <TextArea
          placeholder="Describe el contenido..."
          value={form.descripcion}
          onChange={(v) => setForm((p) => ({ ...p, descripcion: v }))}
          rows={3}
        />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => onGuardar(form)}
          disabled={!form.asunto || !form.descripcion}
          style={{ flex: 1 }}
        >
          Registrar
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
