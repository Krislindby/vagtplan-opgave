import { useMemo, useState } from "react";

export interface Column<T> {
  key: string;
  header: string;
  value: (row: T) => string;
  //Valgfri: vis noget andet end teksten, fx en farveprik. Søgning og sortering bruger stadig value.
  render?: (row: T) => React.ReactNode;
  //Valgfri: sortér på en anden værdi end den viste. Bruges til datoer, hvor "01-12-2026"
  //sorterer forkert som tekst, mens "2026-12-01" sorterer rigtigt.
  sortValue?: (row: T) => string | number;
  filterable?: boolean;
}

interface Props<T> {
  columns: Column<T>[];
  rows: T[];
  onEdit?: (row: T) => void;
  onDelete?: (row: T) => void;
  emptyText?: string;
  loading?: boolean;
  //Valgfri: hvilken kolonne tabellen er sorteret på, før brugeren klikker på noget.
  defaultSortKey?: string;
}

export default function DataTable<T extends { id: string }>({
  columns,
  rows,
  onEdit,
  onDelete,
  emptyText = "Ingen rækker endnu.",
  loading = false,
  defaultSortKey,
}: Props<T>) {
  const [search, setSearch] = useState("");
  const [filters, setFilters] = useState<Record<string, string>>({});
  const [sortKey, setSortKey] = useState<string | null>(defaultSortKey ?? null);
  const [sortAsc, setSortAsc] = useState(true);

  const visibleRows = useMemo(() => {
    let result = rows;

    //Søgning: match mod alle kolonner.
    const query = search.trim().toLowerCase();
    if (query) {
      result = result.filter((row) =>
        columns.some((c) => c.value(row).toLowerCase().includes(query))
      );
    }

    //Filtrering: eksakt match per valgt kolonne.
    for (const [key, selected] of Object.entries(filters)) {
      if (!selected) continue;
      const column = columns.find((c) => c.key === key);
      if (column) result = result.filter((row) => column.value(row) === selected);
    }

    //Sortering: sortValue bruges, hvis kolonnen har en. Tal sammenlignes som tal,
    //resten som tekst med dansk alfabet.
    if (sortKey) {
      const column = columns.find((c) => c.key === sortKey);
      if (column) {
        const sortOn = column.sortValue ?? column.value;

        result = [...result].sort((a, b) => {
          const left = sortOn(a);
          const right = sortOn(b);

          let compared: number;
          if (typeof left === "number" && typeof right === "number") {
            compared = left - right;
          } else {
            compared = String(left).localeCompare(String(right), "da");
          }

          return compared * (sortAsc ? 1 : -1);
        });
      }
    }

    return result;
  }, [rows, columns, search, filters, sortKey, sortAsc]);

  function toggleSort(key: string) {
    if (sortKey === key) {
      setSortAsc(!sortAsc);
    } else {
      setSortKey(key);
      setSortAsc(true);
    }
  }

  function distinctValues(column: Column<T>) {
    return [...new Set(rows.map((r) => column.value(r)))];
  }

  const filterableColumns = columns.filter((c) => c.filterable);

  return (
    <div>
      <div className="mb-4 flex flex-wrap items-end gap-3">
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-600">
            Søg
          </label>
          <input
            type="search"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Søg i tabellen"
            className="w-64 rounded-md border border-slate-300 px-3 py-2 text-sm"
          />
        </div>

        {filterableColumns.map((column) => (
          <div key={column.key}>
            <label className="mb-1 block text-xs font-medium text-slate-600">
              {column.header}
            </label>
            <select
              value={filters[column.key] ?? ""}
              onChange={(e) =>
                setFilters({ ...filters, [column.key]: e.target.value })
              }
              className="rounded-md border border-slate-300 px-3 py-2 text-sm"
            >
              <option value="">Alle</option>
              {distinctValues(column).map((v) => (
                <option key={v} value={v}>
                  {v}
                </option>
              ))}
            </select>
          </div>
        ))}
      </div>

      <table className="w-full border-collapse text-sm">
        <thead>
          <tr className="border-b border-slate-200 bg-[#eef3f8] text-left">
            {columns.map((column) => (
              <th key={column.key} className="px-4 py-2 font-semibold">
                <button
                  type="button"
                  onClick={() => toggleSort(column.key)}
                  className="flex items-center gap-1 hover:text-[#3c5a78]"
                >
                  {column.header}
                  {sortKey === column.key && <span>{sortAsc ? "▲" : "▼"}</span>}
                </button>
              </th>
            ))}
            {(onEdit || onDelete) && <th className="px-4 py-2" />}
          </tr>
        </thead>
        <tbody>
          {visibleRows.length === 0 && (
            <tr>
              <td
                colSpan={columns.length + 1}
                className="px-4 py-6 text-center text-slate-500"
              >
                {loading ? "Henter…" : emptyText}
              </td>
            </tr>
          )}

          {visibleRows.map((row) => (
            <tr key={row.id} className="border-b border-slate-100">
              {columns.map((column) => (
                <td key={column.key} className="px-4 py-2">
                  {column.render ? column.render(row) : column.value(row)}
                </td>
              ))}
              {(onEdit || onDelete) && (
                <td className="px-4 py-2 text-right whitespace-nowrap">
                  {onEdit && (
                    <button
                      type="button"
                      onClick={() => onEdit(row)}
                      className="mr-3 text-[#3c5a78] hover:underline"
                    >
                      Rediger
                    </button>
                  )}
                  {onDelete && (
                    <button
                      type="button"
                      onClick={() => onDelete(row)}
                      className="text-red-600 hover:underline"
                    >
                      Slet
                    </button>
                  )}
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
