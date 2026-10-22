//API'et sender "2026-12-01". Brugeren skal se "01-12-2026".
export function formatDate(iso: string): string {
  if (!iso) return "";
  const [year, month, day] = iso.split("-");
  return `${day}-${month}-${year}`;
}