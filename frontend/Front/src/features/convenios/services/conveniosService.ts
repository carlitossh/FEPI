const API_BASE = "/api";

export const conveniosService = {
  getConvenios: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/convenios`),

  crearConvenio: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/convenios`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  dictaminarConvenio: (id: string, data: unknown) =>
    fetch(`${API_BASE}/convenios/${id}/dictamen`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  promoverConvenio: (id: string) =>
    fetch(`${API_BASE}/convenios/${id}/promover`, { method: "POST" }),

  resolverConvenio: (id: string, data: unknown) =>
    fetch(`${API_BASE}/convenios/${id}/resolucion`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
