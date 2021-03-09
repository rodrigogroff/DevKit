import {
  isFieldContentValid,
  imageChange,
  displaySystemPopup,
} from "@app/Infra/Util";

export default class {
  static getHtml(id, placeholderMsg, isNumeric) {
    var numeric = '';
    if (isNumeric == true)
      numeric = 'type="tel" pattern="[0-9]*" inputmode="numeric"';
    return `<div class="form-row no-padding">
                <table width='100%'>
                    <tr>
                        <td width='20px'>
                            <img src='src/static/img/bolt.png' alt='INput Field' id='fail${id}' style='padding-top:6px' />
                        </td>
                        <td>
                            <input id="${id}" type="text" class="form-element" ${numeric} placeholder="${placeholderMsg}">
                        </td>
                    </tr>
                </table>
                </div>`;
  }

  static validateName(id, _params, _title, customMsg) {
    if (!isFieldContentValid($("#" + id).val(), 50, "fullName", 4)) {
      if (_params.focus == true) $("#" + id).focus();
      else {
        imageChange("#fail" + id, "bolt_err.png");
        if (_params.msg == true) displaySystemPopup(_title, customMsg);
      }
      return false;
    } else imageChange("#fail" + id, "bolt.png");
    return true;
  }

  static validate(id, _params, _title, customMsg) {
    if (!isFieldContentValid($("#" + id).val(), 16, "string", 4)) {
      if (_params.focus == true) $("#" + id).focus();
      else {
        imageChange("#fail" + id, "bolt_err.png");
        if (_params.msg == true) displaySystemPopup(_title, customMsg);
      }
      return false;
    } else imageChange("#fail" + id, "bolt.png");
    return true;
  }

  static validateCPF(id, _params, _title, customMsg) {
    var strCPF = $("#" + id)
      .val()
      .replace(/\D/g, "");
    if (!this.CheckCPF(strCPF)) {
      if (_params != undefined)
      {
        if (_params.focus == true) $("#" + id).focus();
        else {
          imageChange("#fail" + id, "bolt_err.png");
          if (_params.msg == true) displaySystemPopup(_title, customMsg);
        }
      }      
      return false;
    } else imageChange("#fail" + id, "bolt.png");

    return true;
  }

  static CheckCPF(strCPF) {
    var Soma;
    var Resto;
    Soma = 0;
    if (strCPF == "00000000000") return false;
    for (let i = 1; i <= 9; i++)
      Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (11 - i);
    Resto = (Soma * 10) % 11;
    if (Resto == 10 || Resto == 11) Resto = 0;
    if (Resto != parseInt(strCPF.substring(9, 10))) return false;
    Soma = 0;
    for (let i = 1; i <= 10; i++)
      Soma = Soma + parseInt(strCPF.substring(i - 1, i)) * (12 - i);
    Resto = (Soma * 10) % 11;
    if (Resto == 10 || Resto == 11) Resto = 0;
    if (Resto != parseInt(strCPF.substring(10, 11))) return false;
    return true;
  }
}
