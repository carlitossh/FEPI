const API_BASE = "/api";

export const dashboardService = {
  getContrato: (id: string) => fetch(`${API_BASE}/contratos/${id}`),
  getAlertas: (contratoId: string) => fetch(`${API_BASE}/contratos/${contratoId}/alertas`),
};
