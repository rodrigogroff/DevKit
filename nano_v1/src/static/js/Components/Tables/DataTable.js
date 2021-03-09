
import { buildTableSimple } from "@app/Infra/Util";

export function getDataTable(table) {
  return `${buildTableSimple(table)}`;
}

