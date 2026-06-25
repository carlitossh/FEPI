import { useEffect, useState } from "react";
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
import {
  ink,
  paper2,
  rule,
  obra,
  obraSoft,
  aprobado,
  aprobadoSoft,
  observado,
  observadoSoft,
  folio,
  muted,
} from "../../../styles/theme";
import type { FirmasBitacora, NotaBitacora, TipoNota } from "../types";
import { bitacoraService } from "../services/bitacoraService";

interface TabBitacoraProps {
  rol: string;
}

type EventoBitacora = NotaBitacora & {
  firmasTexto: string;
  esFirmable: boolean;
};

const CONTRATO_ID = 1;
const BITACORA_ID = 1;
const USUARIO_ID = 1;

const RolSistemaApi = {
  Residencia: 2,
  Superintendente: 3,
  SupervisorExterno: 4,
} as const;

function rolApiDesdeRol(rol: string) {
  if (rol === "Residente") return RolSistemaApi.Residencia;
  if (rol === "Superintendente") return RolSistemaApi.Superintendente;
  if (rol === "Supervisor") return RolSistemaApi.SupervisorExterno;
  return RolSistemaApi.Superintendente;
}

function mapFirmas(firmas: any[] | undefined): FirmasBitacora {
  const base = {
    superintendente: false,
    residente: false,
    supervisor: false,
  };

  if (!firmas) return base;

  for (const f of firmas) {
    const rolFirmante = Number(f.rol ?? f.rolFirmante);
    const firmado = Boolean(f.firmado);

    if (rolFirmante === 3) base.superintendente = firmado;
    if (rolFirmante === 2) base.residente = firmado;
    if (rolFirmante === 4) base.supervisor = firmado;
  }

  return base;
}

function contarFirmasLocal(firmas: FirmasBitacora) {
  const total = [
    firmas.superintendente,
    firmas.residente,
    firmas.supervisor,
  ].filter(Boolean).length;

  return `${total}/3`;
}

function mapTipoNota(tipo: number | string): TipoNota {
  if (typeof tipo === "string") return tipo as TipoNota;

  const tipos: Record<number, TipoNota> = {
    1: "Apertura",
    2: "Nota",
    3: "Minuta",
    4: "Incidencia",
  };

  return tipos[tipo] ?? "Nota";
}

function mapNota(n: any): EventoBitacora {
  const firmas = mapFirmas(n.firmas ?? n.Firmas);

  return {
    id: n.id,
    folio: String(n.folio).padStart(4, "0"),
    tipo: mapTipoNota(n.tipoRegistro),
    fecha: n.fechaRegistro
      ? new Date(n.fechaRegistro).toLocaleDateString("es-MX")
      : "—",
    asunto: n.asunto,
    contenido: n.contenido,
    firmas,
    firmasTexto: contarFirmasLocal(firmas),
    esFirmable: true,
    folioRef: n.folioVinculadoId ? String(n.folioVinculadoId) : null,
    firmasDetalle:
  n.firmas?.map((f: any) => ({
    rol:
      Number(f.rol ?? f.rolFirmante) === 2
        ? "Residente"
        : Number(f.rol ?? f.rolFirmante) === 3
        ? "Superintendente"
        : Number(f.rol ?? f.rolFirmante) === 4
        ? "Supervisión"
        : "Otro",
    usuarioId: f.usuarioId,
    nombreUsuario: f.nombreUsuario ?? "—",
    firmado: f.firmado,
    fechaFirma: f.fechaFirma,
  })) ?? [],
  };
}

function mapEvento(e: any): EventoBitacora {
  const tipo = e.tipo as TipoNota;
  const esFirmable = tipo === "Nota" || tipo === "Apertura";

  return {
    id: e.id,
    folio: e.folio,
    tipo,
    fecha: e.fecha ? new Date(e.fecha).toLocaleDateString("es-MX") : "—",
    asunto: e.asunto,
    contenido: e.contenido,
    firmas: {
      superintendente: false,
      residente: false,
      supervisor: false,
    },
    firmasTexto: e.firmas ?? "—",
    esFirmable,
    folioRef: e.folioRef ?? null,
  };
}

export function TabBitacora({ rol }: TabBitacoraProps) {
  const [eventos, setEventos] = useState<EventoBitacora[]>([]);
  const [modalTipo, setModalTipo] = useState<string | null>(null);
  const [busqueda, setBusqueda] = useState("");
  const [filtroTipo, setFiltroTipo] = useState("Todos");
  const [abierta, setAbierta] = useState(true);
  const [showApertura, setShowApertura] = useState(false);
  const [detalle, setDetalle] = useState<EventoBitacora | null>(null);

  const puedeCrearNota = ["Residente", "Supervisor", "Superintendente"].includes(rol);
  const puedeCrearMinuta = rol === "Supervisor";
  const puedeCrearIncidencia = rol === "Supervisor";
  const puedeRegistrarSolicitudes = rol === "Superintendente";

  const actorFirma =
    rol === "Superintendente"
      ? "superintendente"
      : rol === "Residente"
      ? "residente"
      : rol === "Supervisor"
      ? "supervisor"
      : null;

  async function cargarEventos() {
    try {
      const data = await bitacoraService.buscarEventos(BITACORA_ID);
      setEventos(data.map(mapEvento));
      setAbierta(true);
    } catch (error) {
      console.error("Error cargando eventos:", error);
      setEventos([]);
      setAbierta(true);
    }
  }

  async function abrirDetalle(evento: EventoBitacora) {
    if (evento.esFirmable) {
      const notas = await bitacoraService.buscarNotas(BITACORA_ID);
      const nota = notas.find((n: any) => n.id === evento.id);

      if (nota) {
        setDetalle(mapNota(nota));
        return;
      }
    }

    setDetalle(evento);
  }

  async function handleFirmar(nota: EventoBitacora, actor: keyof FirmasBitacora) {
    try {
      await bitacoraService.firmarNota(nota.id, {
        usuarioId: USUARIO_ID,
        rol: rolApiDesdeRol(rol),
      });

      await cargarEventos();
      await abrirDetalle(nota);
    } catch (error) {
      console.error("Error firmando nota:", error);
      alert("No se pudo firmar la nota. Revisa consola o backend.");
    }
  }

  useEffect(() => {
    cargarEventos();
  }, []);

  const filtradas = eventos
    .filter((e) => filtroTipo === "Todos" || e.tipo === filtroTipo)
    .filter(
      (e) =>
        busqueda === "" ||
        e.asunto.toLowerCase().includes(busqueda.toLowerCase()) ||
        e.folio.includes(busqueda)
    );

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
            La bitácora no ha sido abierta. No es posible crear registros.
          </div>

          {rol === "Residente" && (
            <PrimaryBtn onClick={() => setShowApertura(true)}>
              Abrir bitácora →
            </PrimaryBtn>
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
          <SecondaryBtn onClick={() => setModalTipo("incidencia")}>
            + Incidencia
          </SecondaryBtn>
        )}

        {puedeRegistrarSolicitudes && abierta && (
          <SecondaryBtn onClick={() => setModalTipo("solicitud")}>
            + Solicitud/Aviso
          </SecondaryBtn>
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
            {["Todos", "Apertura", "Nota", "Minuta", "Incidencia"].map((o) => (
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
              const todas = b.firmasTexto === "3/3";
              const ninguna = b.firmasTexto === "0/3";

              return (
                <tr
                  key={`${b.tipo}-${b.id}`}
                  style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                  onMouseEnter={(el) => (el.currentTarget.style.background = obraSoft)}
                  onMouseLeave={(el) =>
                    (el.currentTarget.style.background =
                      i % 2 === 1 ? "#FAF8F2" : paper2)
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
                    {b.esFirmable ? (
                      <span
                        style={{
                          color: todas ? aprobado : ninguna ? folio : observado,
                          fontWeight: 600,
                        }}
                      >
                        {b.firmasTexto} {!todas && "⏳"}
                      </span>
                    ) : (
                      <span style={{ color: muted }}>—</span>
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
                      onClick={() => abrirDetalle(b)}
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

      {detalle && (
        <Modal
          title={`${detalle.tipo} ${detalle.folio} — ${detalle.asunto}`}
          onClose={() => setDetalle(null)}
          width={500}
        >
          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Tipo</SectionLabel>
            <EstadoBadge estado={detalle.tipo} />
          </div>

          <div style={{ marginBottom: 14 }}>
            <SectionLabel>Fecha</SectionLabel>
            <div style={{ fontFamily: "JetBrains Mono", fontSize: 12.5 }}>
              {detalle.fecha}
            </div>
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

          {detalle.esFirmable && (
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Firmas</SectionLabel>

              <div style={{ display: "flex", gap: 8 }}>
                {[
                  { key: "superintendente" as const, label: "Superintendente" },
                  { key: "residente" as const, label: "Residente" },
                  { key: "supervisor" as const, label: "Supervisión" },
                ].map((s) => {
                  const done = detalle.firmas[s.key];
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
                      {done ? `✓ ${s.label}` : esMiTurno ? "✍ Firmar" : s.label}
                    </div>
                  );
                })}
              </div>
            </div>
          )}

{detalle.firmasDetalle && detalle.firmasDetalle.length > 0 && (
  <div style={{ marginBottom: 20 }}>
    <SectionLabel>Historial de firmas</SectionLabel>

    {[...detalle.firmasDetalle]
      .filter((f) => f.firmado)
      .sort((a, b) => {
        const fa = a.fechaFirma ? new Date(a.fechaFirma).getTime() : 0;
        const fb = b.fechaFirma ? new Date(b.fechaFirma).getTime() : 0;
        return fa - fb;
      })
      .map((f, index) => (
        <div
          key={`${f.rol}-${index}`}
          style={{
            border: `1px solid ${rule}`,
            borderRadius: 4,
            padding: "10px 12px",
            marginBottom: 8,
            background: "#FAF8F2",
          }}
        >
          <div
            style={{
              fontWeight: 600,
              color: aprobado,
              marginBottom: 4,
            }}
          >
            {index + 1}. {f.rol}
          </div>

          <div style={{ fontSize: 12, color: muted }}>
            {f.nombreUsuario}
          </div>

          <div style={{ fontSize: 12, color: muted }}>
            {f.fechaFirma
              ? new Date(f.fechaFirma).toLocaleString("es-MX")
              : "Sin fecha"}
          </div>
        </div>
      ))}
  </div>
)}

          {detalle.tipo === "Incidencia" && (
            <PrimaryBtn
              onClick={async () => {
                await bitacoraService.generarNotaDesdeIncidencia({
                  incidenciaId: detalle.id,
                  tipoNotaCatalogoId: 2,
                  usuarioEmisorId: USUARIO_ID,
                  rolEmisor: rolApiDesdeRol(rol),
                });

                setDetalle(null);
                await cargarEventos();
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
          onAbrir={async (data) => {
            await bitacoraService.abrirBitacora({
              contratoId: CONTRATO_ID,
              nombreContrato: data.descripcion,
              numeroContrato: `Contrato-${CONTRATO_ID}`,
              tipoContrato: "ObraPublica",
              dependenciaContratante: "Dependencia de prueba",
              contratistaEmpresa: "Contratista de prueba",
              residenteNombre: "Residente",
              supervisorDesignadoNombre: "Supervisor",
              superintendenteNombre: "Superintendente",
              fechaAperturaFormal: data.fecha,
            });

            setAbierta(true);
            setShowApertura(false);
            await cargarEventos();
          }}
        />
      )}

      {modalTipo === "nota" && (
        <ModalNota
          tipo="Nota"
          onClose={() => setModalTipo(null)}
          siguienteFolio={`${String(
            eventos.filter((e) => e.tipo === "Nota").length + 1
          ).padStart(4, "0")}`}
          onGuardar={async (data) => {
            await bitacoraService.crearNota({
              bitacoraId: BITACORA_ID,
              tipoNotaCatalogoId: 2,
              asunto: data.asunto,
              contenido: data.contenido,
              folioVinculadoId: data.folioRef ? Number(data.folioRef) : null,
              usuarioEmisorId: USUARIO_ID,
              rolEmisor: rolApiDesdeRol(rol),
            });

            setModalTipo(null);
            await cargarEventos();
          }}
        />
      )}

      {modalTipo === "minuta" && (
        <ModalMinuta
          onClose={() => setModalTipo(null)}
          onGuardar={async (data) => {
            await bitacoraService.crearMinuta({
              bitacoraId: BITACORA_ID,
              fecha: data.fecha,
              lugar: "Sala de juntas de obra",
              contenidoAcuerdos: data.acuerdos,
              participantes: data.participantes
                .split(",")
                .map((x) => x.trim())
                .filter(Boolean),
            });

            setModalTipo(null);
            await cargarEventos();
          }}
        />
      )}

      {modalTipo === "incidencia" && (
        <ModalIncidencia
          onClose={() => setModalTipo(null)}
          onGuardar={async (data) => {
            await bitacoraService.crearIncidencia({
              bitacoraId: BITACORA_ID,
              fechaEvento: data.fecha,
              descripcion: data.descripcion,
              urlFotografia: data.foto,
              actorRegistroId: USUARIO_ID,
            });

            setModalTipo(null);
            await cargarEventos();
          }}
        />
      )}

      {modalTipo === "solicitud" && (
        <ModalSolicitud
          onClose={() => setModalTipo(null)}
          onGuardar={async (data) => {
            await bitacoraService.crearNota({
              bitacoraId: BITACORA_ID,
              tipoNotaCatalogoId: 2,
              asunto: `${data.tipoSolicitud}: ${data.asunto}`,
              contenido: data.descripcion,
              folioVinculadoId: null,
              usuarioEmisorId: USUARIO_ID,
              rolEmisor: rolApiDesdeRol(rol),
            });

            setModalTipo(null);
            await cargarEventos();
          }}
        />
      )}
    </div>
  );
}