import { useState } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { mockCatalogoConceptos } from "../mock/mockCatalogoConceptos";
import { fmtMXN } from "../../../imports/fmtMXN";
import { obra, obraSoft, rule, paper2, muted } from "../../../styles/theme";
import type { ConceptoCatalogo, ConceptoEstimacion } from "../types";

interface ModalAddConceptoProps {
  onClose: () => void;
  onAdd: (concepto: ConceptoEstimacion) => void;
}

export function ModalAddConcepto({ onClose, onAdd }: ModalAddConceptoProps) {
  const [selected, setSelected] = useState<ConceptoCatalogo | null>(null);
  const [cant, setCant] = useState("");

  return (
    <Modal title="Añadir concepto del catálogo" onClose={onClose} width={520}>
      <div style={{ marginBottom: 16 }}>
        <SectionLabel>Selecciona un concepto</SectionLabel>
        {mockCatalogoConceptos.map((c) => (
          <div
            key={c.clave}
            onClick={() => setSelected(c)}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 12,
              padding: "10px 14px",
              border: `1.5px solid ${selected?.clave === c.clave ? obra : rule}`,
              borderRadius: 3,
              marginBottom: 6,
              cursor: "pointer",
              background: selected?.clave === c.clave ? obraSoft : paper2,
            }}
          >
            <div
              style={{
                width: 16,
                height: 16,
                borderRadius: "50%",
                border: `2px solid ${selected?.clave === c.clave ? obra : rule}`,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                flexShrink: 0,
              }}
            >
              {selected?.clave === c.clave && (
                <div style={{ width: 8, height: 8, borderRadius: "50%", background: obra }} />
              )}
            </div>
            <span
              style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra, minWidth: 60 }}
            >
              {c.clave}
            </span>
            <span style={{ fontSize: 12.5, flex: 1 }}>{c.desc}</span>
            <span style={{ fontFamily: "JetBrains Mono", fontSize: 11, color: muted }}>
              {c.unidad} · {fmtMXN(c.precioUnitario)}
            </span>
          </div>
        ))}
      </div>
      {selected && (
        <div style={{ marginBottom: 20 }}>
          <SectionLabel>
            Cantidad ejecutada en este periodo ({selected.unidad})
          </SectionLabel>
          <TextInput
            placeholder={`Cantidad en ${selected.unidad}`}
            value={cant}
            onChange={setCant}
          />
          {cant && (
            <div style={{ fontSize: 12, color: muted, marginTop: 6 }}>
              Importe estimado:{" "}
              <strong>{fmtMXN(parseFloat(cant) * selected.precioUnitario)}</strong>
            </div>
          )}
        </div>
      )}
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn
          onClick={() => {
            if (!selected || !cant) return;
            onAdd({
              ...selected,
              cantEjecutada: parseFloat(cant),
              importe: parseFloat(cant) * selected.precioUnitario,
            });
          }}
          disabled={!selected || !cant}
          style={{ flex: 1 }}
        >
          Añadir concepto
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
