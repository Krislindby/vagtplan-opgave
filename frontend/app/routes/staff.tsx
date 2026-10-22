import { useEffect, useState } from "react";
import DataTable, { type Column } from "../components/DataTable";
import { api } from "../services/apiService";
import type { PlanningPeriod, Staff } from "../types";
import { formatDate } from "../utils/format";

export default function StaffPage() {
  const [periods, setPeriods] = useState<PlanningPeriod[]>([]);
  const [selectedPeriodId, setSelectedPeriodId] = useState("");
  const [staff, setStaff] = useState<Staff[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  //Henter perioderne til dropdownen og vælger den første.
  useEffect(() => {
    api
      .get<PlanningPeriod[]>("/planningperiods")
      .then((data) => {
        setPeriods(data);
        if (data.length > 0) setSelectedPeriodId(data[0].id);
        else setLoading(false); //Ingen perioder: der kommer ikke flere kald.
      })
      .catch((e) => {
        setError((e as Error).message);
        setLoading(false);
      });
  }, []);

  //Kører igen, hver gang brugeren vælger en anden periode.
  useEffect(() => {
    if (!selectedPeriodId) {
      setStaff([]);
      return;
    }

    setLoading(true);

    api
      .get<Staff[]>(`/staff?planningPeriodId=${selectedPeriodId}`)
      .then(setStaff)
      .catch((e) => setError((e as Error).message))
      .finally(() => setLoading(false));
  }, [selectedPeriodId]);

  const columns: Column<Staff>[] = [
    { key: "name", header: "Navn", value: (s) => s.name },
    { key: "ward", header: "Afsnit", value: (s) => s.wardName, filterable: true },
    { key: "staffGroup", header: "Personalegruppe", value: (s) => s.staffGroupName, filterable: true },
    { key: "hours", header: "Timer/uge", value: (s) => String(s.hoursPerWeek) },
  ];

  const selectedPeriod = periods.find((p) => p.id === selectedPeriodId);

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">Personale</h2>

      {error && (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      <div className="mb-6">
        <label className="mb-1 block text-xs font-medium text-slate-600">
          Periode
        </label>
        <select
          value={selectedPeriodId}
          onChange={(e) => setSelectedPeriodId(e.target.value)}
          className="w-72 rounded-md border border-slate-300 px-3 py-2 text-sm"
        >
          {periods.length === 0 && <option value="">Ingen perioder oprettet</option>}
          {periods.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name}
            </option>
          ))}
        
        </select>
        {periods.length === 0 && (
          <p className="mt-1 w-72 text-xs text-amber-700">
            Opret først en periode under fanen Perioder.
          </p>
        )}
      </div>

      <DataTable
        columns={columns}
        rows={staff}
        emptyText="Ingen ansatte i den valgte periode."
        loading={loading}
      />
    </div>
  );
}