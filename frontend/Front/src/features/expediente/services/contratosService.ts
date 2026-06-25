const API = "http://localhost:5000/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const r = await fetch(`${API}${url}`, options);
  if (!r.ok) throw new Error(await r.text());
  if (r.status === 204) return null as T;
  return r.json();
}

export const contratosService = {
  listar: () => request<any[]>("/contratos"),

  obtener: (id: number) => request<any>(`/contratos/${id}`),

  crear: (data: {
    numeroContrato: string;
    nombreObra: string;
    tipo: number;
    periodoEstimacion: number;
    dependenciaContratante: string;
    contratistaEmpresa: string;
    contratistaRepresentante: string;
    residenteNombre: string;
    supervisorExternoNombre: string;
    superintendenteNombre: string;
    montoContratado: number;
    fechaInicio: string;
    fechaTermino: string;
    conceptoContratos: never[];
    programaObra: never[];
    garantias: never[];
  }) =>
    request<number>("/contratos", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),

  actualizar: (
    id: number,
    data: {
      numeroContrato: string;
      nombreObra: string;
      tipo: number;
      periodoEstimacion: number;
      dependenciaContratante: string;
      contratistaEmpresa: string;
      contratistaRepresentante: string;
      residenteNombre: string;
      supervisorExternoNombre: string;
      superintendenteNombre: string;
      montoContratado: number;
      fechaInicio: string;
      fechaTermino: string;
    }
  ) =>
    request<void>(`/contratos/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }),
};
