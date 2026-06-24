const API_BASE = "/api";

export const avanceService = {
  getAvance: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/avance`),

  registrarAvance: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/avance`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
