
import {
  isFieldContentValid,
  displaySystemPopup,
  imageChange,
} from "@app/Infra/Util";

export default class {
  static getHtml(id, msg) {
    return `<div class="form-row no-padding">
                <table width='100%'>
                    <tr>
                        <td width='20px'>
                            <img src='src/static/img/email.png' alt='Email' id='fail${id}' style='padding-top:6px' />
                        </td>
                        <td>
                            <input id="${id}" type="text" class="form-element" placeholder="${msg}">
                        </td>
                    </tr>
                </table>
            </div>`;
  }

  static validate(id, _params, _title, _msg) {
    if (!isFieldContentValid($("#" + id).val(), 99, "email")) {
      if (_params.focus == true) $("#" + id).focus();
      else {
        imageChange("#fail" + id, "email_err.png");
        if (_params.msg == true) displaySystemPopup(_title, _msg);
      }
      return false;
    } else imageChange("#fail" + id, "email.png");
    return true;
  }
}
