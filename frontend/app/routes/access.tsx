import { useState } from "react";
import UsersSection from "../components/UsersSection";
import EmploymentsSection from "../components/EmploymentsSection";

export default function Access() {
  const [tab, setTab] = useState<"users" | "employments">("users");

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">Adgangstyring</h2>

      <div className="mb-6 flex gap-1 border-b border-slate-200">
        <button
          type="button"
          onClick={() => setTab("users")}
          className={`px-4 py-2 text-sm ${
            tab === "users"
              ? "border-b-2 border-[#3c5a78] font-semibold text-[#3c5a78]"
              : "text-slate-600 hover:text-slate-900"
          }`}
        >
          Adgange
        </button>
        <button
          type="button"
          onClick={() => setTab("employments")}
          className={`px-4 py-2 text-sm ${
            tab === "employments"
              ? "border-b-2 border-[#3c5a78] font-semibold text-[#3c5a78]"
              : "text-slate-600 hover:text-slate-900"
          }`}
        >
          Ansættelser
        </button>
      </div>

      {tab === "users" ? <UsersSection /> : <EmploymentsSection />}
    </div>
  );
}