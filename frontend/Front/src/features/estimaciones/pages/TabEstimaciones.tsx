import { useEffect, useState } from "react";
import { Clock, DollarSign, Search } from "lucide-react";
import { Card } from "../../../components/Card";
import { PrimaryBtn } from "../../../components/PrimaryBtn";
import { SecondaryBtn } from "../../../components/SecondaryBtn";
import { DangerBtn } from "../../../components/DangerBtn";
import { TableHeader } from "../../../components/TableHeader";
import { EstadoBadge } from "../../../components/EstadoBadge";
import { SectionLabel } from "../../../components/SectionLabel";
import { TextInput } from "../../../components/TextInput";
import { TextArea } from "../../../components/TextArea";
import { Modal } from "../../../components/Modal";
import { ModalNuevaEstimacion } from "../components/ModalNuevaEstimacion";
import { ModalAddConcepto } from "../components/ModalAddConcepto";
import { ModalVincularBitacora } from "../components/ModalVincularBitacora";
import { bitacoraService } from "../../bitacora/services/bitacoraService";
import type { NotaBitacora, TipoNota } from "../../bitacora/types";
import { fmtMXN } from "../../../imports/fmtMXN";
import {
  ink,
  paper2,
  rule,
  folio,
  folioSoft,
  obra,
  obraSoft,
  aprobado,
  observado,
  observadoSoft,
  muted,
  pagado,
} from "../../../styles/theme";
import { estimacionesService } from "../services/estimacionesService";
import type { Estimacion, ConceptoEstimacion } from "../types";

interface TabEstimacionesProps {
  rol: string;
}

const CONTRATO_ID = 1;
const BITACORA_ID = 1;
const USUARIO_ID = 1;

const TIPO_NOTA_MAP: Record<number, TipoNota> = {
  1: "Apertura",
  2: "Nota",
  3: "Minuta",
  4: "Incidencia",
};

function mapNotaParaVincular(n: any): NotaBitacora {
  const tipo: TipoNota =
    typeof n.tipoRegistro === "string"
      ? (n.tipoRegistro as TipoNota)
      : TIPO_NOTA_MAP[n.tipoRegistro] ?? "Nota";

  const firmasArr: any[] = n.firmas ?? [];
  const firmas = {
    residente: firmasArr.some((f) => Number(f.rol ?? f.rolFirmante) === 2 && f.firmado),
    superintendente: firmasArr.some((f) => Number(f.rol ?? f.rolFirmante) === 3 && f.firmado),
    supervisor: firmasArr.some((f) => Number(f.rol ?? f.rolFirmante) === 4 && f.firmado),
  };

  return {
    id: n.id,
    folio: String(n.folio).padStart(4, "0"),
    tipo,
    fecha: n.fechaRegistro ? new Date(n.fechaRegistro).toLocaleDateString("es-MX") : "—",
    asunto: n.asunto,
    contenido: n.contenido,
    firmas,
    folioRef: n.folioVinculadoId ? String(n.folioVinculadoId) : null,
  };
}

function mapEstado(estado: number | string): Estimacion["estado"] {
  if (typeof estado === "string") return estado as Estimacion["estado"];

  const estados: Record<number, Estimacion["estado"]> = {
    1: "Borrador",
    2: "Enviada",
    3: "Observada",
    4: "Aprobada",
    5: "Rechazada" as Estimacion["estado"],
    6: "Pagada",
  };

  return estados[estado] ?? "Borrador";
}

function mapResumen(e: any): Estimacion {
  return {
    id: e.id,
    periodo: e.periodo,
    monto: e.montoEstimado ?? e.monto ?? 0,
    estado: mapEstado(e.estado),
    dias: e.estado === 2 ? "18/20" : undefined,
    conceptos: [],
    notasVinculadas: [],
    observaciones: [],
    historial: [],
  };
}

function mapDetalle(e: any): Estimacion {
  return {
    id: e.id,
    periodo: e.periodo,
    monto: e.montoEstimado ?? 0,
    estado: mapEstado(e.estado),
    dias: e.estado === 2 ? "18/20" : undefined,
    conceptos:
      e.conceptos?.map((c: any) => ({
        clave: c.clave,
        desc: c.descripcion,
        unidad: c.unidadMedida ?? "",
        cantEjecutada: c.cantidadEjecutada,
        precioUnitario: c.precioUnitario,
        importe: c.importe,
      })) ?? [],
    notasVinculadas: e.notasVinculadasResumen ?? [],
    observaciones:
      e.observaciones?.map((o: any) => ({
        ref: "General",
        txt: o.texto,
        quien: `Usuario ${o.usuarioId}`,
        hora: new Date(o.fecha).toLocaleString("es-MX"),
        color: observado,
      })) ?? [],
    historial: [],
  };
}

export function TabEstimaciones({ rol }: TabEstimacionesProps) {
  const [estimaciones, setEstimaciones] = useState<Estimacion[]>([]);
  const [selected, setSelected] = useState<Estimacion | null>(null);
  const [filtroEstado, setFiltroEstado] = useState("Todos");
  const [busqueda, setBusqueda] = useState("");
  const [obsTab, setObsTab] = useState("Carátula");

  const EstadoEstimacionApi = {
    Borrador: 1,
    Enviada: 2,
    Observada: 3,
    Aprobada: 4,
    Rechazada: 5,
    Pagada: 6,
  } as const;

  const [showNuevaEst, setShowNuevaEst] = useState(false);
  const [showAddConcepto, setShowAddConcepto] = useState(false);
  const [showObservacion, setShowObservacion] = useState(false);
  const [showRechazo, setShowRechazo] = useState(false);
  const [showPago, setShowPago] = useState(false);
  const [showHistorial, setShowHistorial] = useState(false);
  const [showVincular, setShowVincular] = useState(false);
  const [notasBitacora, setNotasBitacora] = useState<NotaBitacora[]>([]);

  const [nuevaObs, setNuevaObs] = useState("");
  const [motivoRechazo, setMotivoRechazo] = useState("");
  const [datoPago, setDatoPago] = useState({
    referencia: "",
    fecha: "",
    monto: "",
  });

  const puedeEnviar = rol === "Superintendente";
  const puedeObservar = rol === "Supervisor";
  const puedeAprobarRechazar = rol === "Residente";
  const puedePagar = rol === "Financiero";

  const esBorrador = selected?.estado === "Borrador";
  const esEnviada = selected?.estado === "Enviada";
  const esAprobada = selected?.estado === "Aprobada";

  async function cargarEstimaciones() {
    const data = await estimacionesService.listarPorContrato(CONTRATO_ID);
    setEstimaciones(data.map(mapResumen));
  }

  async function abrirEstimacion(id: number) {
    const detalle = await estimacionesService.obtenerDetalle(id);
    const mapped = mapDetalle(detalle);

    const historial = await estimacionesService.obtenerHistorial(id);

    mapped.historial =
      historial?.map((h: any) => ({
        estadoAnterior: mapEstado(h.estadoAnterior),
        estadoNuevo: mapEstado(h.estadoNuevo),
        fecha: new Date(h.fecha).toLocaleString("es-MX"),
        usuario: `Usuario ${h.usuarioId}`,
      })) ?? [];

    setSelected(mapped);
    setObsTab("Carátula");
  }

  useEffect(() => {
    cargarEstimaciones();
  }, []);

  const filtradas = estimaciones
    .filter((e) => filtroEstado === "Todos" || e.estado === filtroEstado)
    .filter(
      (e) =>
        busqueda === "" ||
        String(e.id).includes(busqueda) ||
        e.periodo.toLowerCase().includes(busqueda.toLowerCase())
    );

  const visibles = puedePagar
    ? estimaciones.filter((e) => e.estado === "Aprobada")
    : filtradas;

  async function handleEnviar() {
    if (!selected) return;

    await estimacionesService.enviar(selected.id, USUARIO_ID);
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleAprobar() {
    if (!selected) return;

    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoEstimacionApi.Aprobada,
      usuarioId: USUARIO_ID,
      comentario: "Estimación aprobada.",
    });

    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleRechazar() {
    if (!selected || !motivoRechazo.trim()) return;

    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoEstimacionApi.Rechazada,
      usuarioId: USUARIO_ID,
      comentario: motivoRechazo,
    });

    setMotivoRechazo("");
    setShowRechazo(false);

    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleObservar() {
    if (!selected) return;

    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoEstimacionApi.Observada,
      usuarioId: USUARIO_ID,
      comentario: "Estimación marcada como observada.",
    });

    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handlePago() {
    if (!selected || !datoPago.referencia || !datoPago.fecha || !datoPago.monto) return;

    const monto = Number(datoPago.monto);
    if (isNaN(monto) || monto <= 0) {
      alert("El monto pagado debe ser mayor a cero.");
      return;
    }

    await estimacionesService.registrarPago(selected.id, {
      referenciaBancaria: datoPago.referencia,
      fechaPago: datoPago.fecha,
      montoPagado: monto,
    });

    setDatoPago({ referencia: "", fecha: "", monto: "" });
    setShowPago(false);

    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleNuevaObs() {
    if (!selected || !nuevaObs.trim()) return;

    await estimacionesService.agregarObservacion(selected.id, nuevaObs, USUARIO_ID);

    setNuevaObs("");
    setShowObservacion(false);

    await abrirEstimacion(selected.id);
  }

  async function abrirModalVincular() {
    const data = await bitacoraService.buscarNotas(BITACORA_ID);
    setNotasBitacora(data.map(mapNotaParaVincular));
    setShowVincular(true);
  }

  async function handleVincular(ids: number[]) {
    if (!selected || ids.length === 0) return;
    await estimacionesService.vincularNotasBitacora(selected.id, ids);
    setShowVincular(false);
    await abrirEstimacion(selected.id);
  }

  const detailTabs = ["Carátula", "Generadores", "Notas vinculadas", "Historial"];
    if (selected) {
    const totalEst =
      selected.conceptos?.reduce((a, c) => a + (c.importe || 0), 0) ||
      selected.monto;

    return (
      <div>
        <button
          onClick={() => setSelected(null)}
          style={{
            display: "flex",
            alignItems: "center",
            gap: 6,
            fontSize: 12,
            color: obra,
            background: "none",
            border: "none",
            cursor: "pointer",
            padding: "0 0 16px 0",
            fontFamily: "'IBM Plex Sans', sans-serif",
          }}
        >
          ← Volver a bandeja
        </button>

        <div
          style={{
            background: ink,
            color: paper2,
            borderRadius: "4px 4px 0 0",
            overflow: "hidden",
          }}
        >
          <div
            style={{
              background: folio,
              display: "flex",
              justifyContent: "space-between",
              padding: "5px 20px",
              fontSize: 10.5,
              letterSpacing: "0.06em",
            }}
          >
            <span>
              SELLO DIGITAL · FOLIO-EST-
              {String(selected.id).padStart(3, "0")}
            </span>
            <span>Plazo legal: Art. 55 LOPSRM — 20 días hábiles</span>
          </div>

          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              padding: "14px 20px",
            }}
          >
            <div>
              <div
                style={{
                  fontFamily: "'IBM Plex Serif', serif",
                  fontWeight: 700,
                  fontSize: 18,
                }}
              >
                Estimación N.° {String(selected.id).padStart(3, "0")}
              </div>
              <div style={{ fontSize: 11, color: "#C7C2B0", marginTop: 2 }}>
                Contrato {CONTRATO_ID} · Periodo {selected.periodo}
              </div>
            </div>

            <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
              <EstadoBadge estado={selected.estado} />
              <button
                onClick={() => setShowHistorial(true)}
                style={{
                  fontSize: 11,
                  color: muted,
                  background: "none",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  padding: "4px 10px",
                  cursor: "pointer",
                  fontFamily: "'IBM Plex Sans', sans-serif",
                  display: "flex",
                  alignItems: "center",
                  gap: 4,
                }}
              >
                <Clock size={11} /> Historial
              </button>
            </div>
          </div>
        </div>

        <div
          style={{
            display: "flex",
            background: "#EFEAE0",
            borderBottom: `1px solid ${rule}`,
          }}
        >
          {detailTabs.map((t) => (
            <button
              key={t}
              onClick={() => setObsTab(t)}
              style={{
                padding: "10px 18px",
                fontSize: 12.5,
                color: obsTab === t ? obra : muted,
                border: "none",
                borderBottom: `3px solid ${
                  obsTab === t ? obra : "transparent"
                }`,
                background: obsTab === t ? paper2 : "transparent",
                fontWeight: obsTab === t ? 600 : 400,
                cursor: "pointer",
                fontFamily: "'IBM Plex Sans', sans-serif",
              }}
            >
              {t}
            </button>
          ))}
        </div>

        {obsTab === "Carátula" && (
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 290px",
              border: `1px solid ${rule}`,
              borderTop: "none",
              background: paper2,
            }}
          >
            <div
              style={{
                padding: "20px 22px",
                borderRight: `1px solid ${rule}`,
              }}
            >
              {selected.dias && (
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: 12,
                    background: observadoSoft,
                    border: `1px solid ${rule}`,
                    borderLeft: `3px solid ${observado}`,
                    borderRadius: 3,
                    padding: "10px 14px",
                    marginBottom: 18,
                  }}
                >
                  <div
                    style={{
                      width: 38,
                      height: 38,
                      borderRadius: "50%",
                      border: `4px solid ${observado}`,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      fontSize: 11,
                      fontWeight: 700,
                      color: observado,
                      flexShrink: 0,
                      fontFamily: "JetBrains Mono",
                    }}
                  >
                    {selected.dias}
                  </div>
                  <div style={{ fontSize: 12, color: muted }}>
                    Estimación enviada. Revisar dentro del plazo legal.
                  </div>
                </div>
              )}

              <div style={{ display: "flex", gap: 12, marginBottom: 20 }}>
                {[
                  { v: fmtMXN(totalEst), l: "Monto estimado" },
                  { v: selected.periodo, l: "Periodo" },
                  { v: selected.estado, l: "Estado actual" },
                ].map((k) => (
                  <div
                    key={k.l}
                    style={{
                      flex: 1,
                      border: `1px solid ${rule}`,
                      borderRadius: 4,
                      padding: "12px 14px",
                    }}
                  >
                    <div
                      style={{
                        fontFamily: "'IBM Plex Serif', serif",
                        fontSize: 16,
                        fontWeight: 700,
                        color: obra,
                      }}
                    >
                      {k.v}
                    </div>
                    <div
                      style={{
                        fontSize: 10,
                        textTransform: "uppercase",
                        letterSpacing: "0.04em",
                        color: muted,
                        marginTop: 2,
                      }}
                    >
                      {k.l}
                    </div>
                  </div>
                ))}
              </div>

              <table
                style={{
                  width: "100%",
                  borderCollapse: "collapse",
                  fontSize: 12.5,
                }}
              >
                <TableHeader
                  cols={[
                    "Clave",
                    "Descripción",
                    "Unidad",
                    "Cant.",
                    "P.U.",
                    "Importe",
                  ]}
                />
                <tbody>
                  {(selected.conceptos || []).map((r) => (
                    <tr key={r.clave}>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                          fontFamily: "JetBrains Mono",
                          fontSize: 11.5,
                        }}
                      >
                        {r.clave}
                      </td>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                        }}
                      >
                        {r.desc}
                      </td>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                          fontFamily: "JetBrains Mono",
                          fontSize: 11.5,
                        }}
                      >
                        {r.unidad}
                      </td>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                          fontFamily: "JetBrains Mono",
                          fontSize: 11.5,
                        }}
                      >
                        {r.cantEjecutada}
                      </td>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                          fontFamily: "JetBrains Mono",
                          fontSize: 11.5,
                        }}
                      >
                        {fmtMXN(r.precioUnitario)}
                      </td>
                      <td
                        style={{
                          padding: "8px 14px",
                          borderBottom: `1px solid ${rule}`,
                          fontFamily: "JetBrains Mono",
                          fontSize: 11.5,
                          fontWeight: 600,
                        }}
                      >
                        {fmtMXN(r.importe)}
                      </td>
                    </tr>
                  ))}

                  {(selected.conceptos || []).length === 0 && (
                    <tr>
                      <td
                        colSpan={6}
                        style={{
                          padding: "20px 14px",
                          color: muted,
                          textAlign: "center",
                        }}
                      >
                        Sin conceptos registrados
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>

              <div
                style={{
                  marginTop: 16,
                  padding: "12px 14px",
                  background: "#FAF8F2",
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                }}
              >
                <div
                  style={{
                    fontSize: 10.5,
                    textTransform: "uppercase",
                    letterSpacing: "0.05em",
                    color: muted,
                    marginBottom: 8,
                  }}
                >
                  Resumen — total estimado
                </div>
                <div
                  style={{
                    fontFamily: "'IBM Plex Serif', serif",
                    fontSize: 20,
                    fontWeight: 700,
                    color: obra,
                  }}
                >
                  {fmtMXN(totalEst)}
                </div>
              </div>

              <div
                style={{
                  display: "flex",
                  gap: 10,
                  marginTop: 20,
                  flexWrap: "wrap",
                }}
              >
                {esBorrador && puedeEnviar && (
                  <>
                    <PrimaryBtn onClick={() => setShowAddConcepto(true)}>
                      + Añadir concepto
                    </PrimaryBtn>

                    <PrimaryBtn
                      onClick={handleEnviar}
                      style={{ background: folio, marginLeft: "auto" }}
                    >
                      Enviar estimación →
                    </PrimaryBtn>
                  </>
                )}

                {esEnviada && puedeAprobarRechazar && (
                  <>
                    <PrimaryBtn
                      onClick={handleAprobar}
                      style={{ background: aprobado }}
                    >
                      ✓ Aprobar
                    </PrimaryBtn>
                    <DangerBtn onClick={() => setShowRechazo(true)}>
                      Rechazar
                    </DangerBtn>
                  </>
                )}

                {esEnviada && puedeObservar && (
                  <>
                    <SecondaryBtn onClick={() => setShowObservacion(true)}>
                      + Nueva observación
                    </SecondaryBtn>
                    <SecondaryBtn onClick={handleObservar} color={observado}>
                      Marcar como Observada
                    </SecondaryBtn>
                    <PrimaryBtn
                      onClick={handleAprobar}
                      style={{ background: aprobado }}
                    >
                      Aprobar
                    </PrimaryBtn>
                  </>
                )}

                {esAprobada && puedePagar && (
                  <PrimaryBtn
                    onClick={() => setShowPago(true)}
                    style={{ background: pagado }}
                  >
                    <DollarSign size={13} style={{ marginRight: 4 }} />{" "}
                    Registrar pago
                  </PrimaryBtn>
                )}
              </div>
            </div>

            <div style={{ padding: "20px 18px", background: "#FAF8F2" }}>
              <div
                style={{
                  fontSize: 11.5,
                  fontWeight: 700,
                  color: obra,
                  textTransform: "uppercase",
                  letterSpacing: "0.06em",
                  marginBottom: 14,
                }}
              >
                Observaciones ({(selected.observaciones || []).length})
              </div>

              {(selected.observaciones || []).length === 0 && (
                <div
                  style={{
                    fontSize: 12,
                    color: muted,
                    textAlign: "center",
                    padding: "20px 0",
                  }}
                >
                  Sin observaciones registradas
                </div>
              )}

              {(selected.observaciones || []).map((n, i) => (
                <div
                  key={i}
                  style={{
                    background: paper2,
                    border: `1px solid ${rule}`,
                    borderLeft: `3px solid ${n.color}`,
                    borderRadius: 3,
                    padding: "10px 12px",
                    marginBottom: 10,
                  }}
                >
                  <div
                    style={{
                      fontFamily: "JetBrains Mono",
                      fontSize: 10.5,
                      color: n.color,
                      fontWeight: 700,
                      marginBottom: 4,
                    }}
                  >
                    {n.ref}
                  </div>
                  <div style={{ fontSize: 12.5 }}>{n.txt}</div>
                  <div style={{ fontSize: 10.5, color: muted, marginTop: 6 }}>
                    {n.quien} · {n.hora}
                  </div>
                </div>
              ))}

              {(puedeObservar || puedeAprobarRechazar) && esEnviada && (
                <button
                  onClick={() => setShowObservacion(true)}
                  style={{
                    width: "100%",
                    background: "none",
                    border: `1px solid ${rule}`,
                    borderRadius: 3,
                    padding: "8px 12px",
                    fontSize: 12,
                    color: ink,
                    cursor: "pointer",
                    fontFamily: "'IBM Plex Sans', sans-serif",
                    marginTop: 8,
                  }}
                >
                  + Nueva observación
                </button>
              )}
            </div>
          </div>
        )}

        {obsTab === "Generadores" && (
          <Card
            style={{
              padding: 24,
              borderTop: "none",
              borderRadius: "0 0 4px 4px",
            }}
          >
            <SectionLabel>Generadores de cantidad</SectionLabel>
            {(selected.conceptos || []).map((c) => (
              <div
                key={c.clave}
                style={{
                  border: `1px solid ${rule}`,
                  borderRadius: 3,
                  padding: "12px 16px",
                  marginBottom: 10,
                }}
              >
                <div
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    marginBottom: 6,
                  }}
                >
                  <span
                    style={{
                      fontFamily: "JetBrains Mono",
                      fontSize: 11.5,
                      color: obra,
                      fontWeight: 600,
                    }}
                  >
                    {c.clave}
                  </span>
                  <span style={{ fontSize: 12.5 }}>{c.desc}</span>
                  <span
                    style={{
                      fontFamily: "JetBrains Mono",
                      fontSize: 12,
                      color: ink,
                    }}
                  >
                    {c.cantEjecutada} {c.unidad}
                  </span>
                  <span
                    style={{
                      fontFamily: "JetBrains Mono",
                      fontSize: 12,
                      fontWeight: 600,
                    }}
                  >
                    {fmtMXN(c.importe)}
                  </span>
                </div>
                <div
                  style={{
                    background: "#FAF8F2",
                    border: `1px dashed ${rule}`,
                    borderRadius: 2,
                    padding: "8px 12px",
                    fontSize: 12,
                    color: muted,
                  }}
                >
                  Generador registrado con base en cantidades ejecutadas.
                </div>
              </div>
            ))}
          </Card>
        )}

        {obsTab === "Notas vinculadas" && (
          <Card
            style={{
              padding: 24,
              borderTop: "none",
              borderRadius: "0 0 4px 4px",
            }}
          >
            <div
              style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                marginBottom: 16,
              }}
            >
              <SectionLabel>Notas de bitácora vinculadas</SectionLabel>
              {esBorrador && (
                <SecondaryBtn onClick={abrirModalVincular}>
                  + Vincular notas
                </SecondaryBtn>
              )}
            </div>

            {(selected.notasVinculadas ?? []).length === 0 ? (
              <div
                style={{
                  color: muted,
                  fontSize: 12,
                  textAlign: "center",
                  padding: 20,
                  border: `1px dashed ${rule}`,
                  borderRadius: 4,
                }}
              >
                Sin notas de bitácora vinculadas.
                {esBorrador && " Usa \"+ Vincular notas\" para agregar soporte documental."}
              </div>
            ) : (
              <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                {selected.notasVinculadas.map((resumen, i) => (
                  <div
                    key={i}
                    style={{
                      border: `1px solid ${rule}`,
                      borderLeft: `3px solid ${obra}`,
                      borderRadius: 3,
                      padding: "10px 14px",
                      fontSize: 12.5,
                      background: "#FAF8F2",
                      fontFamily: "JetBrains Mono",
                    }}
                  >
                    {resumen}
                  </div>
                ))}
              </div>
            )}
          </Card>
        )}

        {obsTab === "Historial" && (
          <Card
            style={{
              padding: 24,
              borderTop: "none",
              borderRadius: "0 0 4px 4px",
            }}
          >
            <SectionLabel>Historial de cambios</SectionLabel>
            <table
              style={{
                width: "100%",
                borderCollapse: "collapse",
                fontSize: 12.5,
              }}
            >
              <TableHeader
                cols={["Estado anterior", "Estado nuevo", "Fecha y hora", "Usuario"]}
              />
              <tbody>
                {(selected.historial || []).map((h, i) => (
                  <tr
                    key={i}
                    style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                  >
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      {h.estadoAnterior}
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      <EstadoBadge estado={h.estadoNuevo} />
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                        fontFamily: "JetBrains Mono",
                        fontSize: 11.5,
                      }}
                    >
                      {h.fecha}
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      {h.usuario}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Card>
        )}

        {showAddConcepto && (
          <ModalAddConcepto
    contratoId={CONTRATO_ID}
            onClose={() => setShowAddConcepto(false)}
            onAdd={async (concepto: ConceptoEstimacion) => {
              if (!selected) return;

              await estimacionesService.actualizarConceptos(selected.id, [
                ...(selected.conceptos || []).map((c: any) => ({
                  conceptoContratoId: c.conceptoContratoId ?? c.id ?? 1,
                  cantidadEjecutada: c.cantEjecutada,
                })),
                {
                  conceptoContratoId:
                    (concepto as any).conceptoContratoId ??
                    (concepto as any).id ??
                    1,
                  cantidadEjecutada: concepto.cantEjecutada,
                },
              ]);

              setShowAddConcepto(false);
              await abrirEstimacion(selected.id);
              await cargarEstimaciones();
            }}
          />
        )}

        {showObservacion && (
          <Modal
            title="Nueva observación"
            onClose={() => setShowObservacion(false)}
            width={420}
          >
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Observación</SectionLabel>
              <TextArea
                placeholder="Describe la observación..."
                value={nuevaObs}
                onChange={setNuevaObs}
                rows={4}
              />
            </div>

            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn
                onClick={() => setShowObservacion(false)}
                style={{ flex: 1 }}
              >
                Cancelar
              </SecondaryBtn>
              <PrimaryBtn onClick={handleNuevaObs} style={{ flex: 1 }}>
                Registrar
              </PrimaryBtn>
            </div>
          </Modal>
        )}

        {showRechazo && (
          <Modal
            title="Rechazar estimación"
            onClose={() => setShowRechazo(false)}
            width={420}
          >
            <div
              style={{
                background: folioSoft,
                border: `1px solid ${folio}`,
                borderRadius: 3,
                padding: "10px 14px",
                marginBottom: 16,
                fontSize: 12.5,
                color: folio,
              }}
            >
              Se requiere un motivo antes de confirmar el rechazo.
            </div>

            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Motivo de rechazo</SectionLabel>
              <TextArea
                placeholder="Describe el motivo del rechazo..."
                value={motivoRechazo}
                onChange={setMotivoRechazo}
                rows={4}
              />
            </div>

            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn
                onClick={() => setShowRechazo(false)}
                style={{ flex: 1 }}
              >
                Cancelar
              </SecondaryBtn>
              <DangerBtn onClick={handleRechazar} style={{ flex: 1 }}>
                Confirmar rechazo
              </DangerBtn>
            </div>
          </Modal>
        )}

        {showPago && (
          <Modal
            title="Registrar pago"
            subtitle={"Estimación N.° " + String(selected.id).padStart(3, "0")}
            onClose={() => setShowPago(false)}
            width={440}
          >
            <div style={{ display: "grid", gap: 14, marginBottom: 20 }}>
              <div>
                <SectionLabel>Referencia de pago</SectionLabel>
                <TextInput
                  placeholder="Ej: CLC-2026-08741"
                  value={datoPago.referencia}
                  onChange={(v) =>
                    setDatoPago((p) => ({ ...p, referencia: v }))
                  }
                />
              </div>

              <div>
                <SectionLabel>Fecha de pago</SectionLabel>
                <TextInput
                  placeholder="YYYY-MM-DD"
                  value={datoPago.fecha}
                  onChange={(v) => setDatoPago((p) => ({ ...p, fecha: v }))}
                />
              </div>

              <div>
                <SectionLabel>Monto pagado</SectionLabel>
                <TextInput
                  placeholder="0.00"
                  value={datoPago.monto}
                  onChange={(v) => setDatoPago((p) => ({ ...p, monto: v }))}
                />
              </div>
            </div>

            <div
              style={{
                background: folioSoft,
                border: `1px solid ${folio}`,
                borderRadius: 3,
                padding: "10px 14px",
                marginBottom: 16,
                fontSize: 12,
                color: folio,
              }}
            >
              Esta acción marcará la estimación como <strong>Pagada</strong> de forma definitiva.
            </div>

            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowPago(false)} style={{ flex: 1 }}>
                Cancelar
              </SecondaryBtn>
              <PrimaryBtn
                onClick={handlePago}
                style={{ flex: 1, background: pagado }}
                disabled={
                  !datoPago.referencia ||
                  !datoPago.fecha ||
                  !datoPago.monto ||
                  Number(datoPago.monto) <= 0
                }
              >
                Confirmar pago
              </PrimaryBtn>
            </div>
          </Modal>
        )}

        {showVincular && (
          <ModalVincularBitacora
            notas={notasBitacora}
            vinculadas={selected.notasVinculadas ?? []}
            onClose={() => setShowVincular(false)}
            onVincular={handleVincular}
          />
        )}

        {showHistorial && (
          <Modal
            title="Historial de estados"
            onClose={() => setShowHistorial(false)}
            width={560}
          >
            <table
              style={{
                width: "100%",
                borderCollapse: "collapse",
                fontSize: 12.5,
              }}
            >
              <TableHeader
                cols={["Estado anterior", "Estado nuevo", "Fecha y hora", "Usuario"]}
              />
              <tbody>
                {(selected.historial || []).map((h, i) => (
                  <tr
                    key={i}
                    style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                  >
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      {h.estadoAnterior}
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      <EstadoBadge estado={h.estadoNuevo} />
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                        fontFamily: "JetBrains Mono",
                        fontSize: 11.5,
                      }}
                    >
                      {h.fecha}
                    </td>
                    <td
                      style={{
                        padding: "10px 14px",
                        borderBottom: `1px solid ${rule}`,
                      }}
                    >
                      {h.usuario}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Modal>
        )}
      </div>
    );
  }

  return (
    <div>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          gap: 12,
          marginBottom: 20,
          flexWrap: "wrap",
        }}
      >
        {puedeEnviar && (
          <PrimaryBtn onClick={() => setShowNuevaEst(true)}>
            + Nueva estimación
          </PrimaryBtn>
        )}

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
            placeholder="Buscar folio o periodo..."
            style={{
              border: `1px solid ${rule}`,
              background: paper2,
              borderRadius: 3,
              padding: "7px 12px 7px 30px",
              fontSize: 12.5,
              color: ink,
              fontFamily: "'IBM Plex Sans', sans-serif",
              outline: "none",
              width: 220,
            }}
          />
        </div>

        {!puedePagar && (
          <select
            value={filtroEstado}
            onChange={(e) => setFiltroEstado(e.target.value)}
            style={{
              border: `1px solid ${rule}`,
              background: paper2,
              borderRadius: 3,
              padding: "7px 12px",
              fontSize: 12,
              color: muted,
              fontFamily: "'IBM Plex Sans', sans-serif",
              outline: "none",
            }}
          >
            {["Todos", "Borrador", "Enviada", "Observada", "Aprobada", "Pagada"].map(
              (o) => (
                <option key={o}>{o}</option>
              )
            )}
          </select>
        )}

        {puedePagar && (
          <div style={{ fontSize: 11.5, color: muted }}>
            Mostrando solo estimaciones <strong>Aprobadas</strong> pendientes de pago.
          </div>
        )}
      </div>

      <Card style={{ overflow: "hidden" }}>
        <table
          style={{
            width: "100%",
            borderCollapse: "collapse",
            fontSize: 12.5,
          }}
        >
          <TableHeader cols={["N.°", "Periodo", "Monto", "Estado", "Días plazo", ""]} />
          <tbody>
            {visibles.map((e, i) => (
              <tr
                key={e.id}
                style={{ background: i % 2 === 1 ? "#FAF8F2" : paper2 }}
                onMouseEnter={(el) =>
                  (el.currentTarget.style.background = obraSoft)
                }
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
                  {String(e.id).padStart(3, "0")}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  {e.periodo}
                </td>
                <td
                  style={{
                    padding: "10px 14px",
                    borderBottom: `1px solid ${rule}`,
                    fontFamily: "JetBrains Mono",
                  }}
                >
                  {fmtMXN(e.monto)}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <EstadoBadge estado={e.estado} />
                </td>
                <td
                  style={{
                    padding: "10px 14px",
                    borderBottom: `1px solid ${rule}`,
                    fontFamily: "JetBrains Mono",
                    fontSize: 11.5,
                  }}
                >
                  {e.dias ? (
                    <span style={{ color: observado, fontWeight: 600 }}>
                      {e.dias} ⚑
                    </span>
                  ) : (
                    "—"
                  )}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <button
                    onClick={() => abrirEstimacion(e.id)}
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
            ))}
          </tbody>
        </table>
      </Card>

      {showNuevaEst && (
        <ModalNuevaEstimacion
  contrato={{
    id: CONTRATO_ID,
    numeroContrato: `Contrato ${CONTRATO_ID}`,
  }}
  onClose={() => setShowNuevaEst(false)}
  onCrear={async (data) => {
    try {
      const nueva = await estimacionesService.crear({
        contratoId: CONTRATO_ID,
        periodo: data.periodo,
      });
      setShowNuevaEst(false);
      await cargarEstimaciones();
      await abrirEstimacion(nueva.id);
    } catch (err: any) {
      alert("No se pudo crear la estimación: " + (err?.message ?? "Error desconocido"));
    }
  }}
/>
      )}
    </div>
  );
}