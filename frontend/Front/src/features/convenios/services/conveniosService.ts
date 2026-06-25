const API = "http://localhost:5000/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API}${url}`, options);
  if (!res.ok) throw new Error(await res.text());
  if (res.status === 204) return null as T;
  return res.json();
}

export const conveniosService = {
  getConvenios: (contratoId: number) =>
    request<any[]>(`/contratos/${contratoId}/convenios`),

  getConvenio: (id: number) =>
    request<any>(`/convenios/${id}`),

  crearConvenio: (contratoId: number, data: unknown) =>
    request<number>(`/contratos/${contratoId}/convenios`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  dictaminarConvenio: (id: number, data: unknown) =>
    request<void>(`/convenios/${id}/dictamen`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  promoverConvenio: (id: number, data: unknown) =>
    request<void>(`/convenios/${id}/promover`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  resolverConvenio: (id: number, data: unknown) =>
    request<void>(`/convenios/${id}/resolucion`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
