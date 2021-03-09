
import { MultiLanguage, AppLanguage } from "../MultiLanguage";
import ImgLoader from "@app/Components/Images/ImageLoader";
import EmailField from "@app/Components/Fields/Email";
import PasswordField from "@app/Components/Fields/Password";

import {
  buildTable2TDEqual,
  BaseLoader,
  BaseLoaderIcon,
} from "@app/Components/Images/BaseLoader";

import Label from "@app/Components/Misc/Label";
import Popup from "@app/Components/Modals/Popup";
import LanguageSelect from "@app/Components/Selects/LanguageSelect";
import { mobileCheck } from "@app/Infra/Util";

export default class MyForm {
  static elements() {
    return {
      formMail: "formMail",
      formPass: "formPass",
      btnSubmit: "btnSubmit",
    };
  }

  static getHtml() {
    if (mobileCheck()) return this.htmlMobile();
    return this.htmlDesktop();
  }

  static htmlMobile() {
    var elements = this.elements();
    var _langs = new AppLanguage().availableLanguages;
    var forgotMsg = MultiLanguage(0);
    var loginMsg = MultiLanguage(1);
    var ncadMsg = MultiLanguage(2);
    var cadHereMsg = MultiLanguage(3);
    var placeholderEmail = MultiLanguage(23);
    var placeholderPass = MultiLanguage(7);
    var placeholderPass_Title = MultiLanguage(8);
    var title = MultiLanguage(1);
    var language = MultiLanguage(10);

    return `
    ${Popup.getHtml()}
    <div style="width:296px" class="form-row-group-dark"><br>                
                    ${buildTable2TDEqual("<label for='languageSel'>" + language + "</label>", LanguageSelect.getHtml(_langs))}
                    <div class="form-row txt-center" style='padding-top:8px;padding-bottom:18px'>
                        ${buildTable2TDEqual("<h4>" + title + "</h4>", ncadMsg + " <a href='/register'>" + cadHereMsg + "</a>")}
                    </div>
                    <div class="form-row-group" align="left">
                        <label for='${elements.formMail}' style='padding-left:20px'>${placeholderEmail}</label>
                        ${EmailField.getHtml(elements.formMail, "")}
                        <br>
                        <label for='${elements.formPass}' style='padding-left:20px'>${placeholderPass}</label>
                          ${PasswordField.getHtml(elements.formPass, "", placeholderPass_Title)}
                        <br>                        
                    </div>
                    ${BaseLoader()}                    
                    <div class="form-row">
                        <div align='center' id="${elements.btnSubmit}" class="button circle block green">
                          ${loginMsg}
                        </div>
                    </div>
                    <br>
                    <div class="form-row txt-center">
                      <a href="/forgot"> ${Label.getHtml("span", "fp", forgotMsg)} </a>
                    </div>
                    <br>         
                </div>`;
  }

  static htmlDesktop() {
    var elements = this.elements();
    var _langs = new AppLanguage().availableLanguages;
    var forgotMsg = MultiLanguage(0);
    var loginMsg = MultiLanguage(1);
    var ncadMsg = MultiLanguage(2);
    var cadHereMsg = MultiLanguage(3);
    var placeholderEmail = MultiLanguage(23);
    var placeholderPass = MultiLanguage(7);
    var placeholderPass_Title = MultiLanguage(8);
    var title = MultiLanguage(1);    

    return `${Popup.getHtml()}
          <div style="width:1150px" class="form-row-group-dark"><br><br><br>
            <table width='100%'><tr>
            <td width='90px'></td>
            <td valign='top' width='550px'> 
              ${ImgLoader.getDirectHtml("logoSplash", "src/static/img/splash.png", "max-width:100%;max-height:100%;")}
            </td>
            <td width='90px'></td>
            <td valign='top' width='490px'>
              <table><tr><td width='150px'><h2>
                ${title}
              </h2></td><td>${ncadMsg} <a href='/register' style='padding-left:30px'>${cadHereMsg}</a></td></tr></table>
              <div align='left'>
                ${LanguageSelect.getHtml(_langs)}                
              </div>
              <br>
              <div class="form-row-group" align="left">
                  <label for='${elements.formMail}' style='padding-left:20px'>${placeholderEmail}</label>
                  ${EmailField.getHtml(elements.formMail, "")}
                  <br>
                  <label for='${elements.formPass}' style='padding-left:20px'>${placeholderPass}</label>
                    ${PasswordField.getHtml(elements.formPass, "", placeholderPass_Title)}
                  <br>                  
              </div>    
              ${BaseLoader()}
              <div class="form-row">
                  <div align='center' id="${elements.btnSubmit}" class="button circle block green">
                    ${loginMsg}
                  </div>
              </div>
              <br>
              <div class="form-row txt-center">
                <a href="/forgot"> ${Label.getHtml("span", "fp", forgotMsg)} </a>
              </div>
              <br>
            </td>
            <td width='90px'></td>
            </tr></table>
          <br>
          </div>`;
  }

  static validate(_params) {
    var elements = this.elements();
    var valPass = PasswordField.validate(
      elements.formPass,
      _params,
      MultiLanguage(5),
      MultiLanguage(9)
    );
    var valEmail = EmailField.validate(
      elements.formMail,
      _params,
      MultiLanguage(5),
      MultiLanguage(6)
    );
    if (!(valEmail && valPass)) return false;
    document.activeElement.blur();
    return true;
  }
}
