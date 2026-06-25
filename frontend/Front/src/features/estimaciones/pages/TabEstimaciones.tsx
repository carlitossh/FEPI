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
  rule, folio, folioSoft, obra, obraSoft,
  aprobado, observado, observadoSoft, muted, pagado,
} from "../../../styles/theme";
import { C } from "../../../styles/theme";
import { estimacionesService } from "../services/estimacionesService";
import type { Estimacion, ConceptoEstimacion, EstadoEst, EstadoPagoEst, PagoEstimacion } from "../types";

interface TabEstimacionesProps {
  rol: string;
}

const CONTRATO_ID = 1;
const BITACORA_ID = 1;
const USUARIO_ID = 1;

// Números que envía el backend para cada estado técnico
const EstadoApi: Record<EstadoEst, number> = {
  Borrador: 1,
  Enviada: 2,
  ObservadaSupervision: 3,
  AprobadaSupervision: 4,
  RechazadaResidencia: 5,
  AprobadaResidencia: 6,
  Cancelada: 7,
};

// Etiquetas legibles para la UI
const EstadoLabel: Record<EstadoEst, string> = {
  Borrador: "Borrador",
  Enviada: "Enviada",
  ObservadaSupervision: "Observada (sup.)",
  AprobadaSupervision: "Aprobada (sup.)",
  RechazadaResidencia: "Rechazada",
  AprobadaResidencia: "Aprobada",
  Cancelada: "Cancelada",
};

const EstadoPagoLabel: Record<EstadoPagoEst, string> = {
  SinPago: "Sin pago",
  PagoParcial: "Pago parcial",
  Pagada: "Pagada",
};

const TIPO_NOTA_MAP: Record<number, TipoNota> = {
  1: "Apertura", 2: "Nota", 3: "Minuta", 4: "Incidencia",
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

function mapEstado(estado: number | string): EstadoEst {
  if (typeof estado === "string") return estado as EstadoEst;
  const m: Record<number, EstadoEst> = {
    1: "Borrador", 2: "Enviada", 3: "ObservadaSupervision",
    4: "AprobadaSupervision", 5: "RechazadaResidencia",
    6: "AprobadaResidencia", 7: "Cancelada",
  };
  return m[estado] ?? "Borrador";
}

function mapEstadoPago(estadoPago: number | string): EstadoPagoEst {
  if (typeof estadoPago === "string") return estadoPago as EstadoPagoEst;
  const m: Record<number, EstadoPagoEst> = { 1: "SinPago", 2: "PagoParcial", 3: "Pagada" };
  return m[estadoPago] ?? "SinPago";
}

function mapResumen(e: any): Estimacion {
  const montoEst = e.montoEstimado ?? e.monto ?? 0;
  const pagadoAcum = e.montoPagadoAcumulado ?? 0;
  return {
    id: e.id,
    periodo: e.periodo,
    monto: montoEst,
    montoPagadoAcumulado: pagadoAcum,
    saldoPendientePago: e.saldoPendientePago ?? (montoEst - pagadoAcum),
    estado: mapEstado(e.estado),
    estadoPago: mapEstadoPago(e.estadoPago ?? 1),
    dias: e.estado === 2 ? "—" : undefined,
    conceptos: [],
    notasVinculadas: [],
    observaciones: [],
    historial: [],
    pagos: [],
  };
}

function mapDetalle(e: any): Estimacion {
  const montoEst = e.montoEstimado ?? 0;
  const pagadoAcum = e.montoPagadoAcumulado ?? 0;
  return {
    id: e.id,
    periodo: e.periodo,
    monto: montoEst,
    montoPagadoAcumulado: pagadoAcum,
    saldoPendientePago: montoEst - pagadoAcum,
    estado: mapEstado(e.estado),
    estadoPago: mapEstadoPago(e.estadoPago ?? 1),
    conceptos: e.conceptos?.map((c: any) => ({
      clave: c.clave,
      desc: c.descripcion,
      unidad: c.unidadMedida ?? "",
      cantEjecutada: c.cantidadEjecutada,
      precioUnitario: c.precioUnitario,
      importe: c.importe,
    })) ?? [],
    notasVinculadas: e.notasVinculadasResumen ?? [],
    observaciones: e.observaciones?.map((o: any) => ({
      ref: "General",
      txt: o.texto,
      quien: `Usuario ${o.usuarioId}`,
      hora: new Date(o.fecha).toLocaleString("es-MX"),
      color: observado,
    })) ?? [],
    historial: [],
    pagos: e.pagos?.map((p: any) => ({
      id: p.id,
      fechaPago: p.fechaPago,
      referenciaBancaria: p.referenciaBancaria,
      montoPagado: p.montoPagado,
      usuarioRegistroId: p.usuarioRegistroId,
      fechaRegistro: p.fechaRegistro,
    })) ?? [],
  };
}

export function TabEstimaciones({ rol }: TabEstimacionesProps) {
  const [estimaciones, setEstimaciones] = useState<Estimacion[]>([]);
  const [selected, setSelected] = useState<Estimacion | null>(null);
  const [filtroEstado, setFiltroEstado] = useState("Todos");
  const [busqueda, setBusqueda] = useState("");
  const [obsTab, setObsTab] = useState("Carátula");

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
  const [datoPago, setDatoPago] = useState({ referencia: "", fecha: "", monto: "" });

  // Permisos por rol
  const puedeEnviar = rol === "Superintendente";
  const puedeGestionarSupervision = rol === "Supervisor";
  const puedeGestionarResidencia = rol === "Residente";
  const puedePagar = rol === "Financiero";

  // Estado de la estimación seleccionada
  const esBorrador = selected?.estado === "Borrador";
  const esEnviada = selected?.estado === "Enviada";
  const esObservadaSupervision = selected?.estado === "ObservadaSupervision";
  const esAprobadaSupervision = selected?.estado === "AprobadaSupervision";
  const esAprobadaResidencia = selected?.estado === "AprobadaResidencia";
  const tieneSaldoPendiente = (selected?.saldoPendientePago ?? 0) > 0.01;

  async function cargarEstimaciones() {
    const data = await estimacionesService.listarPorContrato(CONTRATO_ID);
    setEstimaciones(data.map(mapResumen));
  }

  async function abrirEstimacion(id: number) {
    const detalle = await estimacionesService.obtenerDetalle(id);
    const mapped = mapDetalle(detalle);

    const historial = await estimacionesService.obtenerHistorial(id);
    mapped.historial = historial?.map((h: any) => ({
      estadoAnterior: EstadoLabel[mapEstado(h.estadoAnterior)] ?? mapEstado(h.estadoAnterior),
      estadoNuevo: EstadoLabel[mapEstado(h.estadoNuevo)] ?? mapEstado(h.estadoNuevo),
      fecha: new Date(h.fecha).toLocaleString("es-MX"),
      usuario: `Usuario ${h.usuarioId}`,
      comentario: h.comentario,
    })) ?? [];

    setSelected(mapped);
    setObsTab("Carátula");
  }

  useEffect(() => { cargarEstimaciones(); }, []);

  const filtradas = estimaciones
    .filter((e) => filtroEstado === "Todos" || e.estado === filtroEstado)
    .filter((e) =>
      busqueda === "" ||
      String(e.id).includes(busqueda) ||
      e.periodo.toLowerCase().includes(busqueda.toLowerCase())
    );

  // Financiero solo ve las aprobadas por residencia con saldo pendiente
  const visibles = puedePagar
    ? estimaciones.filter((e) => e.estado === "AprobadaResidencia" && e.saldoPendientePago > 0.01)
    : filtradas;

  async function handleEnviar() {
    if (!selected) return;
    await estimacionesService.enviar(selected.id, USUARIO_ID);
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleAprobarSupervision() {
    if (!selected) return;
    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoApi.AprobadaSupervision,
      usuarioId: USUARIO_ID,
      comentario: "Aprobada técnicamente por supervisión.",
    });
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleObservarSupervision() {
    if (!selected) return;
    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoApi.ObservadaSupervision,
      usuarioId: USUARIO_ID,
      comentario: "Marcada como observada. Se requieren correcciones.",
    });
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleAprobarResidencia() {
    if (!selected) return;
    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoApi.AprobadaResidencia,
      usuarioId: USUARIO_ID,
      comentario: "Aprobada por residencia para trámite de pago.",
    });
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handleRechazar() {
    if (!selected || !motivoRechazo.trim()) return;
    await estimacionesService.cambiarEstado(selected.id, {
      nuevoEstado: EstadoApi.RechazadaResidencia,
      usuarioId: USUARIO_ID,
      comentario: motivoRechazo,
    });
    setMotivoRechazo("");
    setShowRechazo(false);
    await cargarEstimaciones();
    await abrirEstimacion(selected.id);
  }

  async function handlePago() {
    if (!selected || !datoPago.referencia || !datoPago.fecha || !datoPago.monto) return;
    const monto = Number(datoPago.monto);
    if (isNaN(monto) || monto <= 0) { alert("El monto debe ser mayor a cero."); return; }
    if (monto > (selected.saldoPendientePago ?? 0) + 0.01) {
      alert(`El monto excede el saldo pendiente (${fmtMXN(selected.saldoPendientePago)}).`);
      return;
    }

    await estimacionesService.registrarPago(selected.id, {
      referenciaBancaria: datoPago.referencia,
      fechaPago: datoPago.fecha,
      montoPagado: monto,
      usuarioRegistroId: USUARIO_ID,
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

  const detailTabs = ["Carátula", "Generadores", "Pagos", "Notas vinculadas", "Historial"];

  if (selected) {
    const totalEst = selected.conceptos?.reduce((a, c) => a + (c.importe || 0), 0) || selected.monto;

    return (
      <div>
        <button
          onClick={() => setSelected(null)}
          style={{ display: "flex", alignItems: "center", gap: 6, fontSize: 12, color: obra, background: "none", border: "none", cursor: "pointer", padding: "0 0 16px 0", fontFamily: "'IBM Plex Sans', sans-serif" }}
        >
          ← Volver a bandeja
        </button>

        <div style={{ background: C.surface2, color: C.fg, borderRadius: "16px 16px 0 0", overflow: "hidden", border: `1px solid ${C.border}`, borderBottom: "none" }}>
          <div style={{ background: folio, display: "flex", justifyContent: "space-between", padding: "5px 20px", fontSize: 10.5, letterSpacing: "0.06em" }}>
            <span>SELLO DIGITAL · FOLIO-EST-{String(selected.id).padStart(3, "0")}</span>
            <span>Plazo legal: Art. 55 LOPSRM — 20 días hábiles</span>
          </div>

          <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "14px 20px" }}>
            <div>
              <div style={{ fontFamily: "'IBM Plex Serif', serif", fontWeight: 700, fontSize: 18 }}>
                Estimación N.° {String(selected.id).padStart(3, "0")}
              </div>
              <div style={{ fontSize: 11, color: "#C7C2B0", marginTop: 2 }}>
                Contrato {CONTRATO_ID} · Periodo {selected.periodo}
              </div>
            </div>

            <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
              <EstadoBadge estado={selected.estado} />
              <EstadoBadge estado={selected.estadoPago} />
              <button
                onClick={() => setShowHistorial(true)}
                style={{ fontSize: 11, color: muted, background: "none", border: `1px solid ${rule}`, borderRadius: 3, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", display: "flex", alignItems: "center", gap: 4 }}
              >
                <Clock size={11} /> Historial
              </button>
            </div>
          </div>
        </div>

        <div style={{ display: "flex", background: C.surface2, borderBottom: `1px solid ${C.border}` }}>
          {detailTabs.map((t) => (
            <button
              key={t}
              onClick={() => setObsTab(t)}
              style={{ padding: "10px 18px", fontSize: 12.5, color: obsTab === t ? obra : muted, border: "none", borderBottom: `2px solid ${obsTab === t ? obra : "transparent"}`, background: "transparent", fontWeight: obsTab === t ? 600 : 400, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}
            >
              {t}
            </button>
          ))}
        </div>

        {obsTab === "Carátula" && (
          <div style={{ display: "grid", gridTemplateColumns: "1fr 290px", border: `1px solid ${C.border}`, borderRadius: "0 0 16px 16px", borderTop: "none", background: C.surface, overflow: "hidden" }}>
            <div style={{ padding: "20px 22px", borderRight: `1px solid ${C.border}` }}>
              <div style={{ display: "flex", gap: 12, marginBottom: 20 }}>
                {[
                  { v: fmtMXN(totalEst), l: "Monto estimado" },
                  { v: fmtMXN(selected.montoPagadoAcumulado), l: "Pagado acumulado" },
                  { v: fmtMXN(selected.saldoPendientePago), l: "Saldo pendiente" },
                  { v: selected.periodo, l: "Periodo" },
                ].map((k) => (
                  <div key={k.l} style={{ flex: 1, border: `1px solid ${C.border}`, borderRadius: 10, padding: "12px 14px", background: C.surface2 }}>
                    <div style={{ fontFamily: "JetBrains Mono", fontSize: 14, fontWeight: 700, color: obra }}>{k.v}</div>
                    <div style={{ fontSize: 10, textTransform: "uppercase", letterSpacing: "0.04em", color: muted, marginTop: 2 }}>{k.l}</div>
                  </div>
                ))}
              </div>

              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
                <TableHeader cols={["Clave", "Descripción", "Unidad", "Cant.", "P.U.", "Importe"]} />
                <tbody>
                  {(selected.conceptos || []).map((r) => (
                    <tr key={r.clave}>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{r.clave}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}` }}>{r.desc}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{r.unidad}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{r.cantEjecutada}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{fmtMXN(r.precioUnitario)}</td>
                      <td style={{ padding: "8px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, fontWeight: 600 }}>{fmtMXN(r.importe)}</td>
                    </tr>
                  ))}
                  {(selected.conceptos || []).length === 0 && (
                    <tr><td colSpan={6} style={{ padding: "20px 14px", color: muted, textAlign: "center" }}>Sin conceptos registrados</td></tr>
                  )}
                </tbody>
              </table>

              <div style={{ marginTop: 16, padding: "12px 14px", background: C.surface2, border: `1px solid ${rule}`, borderRadius: 3 }}>
                <div style={{ fontSize: 10.5, textTransform: "uppercase", letterSpacing: "0.05em", color: muted, marginBottom: 8 }}>Total estimado</div>
                <div style={{ fontFamily: "'IBM Plex Serif', serif", fontSize: 20, fontWeight: 700, color: obra }}>{fmtMXN(totalEst)}</div>
              </div>

              <div style={{ display: "flex", gap: 10, marginTop: 20, flexWrap: "wrap" }}>
                {/* Superintendente: Borrador → enviar */}
                {esBorrador && puedeEnviar && (
                  <>
                    <PrimaryBtn onClick={() => setShowAddConcepto(true)}>+ Añadir concepto</PrimaryBtn>
                    <PrimaryBtn onClick={handleEnviar} style={{ background: folio, marginLeft: "auto" }}>Enviar estimación →</PrimaryBtn>
                  </>
                )}

                {/* Superintendente: ObservadaSupervision → corregir y re-enviar */}
                {esObservadaSupervision && puedeEnviar && (
                  <PrimaryBtn onClick={handleEnviar} style={{ background: obra }}>↩ Corregir y re-enviar</PrimaryBtn>
                )}

                {/* Supervisor: Enviada → observar o aprobar por supervisión */}
                {esEnviada && puedeGestionarSupervision && (
                  <>
                    <SecondaryBtn onClick={() => setShowObservacion(true)}>+ Nueva observación</SecondaryBtn>
                    <SecondaryBtn onClick={handleObservarSupervision} color={observado}>Marcar observada</SecondaryBtn>
                    <PrimaryBtn onClick={handleAprobarSupervision} style={{ background: aprobado }}>✓ Aprobar (supervisión)</PrimaryBtn>
                  </>
                )}

                {/* Residente: AprobadaSupervision → aprobar final o rechazar */}
                {esAprobadaSupervision && puedeGestionarResidencia && (
                  <>
                    <SecondaryBtn onClick={() => setShowObservacion(true)}>+ Nueva observación</SecondaryBtn>
                    <DangerBtn onClick={() => setShowRechazo(true)}>Rechazar</DangerBtn>
                    <PrimaryBtn onClick={handleAprobarResidencia} style={{ background: aprobado }}>✓ Aprobar (residencia)</PrimaryBtn>
                  </>
                )}

                {/* Financiero: AprobadaResidencia con saldo → registrar pago */}
                {esAprobadaResidencia && puedePagar && tieneSaldoPendiente && (
                  <PrimaryBtn onClick={() => setShowPago(true)} style={{ background: pagado }}>
                    <DollarSign size={13} style={{ marginRight: 4 }} /> Registrar pago
                  </PrimaryBtn>
                )}
              </div>
            </div>

            {/* Panel de observaciones */}
            <div style={{ padding: "20px 18px", background: C.surface2 }}>
              <div style={{ fontSize: 11.5, fontWeight: 700, color: obra, textTransform: "uppercase", letterSpacing: "0.06em", marginBottom: 14 }}>
                Observaciones ({(selected.observaciones || []).length})
              </div>

              {(selected.observaciones || []).length === 0 && (
                <div style={{ fontSize: 12, color: muted, textAlign: "center", padding: "20px 0" }}>Sin observaciones registradas</div>
              )}

              {(selected.observaciones || []).map((n, i) => (
                <div key={i} style={{ background: C.surface, border: `1px solid ${C.border}`, borderLeft: `3px solid ${n.color}`, borderRadius: 10, padding: "10px 12px", marginBottom: 10 }}>
                  <div style={{ fontFamily: "JetBrains Mono", fontSize: 10.5, color: n.color, fontWeight: 700, marginBottom: 4 }}>{n.ref}</div>
                  <div style={{ fontSize: 12.5, color: C.fg }}>{n.txt}</div>
                  <div style={{ fontSize: 10.5, color: muted, marginTop: 6 }}>{n.quien} · {n.hora}</div>
                </div>
              ))}

              {(puedeGestionarSupervision && esEnviada) || (puedeGestionarResidencia && esAprobadaSupervision) ? (
                <button
                  onClick={() => setShowObservacion(true)}
                  style={{ width: "100%", background: C.surface, border: `1px dashed ${C.borderHi}`, borderRadius: 10, padding: "8px 12px", fontSize: 12, color: muted, cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif", marginTop: 8 }}
                >
                  + Nueva observación
                </button>
              ) : null}
            </div>
          </div>
        )}

        {obsTab === "Generadores" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 16px 16px" }}>
            <SectionLabel>Generadores de cantidad</SectionLabel>
            {(selected.conceptos || []).map((c) => (
              <div key={c.clave} style={{ border: `1px solid ${rule}`, borderRadius: 3, padding: "12px 16px", marginBottom: 10 }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 6 }}>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra, fontWeight: 600 }}>{c.clave}</span>
                  <span style={{ fontSize: 12.5 }}>{c.desc}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, color: C.fgMuted }}>{c.cantEjecutada} {c.unidad}</span>
                  <span style={{ fontFamily: "JetBrains Mono", fontSize: 12, fontWeight: 600 }}>{fmtMXN(c.importe)}</span>
                </div>
                <div style={{ background: C.surface2, border: `1px dashed ${rule}`, borderRadius: 2, padding: "8px 12px", fontSize: 12, color: muted }}>
                  Generador registrado con base en cantidades ejecutadas.
                </div>
              </div>
            ))}
          </Card>
        )}

        {obsTab === "Pagos" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 16px 16px" }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 16 }}>
              <SectionLabel>Historial de pagos</SectionLabel>
              <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
                <EstadoBadge estado={selected.estadoPago} />
                <span style={{ fontSize: 12, color: muted }}>
                  Saldo: <strong style={{ color: selected.saldoPendientePago > 0 ? observado : aprobado }}>{fmtMXN(selected.saldoPendientePago)}</strong>
                </span>
              </div>
            </div>

            {/* Barra de progreso de pago */}
            <div style={{ marginBottom: 16 }}>
              <div style={{ display: "flex", justifyContent: "space-between", fontSize: 11, color: muted, marginBottom: 4 }}>
                <span>{fmtMXN(selected.montoPagadoAcumulado)} pagado</span>
                <span>{fmtMXN(selected.monto)} total</span>
              </div>
              <div style={{ background: rule, borderRadius: 4, height: 8, overflow: "hidden" }}>
                <div style={{ background: pagado, height: "100%", width: `${selected.monto > 0 ? Math.min((selected.montoPagadoAcumulado / selected.monto) * 100, 100) : 0}%`, borderRadius: 4, transition: "width 0.3s ease" }} />
              </div>
            </div>

            {(selected.pagos || []).length === 0 ? (
              <div style={{ color: muted, fontSize: 12, textAlign: "center", padding: 20, border: `1px dashed ${C.borderHi}`, borderRadius: 10 }}>
                Sin pagos registrados aún.
              </div>
            ) : (
              <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
                <TableHeader cols={["Referencia", "Fecha pago", "Monto", "Registro"]} />
                <tbody>
                  {(selected.pagos || []).map((p, i) => (
                    <tr key={p.id} style={{ background: i % 2 === 1 ? C.surface2 : "transparent" }}>
                      <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: obra }}>{p.referenciaBancaria}</td>
                      <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{p.fechaPago}</td>
                      <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, fontWeight: 600, color: pagado }}>{fmtMXN(p.montoPagado)}</td>
                      <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 11, color: muted }}>{new Date(p.fechaRegistro).toLocaleDateString("es-MX")}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            {esAprobadaResidencia && puedePagar && tieneSaldoPendiente && (
              <div style={{ marginTop: 16 }}>
                <PrimaryBtn onClick={() => setShowPago(true)} style={{ background: pagado }}>
                  <DollarSign size={13} style={{ marginRight: 4 }} /> Registrar pago parcial / final
                </PrimaryBtn>
              </div>
            )}
          </Card>
        )}

        {obsTab === "Notas vinculadas" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 16px 16px" }}>
            <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: 16 }}>
              <SectionLabel>Notas de bitácora vinculadas</SectionLabel>
              {esBorrador && <SecondaryBtn onClick={abrirModalVincular}>+ Vincular notas</SecondaryBtn>}
            </div>

            {(selected.notasVinculadas ?? []).length === 0 ? (
              <div style={{ color: muted, fontSize: 12, textAlign: "center", padding: 20, border: `1px dashed ${C.borderHi}`, borderRadius: 10 }}>
                Sin notas de bitácora vinculadas.
                {esBorrador && " Usa \"+ Vincular notas\" para agregar soporte documental."}
              </div>
            ) : (
              <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                {selected.notasVinculadas.map((resumen, i) => (
                  <div key={i} style={{ border: `1px solid ${C.border}`, borderLeft: `3px solid ${obra}`, borderRadius: 10, padding: "10px 14px", fontSize: 12.5, background: C.surface2, fontFamily: "JetBrains Mono", color: C.fg }}>
                    {resumen}
                  </div>
                ))}
              </div>
            )}
          </Card>
        )}

        {obsTab === "Historial" && (
          <Card style={{ padding: 24, borderTop: "none", borderRadius: "0 0 16px 16px" }}>
            <SectionLabel>Historial de cambios</SectionLabel>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
              <TableHeader cols={["Estado anterior", "Estado nuevo", "Fecha y hora", "Usuario", "Comentario"]} />
              <tbody>
                {(selected.historial || []).map((h, i) => (
                  <tr key={i} style={{ background: i % 2 === 1 ? C.surface2 : "transparent" }}>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{h.estadoAnterior}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                      <EstadoBadge estado={h.estadoNuevo} />
                    </td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{h.fecha}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{h.usuario}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 11.5, color: muted }}>{h.comentario ?? "—"}</td>
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
                  conceptoContratoId: (concepto as any).conceptoContratoId ?? (concepto as any).id ?? 1,
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
          <Modal title="Nueva observación" onClose={() => setShowObservacion(false)} width={420}>
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Observación</SectionLabel>
              <TextArea placeholder="Describe la observación..." value={nuevaObs} onChange={setNuevaObs} rows={4} />
            </div>
            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowObservacion(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <PrimaryBtn onClick={handleNuevaObs} style={{ flex: 1 }}>Registrar</PrimaryBtn>
            </div>
          </Modal>
        )}

        {showRechazo && (
          <Modal title="Rechazar estimación" onClose={() => setShowRechazo(false)} width={420}>
            <div style={{ background: C.redSoft, border: `1px solid ${C.red}44`, borderRadius: 10, padding: "10px 14px", marginBottom: 16, fontSize: 12.5, color: folio }}>
              Se requiere un motivo antes de confirmar el rechazo.
            </div>
            <div style={{ marginBottom: 20 }}>
              <SectionLabel>Motivo de rechazo</SectionLabel>
              <TextArea placeholder="Describe el motivo..." value={motivoRechazo} onChange={setMotivoRechazo} rows={4} />
            </div>
            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowRechazo(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <DangerBtn onClick={handleRechazar} style={{ flex: 1 }}>Confirmar rechazo</DangerBtn>
            </div>
          </Modal>
        )}

        {showPago && (
          <Modal
            title="Registrar pago"
            subtitle={"Estimación N.° " + String(selected.id).padStart(3, "0") + " · Saldo: " + fmtMXN(selected.saldoPendientePago)}
            onClose={() => setShowPago(false)}
            width={440}
          >
            <div style={{ display: "grid", gap: 14, marginBottom: 20 }}>
              <div>
                <SectionLabel>Referencia bancaria / CLC</SectionLabel>
                <TextInput placeholder="Ej: CLC-2026-08741" value={datoPago.referencia} onChange={(v) => setDatoPago((p) => ({ ...p, referencia: v }))} />
              </div>
              <div>
                <SectionLabel>Fecha de pago</SectionLabel>
                <TextInput placeholder="YYYY-MM-DD" value={datoPago.fecha} onChange={(v) => setDatoPago((p) => ({ ...p, fecha: v }))} />
              </div>
              <div>
                <SectionLabel>Monto pagado (máx. {fmtMXN(selected.saldoPendientePago)})</SectionLabel>
                <TextInput placeholder="0.00" value={datoPago.monto} onChange={(v) => setDatoPago((p) => ({ ...p, monto: v }))} />
              </div>
            </div>

            <div style={{ background: C.redSoft, border: `1px solid ${C.red}44`, borderRadius: 10, padding: "10px 14px", marginBottom: 16, fontSize: 12, color: folio }}>
              Se admiten pagos parciales. Si el monto cubre el total estimado, la estimación pasará a <strong>Pagada</strong>.
            </div>

            <div style={{ display: "flex", gap: 10 }}>
              <SecondaryBtn onClick={() => setShowPago(false)} style={{ flex: 1 }}>Cancelar</SecondaryBtn>
              <PrimaryBtn
                onClick={handlePago}
                style={{ flex: 1, background: pagado }}
                disabled={!datoPago.referencia || !datoPago.fecha || !datoPago.monto || Number(datoPago.monto) <= 0}
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
          <Modal title="Historial de estados" onClose={() => setShowHistorial(false)} width={620}>
            <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
              <TableHeader cols={["Estado anterior", "Estado nuevo", "Fecha y hora", "Comentario"]} />
              <tbody>
                {(selected.historial || []).map((h, i) => (
                  <tr key={i} style={{ background: i % 2 === 1 ? C.surface2 : "transparent" }}>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{h.estadoAnterior}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}><EstadoBadge estado={h.estadoNuevo} /></td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5 }}>{h.fecha}</td>
                    <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontSize: 11.5, color: muted }}>{h.comentario ?? "—"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Modal>
        )}
      </div>
    );
  }

  // ─── Vista de bandeja ───────────────────────────────────────────────────────
  return (
    <div>
      <div style={{ display: "flex", alignItems: "center", gap: 12, marginBottom: 20, flexWrap: "wrap" }}>
        {puedeEnviar && (
          <PrimaryBtn onClick={() => setShowNuevaEst(true)}>+ Nueva estimación</PrimaryBtn>
        )}

        <div style={{ position: "relative" }}>
          <Search size={13} style={{ position: "absolute", left: 10, top: "50%", transform: "translateY(-50%)", color: muted }} />
          <input
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
            placeholder="Buscar folio o periodo..."
            style={{ border: `1px solid ${C.border}`, background: C.surface2, borderRadius: 10, padding: "8px 12px 8px 30px", fontSize: 12.5, color: C.fg, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", width: 220, colorScheme: "dark" }}
          />
        </div>

        {!puedePagar && (
          <select
            value={filtroEstado}
            onChange={(e) => setFiltroEstado(e.target.value)}
            style={{ border: `1px solid ${C.border}`, background: C.surface2, borderRadius: 10, padding: "8px 12px", fontSize: 12, color: C.fg, fontFamily: "'IBM Plex Sans', sans-serif", outline: "none", colorScheme: "dark" }}
          >
            <option value="Todos">Todos</option>
            <option value="Borrador">Borrador</option>
            <option value="Enviada">Enviada</option>
            <option value="ObservadaSupervision">Observada (supervisión)</option>
            <option value="AprobadaSupervision">Aprobada (supervisión)</option>
            <option value="RechazadaResidencia">Rechazada</option>
            <option value="AprobadaResidencia">Aprobada (residencia)</option>
          </select>
        )}

        {puedePagar && (
          <div style={{ fontSize: 11.5, color: muted }}>
            Mostrando estimaciones <strong>Aprobadas por residencia</strong> con saldo pendiente de pago.
          </div>
        )}
      </div>

      <Card style={{ overflow: "hidden" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 12.5 }}>
          <TableHeader cols={["N.°", "Periodo", "Monto estimado", "Estado técnico", "Estado pago", "Saldo", ""]} />
          <tbody>
            {visibles.map((e, i) => (
              <tr
                key={e.id}
                style={{ background: "transparent" }}
                onMouseEnter={(el) => (el.currentTarget.style.background = C.surface2)}
                onMouseLeave={(el) => (el.currentTarget.style.background = "transparent")}
              >
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontWeight: 600, color: obra }}>
                  {String(e.id).padStart(3, "0")}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>{e.periodo}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono" }}>{fmtMXN(e.monto)}</td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <EstadoBadge estado={e.estado} />
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <EstadoBadge estado={e.estadoPago} />
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}`, fontFamily: "JetBrains Mono", fontSize: 11.5, color: e.saldoPendientePago > 0 ? observado : muted }}>
                  {fmtMXN(e.saldoPendientePago)}
                </td>
                <td style={{ padding: "10px 14px", borderBottom: `1px solid ${rule}` }}>
                  <button
                    onClick={() => abrirEstimacion(e.id)}
                    style={{ fontSize: 11.5, color: obra, background: C.blueSoft, border: "none", borderRadius: 8, padding: "4px 10px", cursor: "pointer", fontFamily: "'IBM Plex Sans', sans-serif" }}
                  >
                    Ver →
                  </button>
                </td>
              </tr>
            ))}
            {visibles.length === 0 && (
              <tr><td colSpan={7} style={{ padding: "24px 14px", color: muted, textAlign: "center" }}>Sin estimaciones que mostrar.</td></tr>
            )}
          </tbody>
        </table>
      </Card>

      {showNuevaEst && (
        <ModalNuevaEstimacion
          contrato={{ id: CONTRATO_ID, numeroContrato: `Contrato ${CONTRATO_ID}` }}
          onClose={() => setShowNuevaEst(false)}
          onCrear={async (data) => {
            try {
              await estimacionesService.crear({ contratoId: CONTRATO_ID, periodo: data.periodo });
              setShowNuevaEst(false);
              await cargarEstimaciones();
            } catch (err: any) {
              console.error("Error al crear estimación:", err);
              alert("No se pudo crear la estimación: " + (err?.message ?? "Error desconocido"));
            }
          }}
        />
      )}
    </div>
  );
}
