import { C } from "../styles/theme";
import { SectionLabel } from "./SectionLabel";

interface FieldProps {
  label: string;
  value: string;
}

export function Field({ label, value }: FieldProps) {
  return (
    <div>
      <SectionLabel>{label}</SectionLabel>
      <div
        style={{
          fontSize: 13,
          color: C.fg,
          fontFamily: "JetBrains Mono",
          fontWeight: 500,
        }}
      >
        {value}
      </div>
    </div>
  );
}
