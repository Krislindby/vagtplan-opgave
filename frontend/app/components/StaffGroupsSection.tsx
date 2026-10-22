import { useEffect, useState } from "react";
import DataTable, { type Column } from "./DataTable";
import { api } from "../services/apiService";
import type { StaffGroup, SaveStaffGroup } from "../types";

const emptyForm: SaveStaffGroup = { name: "" };

export default function StaffGroupsSection() {
  const [staffGroups, setStaffGroups] = useState<StaffGroup[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<SaveStaffGroup>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);

  async function load() {
    try {
      setStaffGroups(await api.get<StaffGroup[]>("/staffgroups"));
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

  function startEdit(staffGroup: StaffGroup) {
    setForm({ name: staffGroup.name });
    setEditingId(staffGroup.id);
    setError(null);
    setShowForm(true);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (editingId) {
        await api.put(`/staffgroups/${editingId}`, form);
      } else {
        await api.post("/staffgroups", form);
      }
      setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(staffGroup: StaffGroup) {
    if (!confirm(`Slet personalegruppen "${staffGroup.name}"?`)) return;

    try {
      await api.delete(`/staffgroups/${staffGroup.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const columns: Column<StaffGroup>[] = [
    { key: "name", header: "Navn", value: (s) => s.name, filterable: true },
  ];

  return (
    <div>
      <div className="mb-4 flex justify-end">
        <button
          type="button"
          onClick={startCreate}
          className="rounded-md bg-[#3c5a78] px-4 py-2 text-sm font-medium text-white hover:bg-[#32506b]"
        >
          Opret personalegruppe
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
        rows={staffGroups}
        onEdit={startEdit}
        onDelete={handleDelete}
        emptyText="Ingen personalegrupper oprettet endnu."
      />
    </div>
  );
}