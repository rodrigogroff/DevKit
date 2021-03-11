
/*
import { 
  getFromSession, 
  setToSession, 
  fadeIn, 
  fadeOut,
} from "@app/Infra/Util";

import ViewSplash from "./Views/ViewSplash";
*/

export default class {
  constructor(params) {
    this.params = params;

    $(document).ready(function () {
      /*
      if (getFromSession("splashed") !== "true") {
        setToSession("splashed", "true");
        $("body").css("background-color", "#FFFFFF");
        $("#splash").html(ViewSplash.getHtml());
        $("#appHeader").css("display", "none");
        setTimeout(function () {
          fadeIn($("#splash")[0], 60);
          setTimeout(function () {
            fadeOut($("#splash")[0], 60);
            setTimeout(function () {
              $("#splash").css("display", "none");
              $("body").css("background-color", "#CFCFCF");
              $("#appHeader").css("display", "block");
              fadeIn($("#appHeader")[0], 30);
              $('#mainForm').show();
            }, 400);
          }, 2000);
        }, 900);
      }
      */
    });
  }

  getHtml() {
    return "";
  }
}
