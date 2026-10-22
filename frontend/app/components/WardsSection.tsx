import { useEffect, useState } from "react";
import DataTable, { type Column } from "./DataTable";
import { api } from "../services/apiService";
import type { Ward, SaveWard } from "../types";

const emptyForm: SaveWard = { name: "", color: "#3c5a78" };

//Fast palet med tydeligt forskellige farver, så afsnit kan kendes fra hinanden.
const PALETTE = [
  "#e5484d", "#f76808", "#ffb224", "#46a758", "#12a594",
  "#0091ff", "#3c5a78", "#8e4ec6", "#d6409f", "#6e6e6e",
  "#ff8b7e", "#c4a000", "#99d52a", "#3dd68c", "#00c2d7",
  "#52a9ff", "#9c7cf4", "#f96bb7", "#a18072", "#1c2024",
];

export default function WardsSection() {
  const [wards, setWards] = useState<Ward[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<SaveWard>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);

  async function load() {
    try {
      setWards(await api.get<Ward[]>("/wards"));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  useEffect(() => {
    load();
  }, []);

  //Vælger automatisk en ledig farve, når man opretter et nyt afsnit.
  function startCreate() {
    const free = PALETTE.find((c) => !wards.some((w) => w.color === c));
    setForm({ ...emptyForm, color: free ?? emptyForm.color });
    setEditingId(null);
    setError(null);
    setShowForm(true);
  }

  function startEdit(ward: Ward) {
    setForm({ name: ward.name, color: ward.color });
    setEditingId(ward.id);
    setError(null);
    setShowForm(true);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (editingId) {
        await api.put(`/wards/${editingId}`, form);
      } else {
        await api.post("/wards", form);
      }
      setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(ward: Ward) {
    if (!confirm(`Slet afsnittet "${ward.name}"?`)) return;

    try {
      await api.delete(`/wards/${ward.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const columns: Column<Ward>[] = [
    { key: "name", header: "Navn", value: (w) => w.name, filterable: true },
    {
      key: "color",
      header: "Farve",
      value: (w) => w.color,
      render: (w) => (
        <span
          className="inline-block h-5 w-5 rounded border border-slate-300"
          style={{ backgroundColor: w.color }}
          title={w.color}
        />
      ),
    },
  ];

  //Viser kun ledige farver. Afsnittets egen farve bliver stående, når det redigeres.
  const availableColors = PALETTE.filter(
    (color) =>
      color === form.color ||
      !wards.some((w) => w.color === color && w.id !== editingId)
  );

  return (
    <div>
      <div className="mb-4 flex justify-end">
        <button
          type="button"
          onClick={startCreate}
          className="rounded-md bg-[#3c5a78] px-4 py-2 text-sm font-medium text-white hover:bg-[#32506b]"
        >
          Opret afsnit
        </button>
      </div>

      {error && (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="mb-6 rounded-md border border-slate-200 bg-[#f8fafc] p-4"
        >
          <div className="flex flex-wrap items-start gap-8">
            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Navn
              </label>
              <input
                required
                maxLength={100}
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                className="w-64 rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Farve
              </label>
              <div className="flex w-72 flex-wrap gap-2">
                {availableColors.map((color) => (
                  <button
                    key={color}
                    type="button"
                    onClick={() => setForm({ ...form, color })}
                    className={`h-8 w-8 rounded-full border-2 ${
                      form.color === color
                        ? "border-slate-900"
                        : "border-transparent"
                    }`}
                    style={{ backgroundColor: color }}
                  />
                ))}
              </div>
            </div>
          </div>

          <div className="mt-4 flex gap-2">
            <button
              type="submit"
              className="rounded-md bg-[#3c5a78] px-4 py-2 text-sm font-medium text-white hover:bg-[#32506b]"
            >
              Gem
            </button>
            <button
              type="button"
              onClick={() => setShowForm(false)}
              className="rounded-md border border-slate-300 px-4 py-2 text-sm"
            >
              Annuller
            </button>
          </div>
        </form>
      )}

      <DataTable
        columns={columns}
        rows={wards}
        onEdit={startEdit}
        onDelete={handleDelete}
        emptyText="Ingen afsnit oprettet endnu."
      />
    </div>
  );
}
