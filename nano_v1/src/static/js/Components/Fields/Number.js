
import {
  isFieldContentValid,
  imageChange,
  displaySystemPopup,
} from "@app/Infra/Util";

export default class {
  static getHtml(id, placeholderMsg) {
    return `<div class="form-row no-padding">
                    <table width='100%'>
                        <tr>
                            <td width='20px'>
                                <img src='src/static/img/bolt.png' alt='Number Field' id='fail${id}' style='padding-top:6px' />
                            </td>
                            <td>
                                <input id="${id}" type="tel" pattern="[0-9]*" inputmode="numeric" class="form-element" placeholder="${placeholderMsg}">
                            </td>
                        </tr>
                    </table>
                </div>`;
  }

  static validate(id, _params, _title, _msg) {
    if (!isFieldContentValid($("#" + id).val(), 16, "number")) {
      if (_params.focus == true) $("#" + id).focus();
      else {
        imageChange("#fail" + id, "bolt.png");
        if (_params.msg == true) displaySystemPopup(_title, _msg);
      }
      return false;
    } else imageChange("#fail" + id, "bolt_err.png");
    return true;
  }
}
