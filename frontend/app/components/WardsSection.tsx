import { useEffect, useState } from "react";
import DataTable, { type Column } from "./DataTable";
import { api } from "../services/apiService";
import type { Ward, SaveWard } from "../types";

const emptyForm: SaveWard = { name: "", color: "#3c5a78" };

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

  function startCreate() {
    setForm(emptyForm);
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
    { key: "name", header: "Navn", value: (w) => w.name },
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
          <div className="flex flex-wrap items-end gap-4">
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
              <input
                required
                type="color"
                value={form.color}
                onChange={(e) => setForm({ ...form, color: e.target.value })}
                className="h-9 w-16 rounded-md border border-slate-300"
              />
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