
import SimpleNumberField from "@app/Components/Fields/SimpleNumber";
import PasswordField from "@app/Components/Fields/Password";
import Popup from "@app/Components/Modals/Popup";
import { LogoBig } from "@app/Components/Images/Logo";
import { BaseLoader } from "@app/Components/Images/BaseLoader";

export default class MyForm {
  static elements() {
    return {
      formEmpresa: "formEmpresa",
      formMatricula: "formMatricula",
      formAcesso: "formAcesso",
      formVencimento: "formVencimento",
      formPass: "formPass",
      btnSubmit: "btnSubmit",
    };
  }

  static getHtml() {     
    var elements = this.elements();    
    return `${Popup.getHtml()}
          <div style="width:296px;margin-top:-32px" class="form-row-group-dark"><br>              
            <p>${LogoBig()}</p>
            <h4>Informe o número do seu<br>cartão benefícios</h4>
            <br>                    
            <div class="form-row">
              <table>
                <tr>
                  <td width='90px'>${SimpleNumberField.getHtml(elements.formEmpresa,6)}</td>
                  <td width='8px'></td>
                  <td width='90px'>${SimpleNumberField.getHtml(elements.formMatricula,6)}</td>
                  <td width='8px'></td>
                  <td width='90px'>${SimpleNumberField.getHtml(elements.formAcesso,4)}</td>
                  <td width='8px'></td>
                  <td width='90px'>${SimpleNumberField.getHtml(elements.formVencimento,4)}</td>
                </tr>
              </table>
            </div>
            <br>
            <div class="form-row-group" align="left">
              <br>
              <label for='${elements.formPass}' style='padding-left:20px'>Senha</label>
                ${PasswordField.getHtml(elements.formPass, 'A senha de seu cartão', 'A senha de seu cartão')}
              <br>
            </div>                    
            <p align='center'>
              ${BaseLoader()}
            </p>                    
            <div class="form-row">
                <div align='center' id="${elements.btnSubmit}" class="button circle block green">
                  Efetuar Login
                </div>
            </div>
            <br>
            <br>    
        </div>`;
  }

  static validate() {
    var elements = this.elements();
    var val_empresa = SimpleNumberField.validate(elements.formEmpresa);
    var val_mat = SimpleNumberField.validate(elements.formMatricula );
    var val_acesso = SimpleNumberField.validate(elements.formAcesso );
    var val_venc = SimpleNumberField.validate(elements.formVencimento );
    var valPass = SimpleNumberField.validate(elements.formPass );
    if (val_empresa && val_mat && val_acesso && val_venc && valPass)
      return true;   
    return false;
  }

}
