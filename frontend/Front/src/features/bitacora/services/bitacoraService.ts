const API_BASE = "/api";

export const bitacoraService = {
  getBitacora: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/bitacora`),

  crearNota: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/bitacora/notas`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  crearMinuta: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/bitacora/minutas`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  crearIncidencia: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/bitacora/incidencias`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  firmarNota: (id: number, actor: string) =>
    fetch(`${API_BASE}/bitacora/notas/${id}/firmar`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ actor }),
    }),

  abrirBitacora: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/bitacora/abrir`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
