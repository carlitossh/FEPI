import {
  obraSoft, obra, observadoSoft, observado, aprobadoSoft, aprobado,
  pagadoSoft, pagado, folioSoft, folio,
} from "../styles/theme";

interface EstadoBadgeProps {
  estado: string;
}

export function EstadoBadge({ estado }: EstadoBadgeProps) {
  const map: Record<string, { bg: string; color: string }> = {
    Borrador: { bg: "#E8ECF2", color: "#4A5568" },
    Enviada: { bg: obraSoft, color: obra },
    Observada: { bg: observadoSoft, color: observado },
    Aprobada: { bg: aprobadoSoft, color: aprobado },
    Pagada: { bg: pagadoSoft, color: pagado },
    "En evaluación": { bg: observadoSoft, color: observado },
    Dictaminada: { bg: obraSoft, color: obra },
    Promovida: { bg: observadoSoft, color: "#7A5A00" },
    Aprobado: { bg: aprobadoSoft, color: aprobado },
    Rechazado: { bg: folioSoft, color: folio },
    vigente: { bg: aprobadoSoft, color: aprobado },
    "por vencer": { bg: observadoSoft, color: observado },
    vencida: { bg: folioSoft, color: folio },
  };
  const s = map[estado] ?? { bg: "#eee", color: "#666" };
  return (
    <span
      style={{
        background: s.bg,
        color: s.color,
        fontSize: 10.5,
        fontWeight: 600,
        padding: "3px 9px",
        borderRadius: 11,
        fontFamily: "'IBM Plex Sans', sans-serif",
        letterSpacing: "0.02em",
        whiteSpace: "nowrap",
      }}
    >
      {estado}
    </span>
  );
}
