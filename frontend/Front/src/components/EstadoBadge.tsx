import { C } from "../styles/theme";

interface EstadoBadgeProps {
  estado: string;
}

const labelMap: Record<string, string> = {
  ObservadaSupervision: "Obs. supervisión",
  AprobadaSupervision:  "Apr. supervisión",
  RechazadaResidencia:  "Rechazada",
  AprobadaResidencia:   "Apr. residencia",
  SinPago:              "Sin pago",
  PagoParcial:          "Pago parcial",
};

export function EstadoBadge({ estado }: EstadoBadgeProps) {
  const map: Record<string, { bg: string; color: string }> = {
    // Estados técnicos de estimación
    Borrador:             { bg: "rgba(136,136,136,0.12)", color: C.fgMuted },
    Enviada:              { bg: C.blueSoft,  color: C.blue  },
    ObservadaSupervision: { bg: C.amberSoft, color: C.amber },
    AprobadaSupervision:  { bg: C.greenSoft, color: C.green },
    RechazadaResidencia:  { bg: C.redSoft,   color: C.red   },
    AprobadaResidencia:   { bg: C.greenSoft, color: C.green },
    Cancelada:            { bg: C.redSoft,   color: C.red   },
    // Estados de pago
    SinPago:    { bg: "rgba(136,136,136,0.12)", color: C.fgMuted },
    PagoParcial:{ bg: C.amberSoft, color: C.amber },
    Pagada:     { bg: C.greenSoft, color: C.green },
    // Estados de convenios
    "En evaluación": { bg: C.amberSoft, color: C.amber },
    Dictaminada:     { bg: C.blueSoft,  color: C.blue  },
    Promovida:       { bg: C.amberSoft, color: C.amber },
    Aprobado:        { bg: C.greenSoft, color: C.green },
    Rechazado:       { bg: C.redSoft,   color: C.red   },
    // Estados de garantías
    vigente:     { bg: C.greenSoft, color: C.green },
    "por vencer":{ bg: C.amberSoft, color: C.amber },
    vencida:     { bg: C.redSoft,   color: C.red   },
    // Tipos de bitácora
    Apertura:   { bg: C.blueSoft,            color: C.blue  },
    Nota:       { bg: "rgba(168,85,247,0.15)", color: C.purple },
    Minuta:     { bg: C.amberSoft,            color: C.amber },
    Incidencia: { bg: C.redSoft,              color: C.red   },
  };

  const s = map[estado] ?? { bg: "rgba(136,136,136,0.12)", color: C.fgMuted };
  const label = labelMap[estado] ?? estado;

  return (
    <span
      style={{
        display: "inline-flex",
        alignItems: "center",
        gap: 5,
        background: s.bg,
        color: s.color,
        fontSize: 11,
        fontWeight: 600,
        padding: "3px 10px",
        borderRadius: 999,
        letterSpacing: "0.01em",
        whiteSpace: "nowrap",
      }}
    >
      <span
        style={{
          width: 5,
          height: 5,
          borderRadius: "50%",
          background: s.color,
          flexShrink: 0,
        }}
      />
      {label}
    </span>
  );
}
