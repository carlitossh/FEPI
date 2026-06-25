import { useState, useEffect } from "react";
import { Modal } from "../../../components/Modal";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  obra,
  obraSoft,
  rule,
  muted,
} from "../../../styles/theme";
import { C } from "../../../styles/theme";
import { estimacionesService } from "../services/estimacionesService";

interface ModalAddConceptoProps {
  contratoId: number;
  onClose: () => void;
  onAdd: (concepto: any) => void;
}

export function ModalAddConcepto({
  contratoId,
  onClose,
  onAdd,
}: ModalAddConceptoProps) {
  const [conceptos, setConceptos] = useState<any[]>([]);
  const [selected, setSelected] = useState<any | null>(null);
  const [cant, setCant] = useState("");

  useEffect(() => {
    async function cargar() {
      const data = await estimacionesService.getConceptosContrato(
        contratoId
      );

      setConceptos(data);
    }

    cargar();
  }, [contratoId]);

  return (
    <Modal
      title="Añadir concepto del contrato"
      onClose={onClose}
      width={520}
    >
      <div style={{ marginBottom: 16 }}>
        <SectionLabel>Selecciona un concepto</SectionLabel>

        {conceptos.map((c) => (
          <div
            key={c.id}
            onClick={() => setSelected(c)}
            style={{
              display: "flex",
              alignItems: "center",
              gap: 12,
              padding: "10px 14px",
              border: `1.5px solid ${
                selected?.id === c.id ? obra : rule
              }`,
              borderRadius: 3,
              marginBottom: 6,
              cursor: "pointer",
              background:
                selected?.id === c.id ? obraSoft : C.surface2,
            }}
          >
            <div
              style={{
                width: 16,
                height: 16,
                borderRadius: "50%",
                border: `2px solid ${
                  selected?.id === c.id ? obra : rule
                }`,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                flexShrink: 0,
              }}
            >
              {selected?.id === c.id && (
                <div
                  style={{
                    width: 8,
                    height: 8,
                    borderRadius: "50%",
                    background: obra,
                  }}
                />
              )}
            </div>

            <span
              style={{
                fontFamily: "JetBrains Mono",
                fontSize: 11.5,
                color: obra,
                minWidth: 70,
              }}
            >
              {c.clave}
            </span>

            <span
              style={{
                flex: 1,
                fontSize: 12.5,
              }}
            >
              {c.descripcion}
            </span>

            <span
              style={{
                fontFamily: "JetBrains Mono",
                fontSize: 11,
                color: muted,
              }}
            >
              {c.unidadMedida} · {fmtMXN(c.precioUnitario)}
            </span>
          </div>
        ))}
      </div>

      {selected && (
        <>
          <div style={{ marginBottom: 20 }}>
            <SectionLabel>
              Cantidad ejecutada ({selected.unidadMedida})
            </SectionLabel>

            <TextInput
              placeholder={`Cantidad en ${selected.unidadMedida}`}
              value={cant}
              onChange={setCant}
            />

            {cant && (
              <div
                style={{
                  marginTop: 8,
                  fontSize: 12,
                  color: muted,
                }}
              >
                Importe estimado:

                <strong>
                  {" "}
                  {fmtMXN(
                    Number(cant) *
                      selected.precioUnitario
                  )}
                </strong>
              </div>
            )}
          </div>
        </>
      )}

      <div
        style={{
          display: "flex",
          gap: 10,
        }}
      >
        <SecondaryBtn
          onClick={onClose}
          style={{ flex: 1 }}
        >
          Cancelar
        </SecondaryBtn>

        <PrimaryBtn
          style={{ flex: 1 }}
          disabled={!selected || !cant}
          onClick={() => {
            onAdd({
              conceptoContratoId: selected.id,
              clave: selected.clave,
              desc: selected.descripcion,
              unidad: selected.unidadMedida,
              precioUnitario: selected.precioUnitario,
              cantEjecutada: Number(cant),
              importe:
                Number(cant) *
                selected.precioUnitario,
            });
          }}
        >
          Añadir concepto
        </PrimaryBtn>
      </div>
    </Modal>
  );
}