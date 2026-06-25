import { C } from "../styles/theme";

interface TableHeaderProps {
  cols: string[];
}

export function TableHeader({ cols }: TableHeaderProps) {
  return (
    <thead>
      <tr style={{ background: C.surface2 }}>
        {cols.map((h) => (
          <th
            key={h}
            style={{
              textAlign: "left",
              padding: "10px 14px",
              color: C.fgMuted,
              fontSize: 10.5,
              textTransform: "uppercase",
              letterSpacing: "0.06em",
              fontWeight: 600,
              whiteSpace: "nowrap",
              borderBottom: `1px solid ${C.border}`,
            }}
          >
            {h}
          </th>
        ))}
      </tr>
    </thead>
  );
}
