import React from "react";
import { C } from "../styles/theme";

interface CardProps {
  children: React.ReactNode;
  style?: React.CSSProperties;
}

export function Card({ children, style = {} }: CardProps) {
  return (
    <div
      style={{
        background: C.surface,
        border: `1px solid ${C.border}`,
        borderRadius: 16,
        overflow: "hidden",
        ...style,
      }}
    >
      {children}
    </div>
  );
}
