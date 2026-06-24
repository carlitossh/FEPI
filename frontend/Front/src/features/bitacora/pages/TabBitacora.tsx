import { useState } from "react";
import { Search } from "lucide-react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { SectionLabel } from "../../../components/SectionLabel";
import { Modal } from "../../../components/Modal";
import { ModalAperturaBitacora } from "../components/ModalAperturaBitacora";
import { ModalNota } from "../components/ModalNota";
import { ModalMinuta } from "../components/ModalMinuta";
import { ModalIncidencia } from "../components/ModalIncidencia";
import { ModalSolicitud } from "../components/ModalSolicitud";
import { mockBitacora } from "../mock/mockBitacora";
import { contarFirmas } from "../../../imports/contarFirmas";
import {
  ink, paper2, rule, obra, obraSoft, aprobado, aprobadoSoft,
  observado, observadoSoft, folio, muted,
} from "../../../styles/theme";
import type { NotaBitacora } from "../types";

interface TabBitacoraProps {
  rol: string;
}

export function TabBitacora({ rol }: TabBitacoraProps) {
  const [notas, setNotas] = useState<NotaBitacora[]>(mockBitacora);
  const [modalTipo, setModalTipo] = useState<string | null>(null);
  const [busqueda, setBusqueda] = useState("");
  const [filtroTipo, setFiltroTipo] = useState("Todos");
  const [abierta, setAbierta] = useState(true);
  const [showApertura, setShowApertura] = useState(false);
  const [detalle, setDetalle] = useState<NotaBitacora | null>(null);

  const puedeCrearNota = ["Residente", "Supervisor", "Superintendente"].includes(rol);
  const puedeCrearMinuta = rol === "Supervisor";
  const puedeCrearIncidencia = rol === "Supervisor";
  const puedeRegistrarSolicitudes = rol === "Superintendente";

  const filtradas = notas
    .filter((n) => filtroTipo === "Todos" || n.tipo === filtroTipo)
    .filter(
      (n) =>
        busqueda === "" ||
        n.asunto.toLowerCase().includes(busqueda.toLowerCase()) ||
        n.folio.includes(busqueda)
    );

  function handleFirmar(nota: NotaBitacora, actor: string) {
    // api.firmarNota(nota.id, actor)
    setNotas((prev) =>
      prev.map((n) =>
        n.id === nota.id
          ? { ...n, firmas: { ...n.firmas, [actor.toLowerCase()]: true } }
          : n
      )
    );
    setDetalle((prev) =>
      prev
        ? { ...prev, firmas: { ...prev.firmas, [actor.toLowerCase()]: true } }
        : null
    );
  }

  const actorFirma =
    rol === "Superintendente"
      ? "superintendente"
      : rol === "Residente"
      ? "residente"
      : rol === "Supervisor"
      ? "supervisor"
      : null;

  return (
    <div>
      {!abierta && (
        <div
          style={{
            background: observadoSoft,
            border: `1px solid ${observado}`,
            borderRadius: 4,
            padding: "14px 18px",
            marginBottom: 20,
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <div style={{ fontSize: 13, color: observado, fontWeight: 600 }}>
            La bitácora no ha sido abierta. No es posible crear notas.
          </div>
          {rol === "Residente" && (
            <PrimaryBtn onClick={() => setShowApertura(true)}>Abrir bitácora →</PrimaryBtn>
          )}
        </div>
      )}

      <div style={{ display: "flex", gap: 10, marginBottom: 20, flexWrap: "wrap" }}>
        {puedeCrearNota && abierta && (
          <PrimaryBtn onClick={() => setModalTipo("nota")}>+ Nota</PrimaryBtn>
        )}
        {puedeCrearMinuta && abierta && (
          <SecondaryBtn onClick={() => setModalTipo("minuta")}>+ Minuta</SecondaryBtn>
        )}
        {puedeCrearIncidencia && abierta && (
          <SecondaryBtn onClick={() => setModalTipo("incidencia")}>+ Incidencia</SecondaryBtn>
        )}
        {puedeRegistrarSolicitudes && abierta && (
          <SecondaryBtn onClick={() => setModalTipo("solicitud")}>+ Solicitud/Aviso</SecondaryBtn>
        )}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          <div style={{ position: "relative" }}>
            <Search
              size={13}
              style={{
                position: "absolute",
                left: 10,
                top: "50%",
                transform: "translateY(-50%)",
                color: muted,
              }}
            />
            <input
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
              placeholder="Buscar asunto o folio..."
              style={{
                border: `1px solid ${rule}`,
                background: paper2,
                borderRadius: 3,
                padding: "7px 12px 7px 28px",
                fontSize: 12,
                fontFamily: "'IBM Plex Sans', sans-serif",
                outline: "none",
                width: 200,
              }}
            />
          </div>
          <select
            value={filtroTipo}
            onChange={(e) => setFiltroTipo(e.target.value)}
            style={{
              border: `1px solid ${rule}`,
              background: paper2,
              borderRadius: 3,
              padding: "7px 12px",
              fontSize: 12,
              fontFamily: "'IBM Plex Sans', sans-serif",
              outline: "none",
            }}
          >
            {["Todos", "Apertura", "Nota", "Minuta", "Incidencia", "Solicitud"].map((o) => (
              <option key={o}>{o}</option>
            ))}
          </select>
        </div>
      </div>

      <Card style={{ overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["Folio", "Tipo", "Fecha", "Asunto", "Firmas", "Folio ref.", ""]} />
          <tbody>
            {filtradas.map((b, i) => {
              const fCount = contarFirmas(b.firmas);
              const todas = fCount === "3/3";
              const ninguna = fCount === "0/3";
              return (
                <tr
                  key={b.id}
                  style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                  onMouseEnter={(el) => (el.currentTarget.style.background = obraSoft)}
                  onMouseLeave={(el) =>
                    (el.currentTarget.style.background = i % 2 === 1 ? "#FAF8F2" : paper2)
                  }
                >
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontWeight: 600,
                      color: obra,
                    }}
                  >
                    {b.folio}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    <EstadoBadge estado={b.tipo} />
                  </td>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                    }}
                  >
                    {b.fecha}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    {b.asunto}
                  </td>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                    }}
                  >
                    {b.tipo === "Incidencia" ? (
                      <span style={{ color: muted }}>—</span>
                    ) : (
                      <span
                        style={{
                          color: todas ? aprobado : ninguna ? folio : observado,
                          fontWeight: 600,
                        }}
                      >
                        {fCount} {!todas && "⏳"}
                      </span>
                    )}
                  </td>
                  <td
                    style={{
                      padding: "10px 14px",
                      borderBottom: `1px solid ${rule}`,
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                      color: muted,
                    }}
                  >
                    {b.folioRef || "—"}
                  </td>
                  <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                    <button
                      onClick={() => setDetalle(b)}
                      style={{
                        fontSize: 11.5,
                        color: obra,
                        background: "none",
                        border: `1px solid ${rule}`,
                        borderRadius: 3,
                        padding: "4px 10px",
                        cursor: "pointer",
                        fontFamily: "'IBM Plex Sans', sans-serif",
                      }}
                    >
                      Ver →
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </Card>

      {/* Detalle modal */}
      {detalle && (
        <Modal
          title={`Nota ${detalle.folio} — ${detalle.asunto}`}
          onClose={() => setDetalle(null)}
          width={500}
        >
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Tipo</SectionLabel>
            <EstadoBadge estado={detalle.tipo} />
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Fecha</SectionLabel>
            <div style={{ fontFamily: "JetBrains Mono", fontSize: 12.5 }}>{detalle.fecha}</div>
          </div>
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Contenido</SectionLabel>
            <div
              style={{
                background: "#FAF8F2",
                border: `1px solid ${rule}`,
                borderRadius: 3,
                padding: "12px 14px",
                fontSize: 13,
                lineHeight: 1.6,
              }}
            >
              {detalle.contenido}
            </div>
          </div>
          {detalle.folioRef && (
            <div style={{ marginBottom: 14 }}>
              <SectionLabel>Folio de referencia</SectionLabel>
              <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, color: obra }}>
                {detalle.folioRef}
              </span>
            </div>
          )}
          {detalle.tipo !== "Incidencia" && (
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Firmas</SectionLabel>
              <div style={{ display: "flex", gap: 8 }}>
                {[
                  { key: "superintendente", label: "Superintendente" },
                  { key: "residente", label: "Residente" },
                  { key: "supervisor", label: "Supervisión" },
                ].map((s) => {
                  const done = detalle.firmas[s.key as keyof typeof detalle.firmas];
                  const esMiTurno = actorFirma === s.key && !done;
                  return (
                    <div
                      key={s.key}
                      onClick={() => esMiTurno && handleFirmar(detalle, s.key)}
                      style={{
                        flex: 1,
                        border: `1.5px solid ${done ? aprobado : esMiTurno ? obra : rule}`,
                        background: done ? aprobadoSoft : esMiTurno ? obraSoft : "#FAF8F2",
                        borderRadius: 3,
                        padding: "10px 6px",
                        textAlign: "center",
                        fontSize: 11,
                        color: done ? aprobado : esMiTurno ? obra : muted,
                        fontWeight: done ? 700 : 400,
                        cursor: esMiTurno ? "pointer" : "default",
                      }}
                    >
                      {done ? `✓ ${s.label}` : esMiTurno ? `✍ Firmar` : s.label}
                    </div>
                  );
                })}
              </div>
            </div>
          )}
          {detalle.tipo === "Incidencia" && (
            <PrimaryBtn
              onClick={() => {
                /* generar nota desde incidencia */
              }}
              style={{ width: "100%", marginBottom: 10 }}
            >
              Generar nota de bitácora desde esta incidencia
            </PrimaryBtn>
          )}
        </Modal>
      )}

      {showApertura && (
        <ModalAperturaBitacora
          onClose={() => setShowApertura(false)}
          onAbrir={() => {
            // api.abrirBitacora(mockContrato.id, data)
            setAbierta(true);
            setShowApertura(false);
          }}
        />
      )}

      {modalTipo === "nota" && (
        <ModalNota
          tipo="Nota"
          onClose={() => setModalTipo(null)}
          siguienteFolio={`${String(
            notas.filter((n) => n.folio.match(/^\d+$/)).length + 1
          ).padStart(4, "0")}`}
          onGuardar={(data) => {
            const nueva: NotaBitacora = {
              id: Date.now(),
              folio: data.folio,
              tipo: "Nota",
              fecha: new Date().toLocaleDateString("es-MX"),
              asunto: data.asunto,
              contenido: data.contenido,
              firmas: {
                superintendente: rol === "Superintendente",
                residente: false,
                supervisor: false,
              },
              folioRef: data.folioRef || null,
            };
            // api.crearNota(mockContrato.id, data)
            setNotas((prev) => [...prev, nueva]);
            setModalTipo(null);
          }}
        />
      )}

      {modalTipo === "minuta" && (
        <ModalMinuta
          onClose={() => setModalTipo(null)}
          onGuardar={(data) => {
            // api.crearMinuta(mockContrato.id, data)
            setNotas((prev) => [
              ...prev,
              {
                id: Date.now(),
                folio: `MIN-${Date.now()}`,
                tipo: "Minuta",
                fecha: data.fecha,
                asunto: data.acuerdos.substring(0, 40) + "...",
                contenido: data.acuerdos,
                firmas: { superintendente: false, residente: false, supervisor: true },
                folioRef: null,
              },
            ]);
            setModalTipo(null);
          }}
        />
      )}

      {modalTipo === "incidencia" && (
        <ModalIncidencia
          onClose={() => setModalTipo(null)}
          onGuardar={(data) => {
            // api.crearIncidencia(mockContrato.id, data)
            setNotas((prev) => [
              ...prev,
              {
                id: Date.now(),
                folio: `INC-${String(
                  notas.filter((n) => n.folio.startsWith("INC")).length + 1
                ).padStart(2, "0")}`,
                tipo: "Incidencia",
                fecha: data.fecha,
                asunto: data.descripcion.substring(0, 50),
                contenido: data.descripcion,
                firmas: { superintendente: false, residente: false, supervisor: false },
                folioRef: null,
                foto: true,
              },
            ]);
            setModalTipo(null);
          }}
        />
      )}

      {modalTipo === "solicitud" && (
        <ModalSolicitud
          onClose={() => setModalTipo(null)}
          onGuardar={(data) => {
            setNotas((prev) => [
              ...prev,
              {
                id: Date.now(),
                folio: String(notas.length + 1).padStart(4, "0"),
                tipo: "Solicitud",
                fecha: new Date().toLocaleDateString("es-MX"),
                asunto: `${data.tipoSolicitud}: ${data.asunto}`,
                contenido: data.descripcion,
                firmas: { superintendente: true, residente: false, supervisor: false },
                folioRef: null,
              },
            ]);
            setModalTipo(null);
          }}
        />
      )}
    </div>
  );
}
