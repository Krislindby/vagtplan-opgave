import { type RouteConfig, index, route, layout } from "@react-router/dev/routes";

export default [
  layout("layouts/AppLayout.tsx", [
    index("routes/periods.tsx"),
    route("organisation", "routes/organisation.tsx"),
    route("access", "routes/access.tsx"),
    route("staff", "routes/staff.tsx"),
  ]),
] satisfies RouteConfig;