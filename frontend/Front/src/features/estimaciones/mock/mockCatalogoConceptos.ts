import type { ConceptoCatalogo } from "../types";

export const mockCatalogoConceptos: ConceptoCatalogo[] = [
  { clave: "01.001", desc: "Limpieza de terreno", unidad: "m²", precioUnitario: 45 },
  { clave: "02.001", desc: "Excavación manual", unidad: "m³", precioUnitario: 320 },
  { clave: "03.012", desc: "Losa de cimentación f'c=250", unidad: "m³", precioUnitario: 4050 },
  { clave: "05.004", desc: "Trabe de liga eje B-C", unidad: "ml", precioUnitario: 6185 },
  { clave: "06.001", desc: "Castillo K-1", unidad: "pza", precioUnitario: 5300 },
  { clave: "07.002", desc: "Muro de block hueco 15cm", unidad: "m²", precioUnitario: 1903 },
];
