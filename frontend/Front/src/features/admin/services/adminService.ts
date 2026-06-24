const API_BASE = "/api";

export const adminService = {
  getUsuarios: (contratoId: string) =>
    fetch(`${API_BASE}/contratos/${contratoId}/usuarios`),

  invitarUsuario: (contratoId: string, data: unknown) =>
    fetch(`${API_BASE}/contratos/${contratoId}/usuarios`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  actualizarRol: (usuarioId: string, rol: string) =>
    fetch(`${API_BASE}/usuarios/${usuarioId}/rol`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ rol }),
    }),

  suspenderUsuario: (usuarioId: string) =>
    fetch(`${API_BASE}/usuarios/${usuarioId}/suspender`, { method: "POST" }),

  activarUsuario: (usuarioId: string) =>
    fetch(`${API_BASE}/usuarios/${usuarioId}/activar`, { method: "POST" }),
};
