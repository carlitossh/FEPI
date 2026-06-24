import { rule, ink } from "../styles/theme";

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
        border: `1px solid ${rule}`,
        background: "#FAF8F2",
        borderRadius: 3,
        padding: "8px 12px",
        fontSize: 12.5,
        color: ink,
        fontFamily: "'IBM Plex Sans', sans-serif",
        outline: "none",
        width: "100%",
        boxSizing: "border-box",
        resize: "vertical",
      }}
    />
  );
}
