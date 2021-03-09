
import { buildTable } from "@app/Infra/Util";

export default class {
  constructor(table, textColor, backColor, styleClass) {
    this.state = {
      table: table,
      textColor: textColor,
      backColor: backColor,
      styleClass: styleClass,
    };

    if (this.state.textColor == undefined) this.state.textColor = "white";
    if (this.state.backColor == undefined) this.state.backColor = "grey";
    if (this.state.styleClass == undefined)
      this.state.styleClass = "table table-hover";

    $(document).bind("click", function (e) {
      $("row").trigger("click");
    });
  }

  getHtml() {
    if (this.state.table === null) return "";
    return `${buildTable(
      this.state.table,
      this.state.textColor,
      this.state.backColor,
      this.state.styleClass
    )}`;
  }
}
