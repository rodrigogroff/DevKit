
export default class MyForm {
  static elements() {
    return {
      btnSubmit: "btnSubmit",
    };
  }

  static getHtml() {
    var elements = this.elements();

    return `<div style="width:296px" class="form-row-group-dark"><br>                
              <div class="form-row txt-center">
                <h4>Sair do sistema?</h4>
                <div class="form-row">
                  <div align='center' id="${elements.btnSubmit}" class="button circle block green">
                    Confirmar
                  </div>
                </div>
              </div>
              <br>         
          </div>`;
  }
}
