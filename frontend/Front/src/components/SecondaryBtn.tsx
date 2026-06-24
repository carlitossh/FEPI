import React from "react";
import { ink, rule } from "../styles/theme";

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
        background: "transparent",
        color: color || ink,
        border: `1px solid ${rule}`,
        borderRadius: 3,
        padding: "7px 14px",
        fontSize: 12,
        fontWeight: 500,
        cursor: "pointer",
        fontFamily: "'IBM Plex Sans', sans-serif",
        ...style,
      }}
    >
      {children}
    </button>
  );
}
