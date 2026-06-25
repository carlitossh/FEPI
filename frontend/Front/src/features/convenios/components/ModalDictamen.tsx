import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { aprobado, aprobadoSoft, folio, folioSoft, rule } from "../../../styles/theme";
import { C } from "../../../styles/theme";

interface ModalDictamenProps {
  onClose: () => void;
  onGuardar: (data: { procedencia: string; justificacion: string }) => void;
}

export function ModalDictamen({ onClose, onGuardar }: ModalDictamenProps) {
  const [form, setForm] = useState({ procedencia: "Procedente", justificacion: "" });

  return (
    <Modal
      title="Emitir dictamen técnico"
      subtitle="HU-16 · Supervisor externo"
      onClose={onClose}
      width={440}
    >
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Procedencia *</SectionLabel>
        <div style={{ display: "flex", gap: 10 }}>
          {["Procedente", "No procedente"].map((p) => (
            <div
              key={p}
              onClick={() => setForm((prev) => ({ ...prev, procedencia: p }))}
              style={{
                flex: 1,
                padding: "12px",
                border: `2px solid ${
                  form.procedencia === p
                    ? p === "Procedente"
                      ? aprobado
                      : folio
                    : rule
                }`,
                borderRadius: 4,
                cursor: "pointer",
                textAlign: "center",
                background:
                  form.procedencia === p
                    ? p === "Procedente"
                      ? aprobadoSoft
                      : folioSoft
                    : C.surface2,
                color:
                  form.procedencia === p
                    ? p === "Procedente"
                      ? aprobado
                      : folio
                    : C.fg,
                fontWeight: form.procedencia === p ? 700 : 400,
                fontSize: 13,
              }}
            >
              {p}
            </div>
          ))}
        </div>
      </div>
      <div style={{ marginBottom: 20 }}>
        <SectionLabel>Justificación escrita *</SectionLabel>
        <TextArea
          placeholder="Fundamenta tu dictamen..."
          value={form.justificacion}
          onChange={(v) => setForm((p) => ({ ...p, justificacion: v }))}
          rows={4}
        />
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => onGuardar(form)}
          disabled={!form.justificacion.trim()}
          style={{
            flex: 1,
            background: form.procedencia === "Procedente" ? aprobado : folio,
          }}
        >
          Confirmar dictamen
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
