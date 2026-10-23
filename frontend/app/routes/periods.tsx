import { useEffect, useState } from "react";
import DataTable, { type Column } from "../components/DataTable";
import { api } from "../services/apiService";
import type { PlanningPeriod, SavePlanningPeriod } from "../types";
import { formatDate } from "../utils/format";

const emptyForm: SavePlanningPeriod = { name: "", startDate: "", endDate: "" };

export default function Periods() {
  const [periods, setPeriods] = useState<PlanningPeriod[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<SavePlanningPeriod>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);

  //Henter listen fra API'et. Kaldes når siden åbnes og efter hver ændring.
  async function load() {
    try {
      setPeriods(await api.get<PlanningPeriod[]>("/planningperiods"));
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  //Tom liste som anden parameter betyder: kør én gang, når siden åbnes.
  useEffect(() => {
    load();
  }, []);

  function startCreate() {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowForm(true);
  }

  function startEdit(period: PlanningPeriod) {
    setForm({
      name: period.name,
      startDate: period.startDate,
      endDate: period.endDate,
    });
    setEditingId(period.id);
    setError(null);
    setShowForm(true);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (editingId) {
        await api.put(`/planningperiods/${editingId}`, form);
      } else {
        await api.post("/planningperiods", form);
      }
      setShowForm(false);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function handleDelete(period: PlanningPeriod) {
    if (!confirm(`Slet perioden "${period.name}"?`)) return;

    try {
      await api.delete(`/planningperiods/${period.id}`);
      await load();
    } catch (e) {
      setError((e as Error).message);
    }
  }

   const columns: Column<PlanningPeriod>[] = [
    {
      key: "name",
      header: "Navn",
      value: (p) => p.name,
      //En periodes navn er en etiket på et tidsrum, så alfabetisk sortering ville give
      //"December 2026" før "Januar 2025". Derfor sorteres der kronologisk i stedet.
      sortValue: (p) => p.startDate,
      filterable: true,
    },
    {
      key: "start",
      header: "Start",
      value: (p) => formatDate(p.startDate),
      //Sorterer på datoen fra API'et (2026-12-01), ikke på den viste tekst (01-12-2026).
      sortValue: (p) => p.startDate,
    },
    {
      key: "end",
      header: "Slut",
      value: (p) => formatDate(p.endDate),
      sortValue: (p) => p.endDate,
    },
  ];
  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-2xl font-bold">Perioder</h2>
        <button
          type="button"
          onClick={startCreate}
          className="rounded-md bg-[#3c5a78] px-4 py-2 text-sm font-medium text-white hover:bg-[#32506b]"
        >
          Opret periode
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
          <div className="flex flex-wrap gap-4">
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
                Startdato
              </label>
              <input
                required
                type="date"
                max={form.endDate}
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
        rows={periods}
        onEdit={startEdit}
        onDelete={handleDelete}
        emptyText="Der er ingen perioder lige nu."
        loading={loading}
        defaultSortKey="start"
      />
    </div>
  );
}
