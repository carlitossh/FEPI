import { useState } from "react";
import { Check } from "lucide-react";
import { Modal } from "../../../components/Modal";
import { TextInput } from "../../../components/TextInput";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { obra, obraSoft, rule, paper2, muted } from "../../../styles/theme";
import type { NotaBitacora } from "../../bitacora/types";

interface ModalVincularBitacoraProps {
  notas: NotaBitacora[];
  vinculadas: string[];
  onClose: () => void;
  onVincular: (ids: number[]) => void;
}

export function ModalVincularBitacora({
  notas,
  vinculadas,
  onClose,
  onVincular,
}: ModalVincularBitacoraProps) {
  const [sel, setSel] = useState<number[]>([]);
  const [busqueda, setBusqueda] = useState("");
  const [filtroTipo, setFiltroTipo] = useState("Todos");

  const filtradas = notas
    .filter((n) => n.tipo !== "Incidencia" || n.tipo === "Incidencia")
    .filter((n) => filtroTipo === "Todos" || n.tipo === filtroTipo)
    .filter(
      (n) =>
        busqueda === "" ||
        n.asunto.toLowerCase().includes(busqueda.toLowerCase()) ||
        n.folio.includes(busqueda)
    );

  return (
    <Modal title="Vincular notas de bitácora" onClose={onClose} width={520}>
      <div style={{ display: "flex", gap: 8, marginBottom: 14 }}>
        <TextInput
          placeholder="Buscar asunto o folio..."
          value={busqueda}
          onChange={setBusqueda}
          style={{ flex: 1 }}
        />
        <select
          value={filtroTipo}
          onChange={(e) => setFiltroTipo(e.target.value)}
          style={{
            border: `1px solid ${rule}`,
            background: paper2,
            borderRadius: 3,
            padding: "8px 10px",
            fontSize: 12,
            fontFamily: "'IBM Plex Sans', sans-serif",
            outline: "none",
          }}
        >
          {["Todos", "Nota", "Apertura", "Incidencia"].map((o) => (
            <option key={o}>{o}</option>
          ))}
        </select>
      </div>
      <div style={{ maxHeight: 280, overflowY: "auto", marginBottom: 16 }}>
        {filtradas.map((n) => {
          const isChecked = sel.includes(n.id);
          return (
            <div
              key={n.id}
              onClick={() =>
                setSel((prev) =>
                  isChecked ? prev.filter((f) => f !== n.id) : [...prev, n.id]
                )
              }
              style={{
                display: "flex",
                alignItems: "center",
                gap: 12,
                padding: "9px 12px",
                border: `1px solid ${isChecked ? obra : rule}`,
                borderRadius: 3,
                marginBottom: 6,
                cursor: "pointer",
                background: isChecked ? obraSoft : paper2,
              }}
            >
              <div
                style={{
                  width: 16,
                  height: 16,
                  borderRadius: 3,
                  border: `2px solid ${isChecked ? obra : rule}`,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  flexShrink: 0,
                  background: isChecked ? obra : "transparent",
                }}
              >
                {isChecked && <Check size={10} color="#fff" />}
              </div>
              <span
                style={{
                  fontFamily: "JetBrains Mono",
                  fontSize: 11,
                  color: obra,
                  minWidth: 50,
                }}
              >
                {n.folio}
              </span>
              <span style={{ fontSize: 12 }}>{n.asunto}</span>
              <span style={{ fontSize: 11, color: muted, marginLeft: "auto" }}>
                {n.fecha}
              </span>
            </div>
          );
        })}
      </div>
      <div style={{ display: "flex", gap: 10 }}>
        <SecondaryBtn onClick={onClose} style={{ flex: 1 }}>
          Cancelar
        </SecondaryBtn>
        <PrimaryBtn onClick={() => onVincular(sel)} style={{ flex: 1 }}>
          Vincular selección
        </PrimaryBtn>
      </div>
    </Modal>
  );
}
