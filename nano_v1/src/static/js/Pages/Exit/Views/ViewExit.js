
import { MultiLanguage } from "../MultiLanguage";

export default class MyForm {
  static elements() {
    return {
      btnSubmit: "btnSubmit",
    };
  }

  static getHtml() {
    var elements = this.elements();
    var title = MultiLanguage(0);
    var buttonTitle = MultiLanguage(1);

    return `<div style="width:296px" class="form-row-group-dark"><br>                
              <div class="form-row txt-center">
                <h4>${title}</h4>
                <div class="form-row">
                  <div align='center' id="${elements.btnSubmit}" class="button circle block green">
                    ${buttonTitle}
                  </div>
                </div>
              </div>
              <br>         
          </div>`;
  }
}
