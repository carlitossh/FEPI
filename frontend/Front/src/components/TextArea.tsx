import { C } from "../styles/theme";

interface TextAreaProps {
  placeholder?: string;
  value: string;
  onChange: (value: string) => void;
  rows?: number;
}

export function TextArea({ placeholder, value, onChange, rows = 3 }: TextAreaProps) {
  return (
    <textarea
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      rows={rows}
      style={{
        border: `1px solid ${C.border}`,
        background: C.surface2,
        borderRadius: 10,
        padding: "9px 12px",
        fontSize: 12.5,
        color: C.fg,
        fontFamily: "'IBM Plex Sans', sans-serif",
        outline: "none",
        width: "100%",
        boxSizing: "border-box",
        resize: "vertical",
        colorScheme: "dark",
      }}
    />
  );
}
