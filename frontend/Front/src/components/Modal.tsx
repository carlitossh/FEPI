import React from "react";
import { X } from "lucide-react";
import { C } from "../styles/theme";

interface ModalProps {
  title: string;
  subtitle?: string;
  onClose: () => void;
  children: React.ReactNode;
  width?: number;
}

export function Modal({ title, subtitle, onClose, children, width = 480 }: ModalProps) {
  return (
    <div
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(0,0,0,0.65)",
        zIndex: 100,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 20,
        backdropFilter: "blur(4px)",
      }}
    >
      <div
        style={{
          background: C.surface,
          border: `1px solid ${C.borderHi}`,
          borderRadius: 18,
          width,
          maxWidth: "100%",
          boxShadow: "0 32px 80px rgba(0,0,0,0.5)",
          overflow: "hidden",
          maxHeight: "90vh",
          display: "flex",
          flexDirection: "column",
        }}
      >
        <div
          style={{
            background: C.surface2,
            padding: "14px 20px",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            flexShrink: 0,
            borderBottom: `1px solid ${C.border}`,
          }}
        >
          <div>
            {subtitle && (
              <div
                style={{
                  fontFamily: "JetBrains Mono",
                  fontSize: 10.5,
                  color: C.fgMuted,
                  marginBottom: 2,
                }}
              >
                {subtitle}
              </div>
            )}
            <div
              style={{
                fontWeight: 700,
                fontSize: 16,
                color: C.fg,
              }}
            >
              {title}
            </div>
          </div>
          <button
            onClick={onClose}
            style={{
              background: C.surface,
              border: `1px solid ${C.border}`,
              borderRadius: 8,
              color: C.fgMuted,
              cursor: "pointer",
              padding: 6,
              display: "flex",
            }}
          >
            <X size={16} />
          </button>
        </div>
        <div style={{ padding: 20, overflowY: "auto" }}>{children}</div>
      </div>
    </div>
  );
}
