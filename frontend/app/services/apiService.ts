const BASE_URL = "http://localhost:5166/api";

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  //204 No Content har ingen body at læse.
  if (response.status === 204) return undefined as T;

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    throw new Error(errorMessage(data));
  }

  return data as T;
}

//Trækker en læsbar besked ud, uanset om fejlen kom fra vores service eller fra ASP.NETs validering.
function errorMessage(data: any): string {
  if (!data) return "Der opstod en uventet fejl.";

  //Vores egne regler: { "message": "..." }
  if (typeof data.message === "string") return data.message;

  //ASP.NETs validering: { "errors": { "name": ["..."] } }
  if (data.errors) {
    const first = Object.values(data.errors)[0];
    if (Array.isArray(first) && first.length > 0) return String(first[0]);
  }

  return data.title ?? "Der opstod en uventet fejl.";
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: "POST", body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: <T>(path: string) => request<T>(path, { method: "DELETE" }),
};