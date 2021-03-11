
import { BaseLoader } from "@app/Components/Images/BaseLoader";

export default class {
  static loadAsync(id, url) {
    $("#" + id).css("display", "none");
    $("#" + id)
      .attr("src", url)
      .on("load", function () {
        if (
          !this.complete ||
          typeof this.naturalWidth == "undefined" ||
          this.naturalWidth == 0
        ) {
        } else {
          $("#" + id).css("display", "block");
        }
      });
  }

  static getHtml(id, style, _class) {
    if (style == undefined) style = "";
    else style = "style='" + style + "'";
    if (_class == undefined) _class = "";
    else _class = "class='" + _class + "'";
    return `<img id='${id}' ${style} ${_class} alt=' ' />${BaseLoader(
      "loading_img_" + id,
      "display:block"
    )}`;
  }

  static getDirectHtml(id, url, style, _class) {
    if (style == undefined) style = "";
    else style = "style='" + style + "'";
    if (_class == undefined) _class = "";
    else _class = "class='" + _class + "'";
    return `<img id='${id}' src='${url}' ${style} ${_class} alt=' ' />`;
  }
}
