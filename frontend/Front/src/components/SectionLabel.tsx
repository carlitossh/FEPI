import { muted } from "../styles/theme";

interface SectionLabelProps {
  children: React.ReactNode;
}

export function SectionLabel({ children }: SectionLabelProps) {
  return (
    <div
      style={{
        fontSize: 10.5,
        textTransform: "uppercase",
        letterSpacing: "0.06em",
        color: muted,
        fontWeight: 600,
        marginBottom: 6,
      }}
    >
      {children}
    </div>
  );
}
