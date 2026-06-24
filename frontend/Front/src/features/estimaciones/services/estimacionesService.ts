const API_BASE = "/api";

export const estimacionesService = {
  getEstimaciones: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/estimaciones`),

  getEstimacion: (id: number) =>
    fetch(`${API_BASE}/estimaciones/${id}`),

  crearEstimacion: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/estimaciones`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  actualizarEstimacion: (id: number, data: unknown) =>
    fetch(`${API_BASE}/estimaciones/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  enviarEstimacion: (id: number) =>
    fetch(`${API_BASE}/estimaciones/${id}/enviar`, { method: "POST" }),

  aprobarEstimacion: (id: number) =>
    fetch(`${API_BASE}/estimaciones/${id}/aprobar`, { method: "POST" }),

  rechazarEstimacion: (id: number, motivo: string) =>
    fetch(`${API_BASE}/estimaciones/${id}/rechazar`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ motivo }),
    }),

  observarEstimacion: (id: number) =>
    fetch(`${API_BASE}/estimaciones/${id}/observar`, { method: "POST" }),

  registrarPago: (id: number, data: unknown) =>
    fetch(`${API_BASE}/estimaciones/${id}/pago`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  addObservacion: (id: number, data: unknown) =>
    fetch(`${API_BASE}/estimaciones/${id}/observaciones`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  addConcepto: (id: number, data: unknown) =>
    fetch(`${API_BASE}/estimaciones/${id}/conceptos`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  getHistorial: (id: number) =>
    fetch(`${API_BASE}/estimaciones/${id}/historial`),

  adjuntarDoc: (id: number, formData: FormData) =>
    fetch(`${API_BASE}/estimaciones/${id}/documentos`, {
      method: "POST",
      body: formData,
    }),

  vincularBitacora: (id: number, folios: string[]) =>
    fetch(`${API_BASE}/estimaciones/${id}/bitacora`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ folios }),
    }),
};
