
import {
  isFieldContentValid,
  imageChange,
  displaySystemPopup,
} from "@app/Infra/Util";

export default class {
  static getHtml(id, msg, title) {
    return `<div class="form-row no-padding">                
                <table width='100%'>
                    <tr>
                        <td valign='top' width='20px'>
                            <img src='src/static/img/lock.png' alt='Password Field' id='fail${id}' style='padding-top:9px' />
                        </td>
                        <td valign='top' width='90%'>
                            <input id="${id}" type="password" class="form-element" placeholder="${msg}">
                        </td>
                        <td valign='top' width='20px'>
                            <img src='src/static/img/eye.png' alt='Password Field' id='seePass${id}' title='${title}' style='padding-top:9px' />                            
                        </td>
                    </tr>
                </table>
            </div>`;
  }

  static btnSeePassword(id) {
    $(id).removeAttr("type");
    if ($("#seePass" + id)[0].src.indexOf("_err.png") > 0) {
      imageChange("#seePass" + id, "eye.png");
      $("#" + id).attr("type", "password");
    } else {
      imageChange("#seePass" + id, "eye_err.png");
      $("#" + id).attr("type", "text");
    }
    $("#" + id)[0].focus();
  }

  static validate(id, _params, _title, _msg) {
    if (!isFieldContentValid($("#" + id).val(), 20, "password", 4)) {
      if (_params.focus == true) $("#" + id).focus();
      else {
        imageChange("#fail" + id, "lock_err.png");
        if (_params.msg == true) displaySystemPopup(_title, _msg);
      }
      return false;
    } else imageChange("#fail" + id, "lock.png");

    return true;
  }
}
