export function contarFirmas(firmas: Record<string, boolean>): string {
  const vals = Object.values(firmas);
  return `${vals.filter(Boolean).length}/${vals.length}`;
}
