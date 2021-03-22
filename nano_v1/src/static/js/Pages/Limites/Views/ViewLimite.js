
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
                    <td ><div class="button" style="padding:5px">Maio / 2021</div><br></td>
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
                    <td ><div class="button" style="padding:5px">R$ 0,00</div><br></td>
                    <td ></td>
                    <td ><div class="button" style="padding:5px">R$ 0,00</div><br></td>
                    <td ></td>
                  </tr>
                  <tr >
                    <td colspan='4' align='left'>Parcela Mensal Utilizada</td>
                  </tr>
                  <tr >
                    <td ><div class="button" style="padding:5px">R$ 0,00</div><br></td>
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
                    <td ><div class="button" style="padding:5px">R$ 0,00</div><br></td>
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
                    <td ><div class="button" style="padding:5px">12</div><br></td>
                    <td ></td>
                    <td ><div class="button" style="padding:5px">19</div><br></td>
                    <td ></td>
                  </tr>
                  </table>
                </div>                
              </div>
              <br>         
          </div>`;
  }
}
