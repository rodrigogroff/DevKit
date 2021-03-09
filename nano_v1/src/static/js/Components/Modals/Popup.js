export default class {
  static getHtml() {
    return `<div class="popup-overlay" id="popUpSystem" align='center'>
                <div class="popup-container" style='margin-top:50px;max-width:350px'>
                    <div class="popup-header">
                        <h3 style='padding-left:32px' class="popup-title" id='popUpSystemTitle'></h3>
                        <span id='popupClose' class="popup-close" data-dismiss="true">X</span>
                    </div>
                    <div class="popup-content">
                        <img src='src/static/img/error_big.png' style='width:60px;height:60px' alt=' ' />
                        <span style="font-size:64px;color:red;padding-top:32px;" id='popupSymbol' class="fa fa-exclamation-circle"></span><br>
                        <br><span id='popUpSystemText' style="padding-top:16px;"></span><br><br>
                    </div>
                </div>
            </div>`;
  }
}
