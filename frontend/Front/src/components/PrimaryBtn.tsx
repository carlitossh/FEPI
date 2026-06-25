import React from "react";
import { C } from "../styles/theme";

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
        background: disabled ? C.fgSub : C.blue,
        color: "#fff",
        border: "none",
        borderRadius: 10,
        padding: "8px 16px",
        fontSize: 12.5,
        fontWeight: 600,
        cursor: disabled ? "not-allowed" : "pointer",
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
