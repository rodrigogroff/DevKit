export default class {
  static getHtml(id, str, checked, _style, _color) {
    if (_style == undefined) _style = "";
    else _style = "style=" + _style;
    return `<div class="form-row no-padding">
            <input type='checkbox' id='${id}' ${checked} /><label for='${id}'><span style='font-size:small'>${str}</span></label>
        </div>`;
  }
}
