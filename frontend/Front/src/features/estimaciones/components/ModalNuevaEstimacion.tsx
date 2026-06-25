import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { obraSoft, obra } from "../../../styles/theme";

interface ModalNuevaEstimacionProps {
  contrato: {
    id: number;
    numeroContrato: string;
  };
  onClose: () => void;
  onCrear: (data: { periodo: string }) => void;
}

export function ModalNuevaEstimacion({
  contrato,
  onClose,
  onCrear,
}: ModalNuevaEstimacionProps) {
  const [periodo, setPeriodo] = useState("");

  return (
    <Modal
      title="Nueva estimación"
      subtitle="Se asignará número correlativo automático"
      onClose={onClose}
      width={420}
    >
      <div style={{ marginBottom: 16 }}>
        <SectionLabel>Periodo de la estimación</SectionLabel>
        <TextInput
          placeholder="Ej: 2026-07"
          value={periodo}
          onChange={setPeriodo}
        />
      </div>

      <div
        style={{
          background: obraSoft,
          border: `1px solid ${obra}`,
          borderRadius: 3,
          padding: "10px 14px",
          marginBottom: 20,
          fontSize: 12.5,
        }}
      >
        <strong>Datos de carátula precargados:</strong> Contrato{" "}
        {contrato.numeroContrato}
      </div>

      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>

        <PrimaryBtn
          onClick={() => onCrear({ periodo })}
          disabled={!periodo.trim()}
          style={{ flex: 1 }}
        >
          Crear borrador
        </PrimaryBtn>
      </div>
    </Modal>
  );
}