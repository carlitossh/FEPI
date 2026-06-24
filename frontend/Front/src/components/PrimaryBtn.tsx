import React from "react";
import { obra } from "../styles/theme";

interface PrimaryBtnProps {
  children: React.ReactNode;
  onClick?: () => void;
  disabled?: boolean;
  style?: React.CSSProperties;
}

export function PrimaryBtn({ children, onClick, disabled, style = {} }: PrimaryBtnProps) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      style={{
        background: disabled ? "#C9C2AC" : obra,
        color: "#fff",
        border: "none",
        borderRadius: 3,
        padding: "8px 16px",
        fontSize: 12.5,
        fontWeight: 600,
        cursor: disabled ? "not-allowed" : "pointer",
        fontFamily: "'IBM Plex Sans', sans-serif",
        ...style,
      }}
    >
      {children}
    </button>
  );
}
