const API_BASE = "/api";

export const finiquitoService = {
  calcularFiniquito: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/finiquito`),

  registrarCierre: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/cierre`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
