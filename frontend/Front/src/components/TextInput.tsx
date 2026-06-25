import React from "react";
import { C } from "../styles/theme";

interface TextInputProps {
  placeholder?: string;
  value: string;
  onChange: (value: string) => void;
  style?: React.CSSProperties;
}

export function TextInput({ placeholder, value, onChange, style = {} }: TextInputProps) {
  return (
    <input
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
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
        colorScheme: "dark",
        ...style,
      }}
    />
  );
}
