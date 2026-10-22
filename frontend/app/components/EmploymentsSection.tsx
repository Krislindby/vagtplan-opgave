import { useEffect, useState } from "react";
import DataTable, { type Column } from "./DataTable";
import { api } from "../services/apiService";
import type {
  Employment,
  SaveEmployment,
  User,
  Ward,
  StaffGroup,
} from "../types";
import { formatDate } from "../utils/format";

const emptyForm: SaveEmployment = {
  userId: "",
  wardId: "",
  staffGroupId: "",
  hoursPerWeek: 37,
  startDate: "",
  endDate: "",
};

export default function EmploymentsSection() {
  const [employments, setEmployments] = useState<Employment[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [wards, setWards] = useState<Ward[]>([]);
  const [staffGroups, setStaffGroups] = useState<StaffGroup[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<SaveEmployment>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);

  //Henter både ansættelserne og de tre lister, dropdowns skal bruge.
  async function load() {
    try {
      const [e, u, w, s] = await Promise.all([
        api.get<Employment[]>("/employments"),
        api.get<User[]>("/users"),
        api.get<Ward[]>("/wards"),
        api.get<StaffGroup[]>("/staffgroups"),
      ]);
      setEmployments(e);
      setUsers(u);
      setWards(w);
      setStaffGroups(s);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
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

  function startEdit(employment: Employment) {
    setForm({
      userId: employment.userId,
      wardId: employment.wardId,
      staffGroupId: employment.staffGroupId,
      hoursPerWeek: employment.hoursPerWeek,
      startDate: employment.startDate,
      endDate: employment.endDate,
    });
    setEditingId(employment.id);
    setError(null);
    setShowForm(true);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (editingId) {
        await api.put(`/employments/${editingId}`, form);
      } else {
        await api.post("/employments", form);
      }
      setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(employment: Employment) {
    if (!confirm(`Slet ansættelsen for "${employment.userName}"?`)) return;

    try {
      await api.delete(`/employments/${employment.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  //Kun personale kan ansættes, så dropdown'en viser kun dem.
  const staffUsers = users.filter((u) => u.isStaff);

  const columns: Column<Employment>[] = [
    { key: "user", header: "Person", value: (e) => e.userName },
    { key: "ward", header: "Afsnit", value: (e) => e.wardName, filterable: true },
    { key: "staffGroup", header: "Personalegruppe", value: (e) => e.staffGroupName, filterable: true },
    { key: "hours", header: "Timer/uge", value: (e) => String(e.hoursPerWeek) },
    { key: "start", header: "Start", value: (e) => formatDate(e.startDate) },
    { key: "end", header: "Slut", value: (e) => formatDate(e.endDate) },
  ];

  return (
    <div>
      <div className="mb-4 flex justify-end">
        <button
          type="button"
          onClick={startCreate}
          className="rounded-md bg-[#3c5a78] px-4 py-2 text-sm font-medium text-white hover:bg-[#32506b]"
        >
          Opret ansættelse
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
                Person
              </label>
              <select
                required
                value={form.userId}
                onChange={(e) => setForm({ ...form, userId: e.target.value })}
                className="w-56 rounded-md border border-slate-300 px-3 py-2 text-sm"
              >
                <option value="">Vælg person</option>
                {staffUsers.map((u) => (
                  <option key={u.id} value={u.id}>{u.name}</option>
                ))}
              </select>
              {staffUsers.length === 0 && (
                <p className="mt-1 w-56 text-xs text-amber-700">
                  Opret først en adgang med rollen Personale under fanen Adgangsstyring.
                </p>
              )}
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Afsnit
              </label>
              <select
                required
                value={form.wardId}
                onChange={(e) => setForm({ ...form, wardId: e.target.value })}
                className="w-56 rounded-md border border-slate-300 px-3 py-2 text-sm"
              >
                                <option value="">Vælg afsnit</option>
                {wards.map((w) => (
                  <option key={w.id} value={w.id}>{w.name}</option>
                ))}
              </select>
              {wards.length === 0 && (
                <p className="mt-1 w-56 text-xs text-amber-700">
                  Opret først et afsnit under fanen Organisering.
                </p>
              )}
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Personalegruppe
              </label>
              <select
                required
                value={form.staffGroupId}
                onChange={(e) => setForm({ ...form, staffGroupId: e.target.value })}
                className="w-56 rounded-md border border-slate-300 px-3 py-2 text-sm"
              >
                                <option value="">Vælg personalegruppe</option>
                {staffGroups.map((s) => (
                  <option key={s.id} value={s.id}>{s.name}</option>
                ))}
              </select>
              {staffGroups.length === 0 && (
                <p className="mt-1 w-56 text-xs text-amber-700">
                  Opret først en personalegruppe under fanen Organisering.
                </p>
              )}
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Timer pr. uge
              </label>
              <input
                required
                type="number"
                min={0}
                max={37}
                value={form.hoursPerWeek}
                onChange={(e) => setForm({ ...form, hoursPerWeek: Number(e.target.value) })}
                className="w-28 rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Startdato
              </label>
              <input
                required
                type="date"
                value={form.startDate}
                onChange={(e) => setForm({ ...form, startDate: e.target.value })}
                className="rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">
                Slutdato
              </label>
              <input
                required
                type="date"
                min={form.startDate}
                value={form.endDate}
                onChange={(e) => setForm({ ...form, endDate: e.target.value })}
                className="rounded-md border border-slate-300 px-3 py-2 text-sm"
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
        rows={employments}
        onEdit={startEdit}
        onDelete={handleDelete}
        emptyText="Ingen ansættelser oprettet endnu."
        loading={loading}
      />
    </div>
  );
}