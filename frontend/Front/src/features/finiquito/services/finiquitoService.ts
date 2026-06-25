const API = "http://localhost:5000/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API}${url}`, options);
  if (!res.ok) throw new Error(await res.text());
  if (res.status === 204) return null as T;
  return res.json();
}

export const finiquitoService = {
  calcularFiniquito: (contratoId: number) =>
    request<any>(`/contratos/${contratoId}/finiquito`),

  registrarCierre: (contratoId: number, data: unknown) =>
    request<void>(`/contratos/${contratoId}/cierre`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
