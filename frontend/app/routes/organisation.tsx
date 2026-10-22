import { useState } from "react";
import WardsSection from "../components/WardsSection";
import StaffGroupsSection from "../components/StaffGroupsSection";
import test from "node:test";

export default function Organisation() {
  const [tab, setTab] = useState<"wards" | "staffGroups">("wards");

  return (
    <div>
      <h2 className="mb-6 text-2xl font-bold">Organisering</h2>

      <div className="mb-6 flex gap-1 border-b border-slate-200">
        <button
          type="button"
          onClick={() => setTab("wards")}
          className={`px-4 py-2 text-sm ${
            tab === "wards"
              ? "border-b-2 border-[#3c5a78] font-semibold text-[#3c5a78]"
              : "text-slate-600 hover:text-slate-900"
          }`}
        >
          Afsnit
        </button>
        <button
          type="button"
          onClick={() => setTab("staffGroups")}
          className={`px-4 py-2 text-sm ${
            tab === "staffGroups"
              ? "border-b-2 border-[#3c5a78] font-semibold text-[#3c5a78]"
              : "text-slate-600 hover:text-slate-900"
          }`}
        >
          Personalegrupper
        </button>
      </div>

      {tab === "wards" ? <WardsSection /> : <StaffGroupsSection />}
    </div>
  );
}