import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { C } from "../../../styles/theme";
import { obraSoft, obra } from "../../../styles/theme";

interface ModalAperturaBitacoraProps {
  contrato: any;
  onClose: () => void;
  onAbrir: (fecha: string) => Promise<void>;
}

export function ModalAperturaBitacora({ contrato, onClose, onAbrir }: ModalAperturaBitacoraProps) {
  const [fecha, setFecha] = useState("");

  const resumen = [
    { label: "Número de contrato", value: contrato?.numeroContrato },
    { label: "Nombre de obra", value: contrato?.nombreObra },
    { label: "Dependencia contratante", value: contrato?.dependenciaContratante },
    { label: "Contratista", value: contrato?.contratistaEmpresa },
    { label: "Residente", value: contrato?.residenteNombre },
    { label: "Supervisor externo", value: contrato?.supervisorExternoNombre },
    { label: "Superintendente", value: contrato?.superintendenteNombre },
  ];

  return (
    <Modal
      title="Apertura de bitácora"
      subtitle="Se asignará folio único automático"
      onClose={onClose}
      width={500}
    >
      <div
        style={{
          background: C.surface2,
          border: `1px solid ${C.border}`,
          borderRadius: 10,
          padding: "12px 14px",
          marginBottom: 18,
        }}
      >
        {resumen.map((r) => (
          <div
            key={r.label}
            style={{
              display: "flex",
              justifyContent: "space-between",
              padding: "5px 0",
              borderBottom: `1px solid ${C.border}`,
              fontSize: 12.5,
            }}
          >
            <span style={{ color: C.fgMuted }}>{r.label}</span>
            <span style={{ color: C.fg, fontWeight: 500 }}>{r.value ?? "—"}</span>
          </div>
        ))}
      </div>

      <div style={{ marginBottom: 14 }}>
        <SectionLabel>Fecha de apertura formal *</SectionLabel>
        <TextInput
          placeholder="YYYY-MM-DD"
          value={fecha}
          onChange={setFecha}
        />
      </div>

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
        <strong>Nota de Apertura (Folio 0001)</strong> con los datos del contrato.
      </div>

      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => onAbrir(fecha)}
          disabled={!fecha.trim()}
          style={{ flex: 1 }}
        >
          Abrir bitácora
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
