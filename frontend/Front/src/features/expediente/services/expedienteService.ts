const API_BASE = "/api";

export const expedienteService = {
  getContrato: (id: string) => fetch(`${API_BASE}/contratos/${id}`),
};
