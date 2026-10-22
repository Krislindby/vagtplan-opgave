import { useEffect, useState } from "react";
import DataTable, { type Column } from "./DataTable";
import { api } from "../services/apiService";
import type { User, SaveUser } from "../types";

const emptyForm: SaveUser = { name: "", email: "", isAdmin: false, isStaff: true };

export default function UsersSection() {
  const [users, setUsers] = useState<User[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<SaveUser>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);

  async function load() {
    try {
      setUsers(await api.get<User[]>("/users"));
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

  function startEdit(user: User) {
    setForm({
      name: user.name,
      email: user.email,
      isAdmin: user.isAdmin,
      isStaff: user.isStaff,
    });
    setEditingId(user.id);
    setError(null);
    setShowForm(true);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (editingId) {
        await api.put(`/users/${editingId}`, form);
      } else {
        await api.post("/users", form);
      }
      setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(user: User) {
    if (!confirm(`Slet brugeren "${user.name}"?`)) return;

    try {
      await api.delete(`/users/${user.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

    const columns: Column<User>[] = [
    { key: "name", header: "Navn", value: (u) => u.name },
    { key: "email", header: "E-mail", value: (u) => u.email },
    {
      key: "roles",
      header: "Roller",
      value: (u) =>
        [u.isAdmin && "Admin", u.isStaff && "Personale"]
          .filter(Boolean)
          .join(", "),
      filterable: true,
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
          Opret adgang
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
                E-mail
              </label>
              <input
                required
                type="email"
                maxLength={200}
                value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
                className="w-64 rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </div>
            <label className="flex items-center gap-2 pb-2 text-sm">
              <input
                type="checkbox"
                checked={form.isAdmin}
                onChange={(e) => setForm({ ...form, isAdmin: e.target.checked })}
              />
              Admin
            </label>
            <label className="flex items-center gap-2 pb-2 text-sm">
              <input
                type="checkbox"
                checked={form.isStaff}
                onChange={(e) => setForm({ ...form, isStaff: e.target.checked })}
              />
              Personale
            </label>
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
        rows={users}
        onEdit={startEdit}
        onDelete={handleDelete}
        emptyText="Ingen adgange oprettet endnu."
      />
    </div>
  );
}