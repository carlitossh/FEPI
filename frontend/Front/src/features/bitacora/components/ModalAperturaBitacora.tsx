import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { obraSoft, obra } from "../../../styles/theme";

interface ModalAperturaBitacoraProps {
  onClose: () => void;
  onAbrir: (data: { lugar: string; fecha: string; descripcion: string }) => void;
}

export function ModalAperturaBitacora({ onClose, onAbrir }: ModalAperturaBitacoraProps) {
  const [form, setForm] = useState({ lugar: "", fecha: "", descripcion: "" });

  const campos = [
    { key: "lugar" as const, label: "Lugar de obra", placeholder: "Ej: Av. Reforma Norte, Km 4+200" },
    { key: "fecha" as const, label: "Fecha de apertura", placeholder: "DD-MM-YYYY" },
    { key: "descripcion" as const, label: "Descripción del contrato", placeholder: "Descripción del objeto del contrato" },
  ];

  const completo = campos.every((c) => form[c.key].trim());

  return (
    <Modal
      title="Apertura de bitácora"
      subtitle="Se asignará folio único automático"
      onClose={onClose}
      width={460}
    >
      {campos.map((c) => (
        <div key={c.key} style={{ marginBottom: 14 }}>
          <SectionLabel>{c.label} *</SectionLabel>
          <TextInput
            placeholder={c.placeholder}
            value={form[c.key]}
            onChange={(v) => setForm((p) => ({ ...p, [c.key]: v }))}
          />
        </div>
      ))}
      <div
        style={{
          background: obraSoft,
          border: `1px solid ${obra}`,
          borderRadius: 3,
          padding: "10px 14px",
          fontSize: 12.5,
          marginBottom: 20,
        }}
      >
        Se generará automáticamente la{" "}
        <strong>Nota de Apertura (Folio 0001)</strong> con los datos capturados.
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn onClick={() => onAbrir(form)} disabled={!completo} style={{ flex: 1 }}>
          Abrir bitácora
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
