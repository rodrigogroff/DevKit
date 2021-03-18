
import {
  updateHTML,
  logout,  
} from "@app/Infra/Util";

import MyForm from "./Views/ViewExit";

window.addEventListener("resize", (e) => { 
  var curWidth = window.innerWidth;
  var update = false;
  var lastRes = sessionStorage.getItem('res');
  if (lastRes != null && lastRes != undefined)
  {
    if (parseInt(lastRes) != curWidth)
      update = true;
  }
  else  
    update = true;
  if(update == true)
    updateHTML("mainFormApp", MyForm.getHtml()); 
  sessionStorage.setItem('res', curWidth);
});

export default class ExitPage {

  getHtml() {
    return MyForm.getHtml();
  }

  constructor(params) {
    this.params = params;

    $(document).on("keydown", function (e) {
      switch (e.keyCode) {
        case 13: btnSubmit_Click(); break;
      }
    });

    document.body.addEventListener("click", (e) => {
      var elements = MyForm.elements();
      switch ($(e.target).attr("id")) {
        case elements.btnSubmit: btnSubmit_Click(); break;        
      }
    });

    function btnSubmit_Click() {
      logout();
      location.href = "/login";
    }
  }
}
