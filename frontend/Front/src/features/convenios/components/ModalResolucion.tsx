import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextArea } from "../../../components/TextArea";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { aprobado, aprobadoSoft, folio, folioSoft, rule, paper2, ink } from "../../../styles/theme";

interface ModalResolucionProps {
  onClose: () => void;
  onGuardar: (data: { resolucion: string; motivo: string }) => void;
}

export function ModalResolucion({ onClose, onGuardar }: ModalResolucionProps) {
  const [form, setForm] = useState({ resolucion: "Aprobado", motivo: "" });
  const requiereMotivo = form.resolucion === "Rechazado";

  return (
    <Modal
      title="Resolución institucional"
      subtitle="HU-18 · Dependencia"
      onClose={onClose}
      width={440}
    >
      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Resolución *</SectionLabel>
        <div style={{ display: "flex", gap: 10 }}>
          {["Aprobado", "Rechazado"].map((r) => (
            <div
              key={r}
              onClick={() => setForm((prev) => ({ ...prev, resolucion: r }))}
              style={{
                flex: 1,
                padding: "12px",
                border: `2px solid ${
                  form.resolucion === r
                    ? r === "Aprobado"
                      ? aprobado
                      : folio
                    : rule
                }`,
                borderRadius: 4,
                cursor: "pointer",
                textAlign: "center",
                background:
                  form.resolucion === r
                    ? r === "Aprobado"
                      ? aprobadoSoft
                      : folioSoft
                    : paper2,
                color:
                  form.resolucion === r
                    ? r === "Aprobado"
                      ? aprobado
                      : folio
                    : ink,
                fontWeight: form.resolucion === r ? 700 : 400,
                fontSize: 13,
              }}
            >
              {r}
            </div>
          ))}
        </div>
      </div>
      {requiereMotivo && (
        <div style={{ marginBottom: 14 }}>
          <SectionLabel>Motivo de rechazo *</SectionLabel>
          <TextArea
            placeholder="Indica el motivo de la resolución de rechazo..."
            value={form.motivo}
            onChange={(v) => setForm((p) => ({ ...p, motivo: v }))}
            rows={3}
          />
        </div>
      )}
      {form.resolucion === "Aprobado" && (
        <div
          style={{
            background: aprobadoSoft,
            border: `1px solid ${aprobado}`,
            borderRadius: 3,
            padding: "10px 14px",
            marginBottom: 16,
            fontSize: 12.5,
            color: aprobado,
          }}
        >
          Al aprobar, el sistema actualizará automáticamente el catálogo de conceptos, el programa
          de obra y el monto contratado.
        </div>
      )}
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => onGuardar(form)}
          disabled={requiereMotivo && !form.motivo.trim()}
          style={{
            flex: 1,
            background: form.resolucion === "Aprobado" ? aprobado : folio,
          }}
        >
          Confirmar resolución
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
