const API = "http://localhost:5000/api";

export async function getContrato(id: number) {
  const response = await fetch(`${API}/contratos/${id}`);

  if (!response.ok) {
    throw new Error("No se pudo obtener el contrato");
  }

  return response.json();
}

export async function getCurvaS(id: number) {
  const response = await fetch(`${API}/avance/contrato/${id}/curva-s`);

  if (!response.ok) {
    throw new Error("No se pudo obtener la Curva S");
  }

  return response.json();
}