import React from "react";
import { rule, ink } from "../styles/theme";

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
        ...style,
      }}
    />
  );
}
