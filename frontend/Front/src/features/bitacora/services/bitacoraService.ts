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

export const bitacoraService = {
  abrirBitacora: (data: any) =>
    request<any>("/bitacora/abrir", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  buscarNotas: (bitacoraId: number) =>
    request<any[]>(`/bitacora/${bitacoraId}/notas`),

  crearNota: (data: any) =>
    request<any>("/bitacora/notas", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

firmarNota: (notaId: number, data: any) =>
  request<void>(`/bitacora/notas/${notaId}/firmar`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  }),

  crearMinuta: (data: any) =>
    request<void>("/bitacora/minutas", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  crearIncidencia: (data: any) =>
    request<number>("/bitacora/incidencias", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  generarNotaDesdeIncidencia: (data: any) =>
    request<any>("/bitacora/incidencias/generar-nota", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};