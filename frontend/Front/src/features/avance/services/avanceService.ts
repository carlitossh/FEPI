const API = "http://localhost:5000/api";

export async function getCurvaS(contratoId: number) {
  const response = await fetch(`${API}/avance/contrato/${contratoId}/curva-s`);

  if (!response.ok) {
    throw new Error("No se pudo obtener la Curva S");
  }

  return response.json();
}

export async function registrarAvance(data: unknown) {
  const response = await fetch(`${API}/avance`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    throw new Error("No se pudo registrar el avance");
  }
}

export async function getConceptosContrato(contratoId: number) {
  const response = await fetch(`${API}/contratos/${contratoId}/conceptos`);

  if (!response.ok) {
    throw new Error("No se pudieron obtener los conceptos");
  }

  return response.json();
}