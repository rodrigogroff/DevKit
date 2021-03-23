
import {
  updateHTML,
  getTokenPortal,
  displaySystemPopup,
} from "@app/Infra/Util";

import { Endpoints } from "@app/Infra/Endpoints";

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

    $(document).ready(function () {
      getTokenPortal(Endpoints().limites, null)
        .then((resp) => {
          if (resp.ok == true)
            serviceOk(resp.payload);
          // output service data
          else
            displaySystemPopup('Aviso do Sistema', resp.msg);
        })
        .catch((resp) => {
          displaySystemPopup('Aviso do Sistema', resp.msg);
        });
    });
    
    function serviceOk(payload) {
      MyForm.update(payload);
    }
  }
 
}
