import React from "react";
import { folio } from "../styles/theme";

interface DangerBtnProps {
  children: React.ReactNode;
  onClick?: () => void;
  style?: React.CSSProperties;
}

export function DangerBtn({ children, onClick, style = {} }: DangerBtnProps) {
  return (
    <button
      onClick={onClick}
      style={{
        background: folio,
        color: "#fff",
        border: "none",
        borderRadius: 3,
        padding: "8px 16px",
        fontSize: 12.5,
        fontWeight: 600,
        cursor: "pointer",
        fontFamily: "'IBM Plex Sans', sans-serif",
        ...style,
      }}
    >
      {children}
    </button>
  );
}
