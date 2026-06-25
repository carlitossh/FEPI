import React from "react";
import { C } from "../styles/theme";

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
        background: C.red,
        color: "#fff",
        border: "none",
        borderRadius: 10,
        padding: "8px 16px",
        fontSize: 12.5,
        fontWeight: 600,
        cursor: "pointer",
        fontFamily: "'IBM Plex Sans', sans-serif",
        display: "inline-flex",
        alignItems: "center",
        gap: 6,
        ...style,
      }}
    >
      {children}
    </button>
  );
}
