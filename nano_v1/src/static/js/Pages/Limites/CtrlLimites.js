
import {
  updateHTML,
  logout,  
} from "@app/Infra/Util";

import MyForm from "./Views/ViewLimite";

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

export default class LimitesPage {

  getHtml() {
    return MyForm.getHtml();
  }

  constructor(params) {
    this.params = params;

    
  }
}
