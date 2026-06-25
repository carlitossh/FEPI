// ─── Dark palette ──────────────────────────────────────────────────────────────
export const C = {
  bg:        "#0e0e0e",
  surface:   "#1a1a1a",
  surface2:  "#222222",
  border:    "rgba(255,255,255,0.07)",
  borderHi:  "rgba(255,255,255,0.14)",
  fg:        "#f5f5f5",
  fgMuted:   "#888888",
  fgSub:     "#555555",
  blue:      "#4f86f7",
  blueSoft:  "rgba(79,134,247,0.15)",
  red:       "#f05252",
  redSoft:   "rgba(240,82,82,0.15)",
  amber:     "#f59e0b",
  amberSoft: "rgba(245,158,11,0.15)",
  green:     "#22c55e",
  greenSoft: "rgba(34,197,94,0.15)",
  purple:    "#a855f7",
};

// ─── Legacy aliases (mantienen compatibilidad con imports existentes) ──────────
export const ink          = C.fg;
export const paper        = C.bg;
export const paper2       = C.surface;
export const rule         = C.border;
export const folio        = C.red;
export const folioSoft    = C.redSoft;
export const obra         = C.blue;
export const obraSoft     = C.blueSoft;
export const aprobado     = C.green;
export const aprobadoSoft = C.greenSoft;
export const observado    = C.amber;
export const observadoSoft = C.amberSoft;
export const muted        = C.fgMuted;
export const pagado       = C.green;
export const pagadoSoft   = C.greenSoft;
