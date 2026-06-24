import React from "react";
import { paper2, rule } from "../styles/theme";

interface CardProps {
  children: React.ReactNode;
  style?: React.CSSProperties;
}

export function Card({ children, style = {} }: CardProps) {
  return (
    <div
      style={{
        background: paper2,
        border: `1px solid ${rule}`,
        borderRadius: 4,
        ...style,
      }}
    >
      {children}
    </div>
  );
}
