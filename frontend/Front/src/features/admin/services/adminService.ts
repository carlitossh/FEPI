const API = "http://localhost:5000/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API}${url}`, options);
  if (!res.ok) throw new Error(await res.text());
  if (res.status === 204) return null as T;
  return res.json();
}

export const adminService = {
  getUsuarios: (contratoId: number) =>
    request<any[]>(`/contratos/${contratoId}/usuarios`),

  invitarUsuario: (contratoId: number, data: unknown) =>
    request<any>(`/contratos/${contratoId}/usuarios`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  actualizarRol: (usuarioId: number, contratoId: number, rol: number) =>
    request<void>(`/usuarios/${usuarioId}/rol`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ contratoId, rol }),
    }),

  suspenderUsuario: (usuarioId: number) =>
    request<void>(`/usuarios/${usuarioId}/suspender`, { method: "POST" }),

  activarUsuario: (usuarioId: number) =>
    request<void>(`/usuarios/${usuarioId}/activar`, { method: "POST" }),
};
