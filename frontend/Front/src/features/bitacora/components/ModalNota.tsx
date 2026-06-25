import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { obra, folio, rule } from "../../../styles/theme";
import { C } from "../../../styles/theme";

interface ModalNotaProps {
  tipo: string;
  onClose: () => void;
  siguienteFolio: string;
  onGuardar: (data: {
    folio: string;
    asunto: string;
    contenido: string;
    tipoNota: string;
    folioRef: string;
  }) => void;
}

export function ModalNota({ tipo, onClose, siguienteFolio, onGuardar }: ModalNotaProps) {
  const [form, setForm] = useState({
    folio: siguienteFolio,
    asunto: "",
    contenido: "",
    tipoNota: "Aviso al residente",
    folioRef: "",
  });

  const tipos = [
    "Aviso al residente",
    "Solicitud de convenio",
    "Cambio de procedimiento",
    "Concepto terminado",
    "Instrucción",
  ];

  const completo = form.asunto.trim() && form.contenido.trim();

  return (
    <Modal
      title={`Nueva ${tipo}`}
      subtitle={`FOLIO-${form.folio} (autoasignado)`}
      onClose={onClose}
      width={480}
    >
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Tipo</SectionLabel>
        <div style={{ display: "flex", gap: 6, flexWrap: "wrap" }}>
          {tipos.map((t) => (
            <span
              key={t}
              onClick={() => setForm((p) => ({ ...p, tipoNota: t }))}
              style={{
                fontSize: 11.5,
                border: `1px solid ${form.tipoNota === t ? obra : rule}`,
                color: form.tipoNota === t ? "#fff" : C.fgMuted,
                background: form.tipoNota === t ? obra : "transparent",
                padding: "5px 10px",
                borderRadius: 13,
                cursor: "pointer",
              }}
            >
              {t}
            </span>
          ))}
        </div>
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Asunto *</SectionLabel>
        <TextInput
          placeholder="Asunto de la nota"
          value={form.asunto}
          onChange={(v) => setForm((p) => ({ ...p, asunto: v }))}
        />
      </div>
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Contenido *</SectionLabel>
        <TextArea
          placeholder="Describe el contenido de la nota..."
          value={form.contenido}
          onChange={(v) => setForm((p) => ({ ...p, contenido: v }))}
          rows={4}
        />
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Folio de referencia (opcional)</SectionLabel>
        <TextInput
          placeholder="Ej: 0138"
          value={form.folioRef}
          onChange={(v) => setForm((p) => ({ ...p, folioRef: v }))}
        />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => onGuardar(form)}
          disabled={!completo}
          style={{ flex: 1, background: folio }}
        >
          Firmar y registrar nota
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
