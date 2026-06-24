import { ink } from "../styles/theme";
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
          color: ink,
          fontFamily: "JetBrains Mono",
          fontWeight: 500,
        }}
      >
        {value}
      </div>
    </div>
  );
}
