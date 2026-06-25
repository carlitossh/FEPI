const API = "http://localhost:5000/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API}${url}`, options);

  if (!response.ok) {
    throw new Error(await response.text());
  }

  if (response.status === 204) {
    return null as T;
  }

  return response.json();
}

export const estimacionesService = {
  listarPorContrato: (contratoId: number) =>
    request<any[]>(`/estimaciones/contrato/${contratoId}`),

  obtenerDetalle: (id: number) =>
    request<any>(`/estimaciones/${id}`),

  crear: (data: { contratoId: number; periodo: string }) =>
    request<any>(`/estimaciones`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  actualizarConceptos: (
    id: number,
    conceptos: { conceptoContratoId: number; cantidadEjecutada: number }[]
  ) =>
    request<void>(`/estimaciones/${id}/conceptos`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ conceptos }),
    }),

  enviar: (id: number, usuarioId: number) =>
    request<void>(`/estimaciones/${id}/enviar?usuarioId=${usuarioId}`, {
      method: "POST",
    }),

  cambiarEstado: (
    id: number,
    data: { nuevoEstado: number; usuarioId: number; comentario?: string }
  ) =>
    request<void>(`/estimaciones/${id}/estado`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  agregarObservacion: (id: number, texto: string, usuarioId: number) =>
    request<any>(`/estimaciones/${id}/observaciones?usuarioId=${usuarioId}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ texto }),
    }),

  registrarPago: (
    id: number,
    data: { fechaPago: string; referenciaBancaria: string; montoPagado: number }
  ) =>
    request<void>(`/estimaciones/${id}/pago`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  obtenerHistorial: (id: number) =>
    request<any[]>(`/estimaciones/${id}/historial`),

  getConceptosContrato: (contratoId: number) =>
    request<any[]>(`/contratos/${contratoId}/conceptos`),

  vincularNotasBitacora: (estimacionId: number, notaBitacoraIds: number[]) =>
    request<void>(`/estimaciones/${estimacionId}/notas-bitacora`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ notaBitacoraIds }),
    }),
};