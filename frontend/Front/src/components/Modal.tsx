import React from "react";
import { X } from "lucide-react";
import { paper2, obra } from "../styles/theme";

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
        background: "rgba(26,34,56,0.45)",
        zIndex: 100,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 20,
      }}
    >
      <div
        style={{
          background: paper2,
          borderRadius: 5,
          width,
          maxWidth: "100%",
          boxShadow: "0 20px 60px rgba(26,34,56,0.25)",
          overflow: "hidden",
          maxHeight: "90vh",
          display: "flex",
          flexDirection: "column",
        }}
      >
        <div
          style={{
            background: obra,
            color: "#fff",
            padding: "14px 20px",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            flexShrink: 0,
          }}
        >
          <div>
            {subtitle && (
              <div
                style={{
                  fontFamily: "JetBrains Mono",
                  fontSize: 10.5,
                  opacity: 0.7,
                  marginBottom: 2,
                }}
              >
                {subtitle}
              </div>
            )}
            <div
              style={{
                fontFamily: "'IBM Plex Serif', serif",
                fontWeight: 700,
                fontSize: 16,
              }}
            >
              {title}
            </div>
          </div>
          <button
            onClick={onClose}
            style={{
              background: "none",
              border: "none",
              color: "#fff",
              cursor: "pointer",
              padding: 4,
            }}
          >
            <X size={18} />
          </button>
        </div>
        <div style={{ padding: 20, overflowY: "auto" }}>{children}</div>
      </div>
    </div>
  );
}
