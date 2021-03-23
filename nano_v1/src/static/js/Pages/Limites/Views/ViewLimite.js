
import Popup from "@app/Components/Modals/Popup";

export default class MyForm {
  static elements() {
    return {
      limiteCartao: "limiteCartao",
      parcelas: "parcelas",
      limiteMensalDisp: "limiteMensalDisp",
      cotaExtra: "cotaExtra",
      melhorDia: "melhorDia",
      mensalUtilizado: "mensalUtilizado",
      mesVigente: "mesVigente",
      pct: "pct",
    };
  }

  static getHtml() {
    var elements = this.elements();

    return `${Popup.getHtml()}
            <div style="width:296px" class="form-row-group-dark"><br>   
              <div class="form-row txt-center">
                <div class="button orange" style="padding:5px">Portal / Limites do Associado</div>
              </div>              
              <div class="form-row txt-center">
                <div class="form-row">
                  <table style='padding-left:20px'>
                  <tr>
                    <td width='120px' align='left'>Vigência</td>
                    <td width='20px'></td>
                    <td width='120px'></td>
                    <td width='20px'></td>
                  </tr>
                  <tr>
                    <td ><div class="button" style="padding:5px"><span id='${elements.mesVigente}'></span></div><br></td>
                    <td ></td>
                    <td ></td>
                    <td ></td>
                  </tr>
                  <tr>
                    <td align='left'>Limite Mensal</td>
                    <td ></td>
                    <td align='left'>Disponível</td>
                    <td ></td>
                  </tr>
                  <tr>
                    <td ><div class="button" style="padding:5px"><span id='${elements.limiteCartao}'></span></div><br></td>
                    <td ></td>
                    <td ><div class="button" style="padding:5px"><span id='${elements.limiteMensalDisp}'></span></div><br></td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td colspan='4' align='left'>Parcela Mensal Utilizada</td>
                  </tr>
                  <tr >
                    <td ><div class="button" style="padding:5px"><span id='${elements.mensalUtilizado}'></span></div><br></td>
                    <td ></td>
                    <td ></td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td align='left'>Cota Extra</td>
                    <td ></td>
                    <td ></td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td ><div class="button" style="padding:5px"><span id='${elements.cotaExtra}'></span></div><br></td>
                    <td ></td>
                    <td ></td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td align='left'>Parcelamento</td>
                    <td ></td>
                    <td align='left'>Melhor dia</td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td ><div class="button" style="padding:5px"><span id='${elements.parcelas}'></span></div><br></td>
                    <td ></td>
                    <td ><div class="button" style="padding:5px"><span id='${elements.melhorDia}'></span></div><br></td>
                    <td ></td>
                  </tr>
                  </table>
                </div>                
              </div>
              <br>         
          </div>`;
  }

  static update (payload) {
    var elements = this.elements();    
    $("#" + elements.mesVigente).text(payload.mesVigente);
    $("#" + elements.limiteCartao).text(payload.limiteCartao);
    $("#" + elements.limiteMensalDisp).text(payload.limiteMensalDisp);
    $("#" + elements.mensalUtilizado).text(payload.mensalUtilizado);
    $("#" + elements.cotaExtra).text(payload.cotaExtra);
    $("#" + elements.parcelas).text(payload.parcelas);
    $("#" + elements.melhorDia).text(payload.melhorDia);
  }
}
