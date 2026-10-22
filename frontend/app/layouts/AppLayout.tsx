import { NavLink, Outlet } from "react-router";

const menuItems = [
  { to: "/", label: "Perioder", end: true },
  { to: "/organisation", label: "Organisering" },
  { to: "/access", label: "Adgangstyring" },
  { to: "/staff", label: "Personale" },
];

export default function AppLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-white text-slate-900">
        <header className="flex items-center gap-3 border-b border-slate-200 px-6 py-4">
            {/* Plads til logo. Erstattes med et billede, når et er leveret. */}
            <div className="h-9 w-9 rounded-lg bg-slate-200" />
            <h1 className="text-xl font-bold">FairPlan</h1>
      </header>

      <div className="flex flex-1">
        <nav className="w-56 shrink-0 border-r border-slate-200 px-4 py-8">
          <ul className="space-y-1">
            {menuItems.map((item) => (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  end={item.end}
                  className={({ isActive }) =>
                    `block rounded-md px-3 py-2 text-sm ${
                      isActive
                        ? "bg-[#eef3f8] font-semibold text-[#3c5a78]"
                        : "text-slate-700 hover:bg-slate-50"
                    }`
                  }
                >
                  {item.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>

        <main className="flex-1 px-10 py-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}