
import { Endpoints, DtoLoginInformation, DtoAuthenticatedUser } from "@app/Infra/Endpoints";

import {
  postPublicPortal,
  IsLoading,
  loadingOn,
  loadingOff,
  loginOk,
  mockServer,
  CheckPopUpCloseClick,
  updateHTML,
  displaySystemPopup,
} from "@app/Infra/Util";

import MyForm from "./Views/ViewLogin";
import PasswordField from "@app/Components/Fields/Password";

window.addEventListener("resize", (e) => {
  var curWidth = window.innerWidth;
  var update = false;
  var lastRes = sessionStorage.getItem('res');
  if (lastRes != null && lastRes != undefined) {
    if (parseInt(lastRes) != curWidth)
      update = true;
  }
  else
    update = true;
  if (update == true)
    updateHTML("mainFormApp", MyForm.getHtml());
  sessionStorage.setItem('res', curWidth);
});

export default class LoginPage {

  getHtml() {
    return MyForm.getHtml();
  }

  constructor(params) {
    this.params = params;

    $(document).ready(function () {
      var elements = MyForm.elements();
      $("#" + elements.formEmpresa)[0].focus();
      VMasker($("#" + elements.formEmpresa)).maskPattern("999999");
      VMasker($("#" + elements.formMatricula)).maskPattern("999999");
      VMasker($("#" + elements.formAcesso)).maskPattern("9999");
      VMasker($("#" + elements.formVencimento)).maskPattern("9999");
      VMasker($("#" + elements.formPass)).maskPattern("999999");
    });

    $(document).on("keydown", function (e) {
      switch (e.keyCode) {
        case 13: btnSubmit_Click(); break;
      }
    });

    document.body.addEventListener("click", (e) => {
      if (CheckPopUpCloseClick(e))
        return;
      var elements = MyForm.elements();
      switch ($(e.target).attr("id")) {
        case elements.btnSubmit: btnSubmit_Click(); break;
        case "seePass" + elements.formPass: PasswordField.btnSeePassword(elements.formPass); break;
      }
    });

    function btnSubmit_Click() {
      if (IsLoading()) return;
      var elements = MyForm.elements();
      var formData = DtoLoginInformation(
        document.getElementById(elements.formEmpresa).value.trim(),
        document.getElementById(elements.formMatricula).value.trim(),
        document.getElementById(elements.formAcesso).value.trim(),
        document.getElementById(elements.formVencimento).value.trim(),
        '', // email
        '', // login
        document.getElementById(elements.formPass).value.trim(),
        '2' // tipo de login       
      );

      if (!MyForm.validate()) {        
        displaySystemPopup('Aviso do Sistema', 'Preencha os campos corretamente!');
        return;
      }       

      loadingOn("#" + elements.btnSubmit);

      if (sessionStorage["mock"])
        mockServer(
          JSON.stringify({
            payload: true,
            obj: {
              token: "aaaa",
              user: {
                email: "aaa@aa",
                login: "aaaa",
                hsh: "_1",
              },
            },
          })
        );

      setTimeout(() => {
        postPublicPortal(Endpoints().authenticate, formData)
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
      }, 1000);
    }

    function serviceOk(payload) {
      loadingOff();
      var response = DtoAuthenticatedUser(payload);
      loginOk(response);      
      location.href = "/";
    }
  }
}
