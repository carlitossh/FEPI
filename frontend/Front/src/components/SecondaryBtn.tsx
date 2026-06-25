import React from "react";
import { C } from "../styles/theme";

interface SecondaryBtnProps {
  children: React.ReactNode;
  onClick?: () => void;
  style?: React.CSSProperties;
  color?: string;
}

export function SecondaryBtn({ children, onClick, style = {}, color }: SecondaryBtnProps) {
  return (
    <button
      onClick={onClick}
      style={{
        background: C.surface2,
        color: color || C.fg,
        border: `1px solid ${C.border}`,
        borderRadius: 10,
        padding: "7px 14px",
        fontSize: 12,
        fontWeight: 500,
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
