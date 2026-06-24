import { ink, paper } from "../styles/theme";

interface TableHeaderProps {
  cols: string[];
}

export function TableHeader({ cols }: TableHeaderProps) {
  return (
    <thead>
      <tr style={{ background: ink }}>
        {cols.map((h) => (
          <th
            key={h}
            style={{
              textAlign: "left",
              padding: "10px 14px",
              color: paper,
              fontSize: 10.5,
              textTransform: "uppercase",
              letterSpacing: "0.06em",
              fontWeight: 600,
              whiteSpace: "nowrap",
            }}
          >
            {h}
          </th>
        ))}
      </tr>
    </thead>
  );
}
