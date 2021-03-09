
export default class {
  constructor(totItens, itensPerPage, currentPage) {
    this.totItens = totItens;
    this.itensPerPage = itensPerPage;
    this.currentPage = currentPage;
  }

  getHtml() {
    var injectTotItens = this.totItens;
    var initPage = this.currentPage - 5;
    if (initPage < 1) initPage = 1;
    var endPage = this.totItens / this.itensPerPage;
    if (endPage < 0) endPage = 1;
    var startRes = (this.currentPage - 1) * this.itensPerPage + 1;
    var endRes = startRes + this.itensPerPage - 1;
    if (endRes > this.totItens) endRes = this.totItens;
    var previous = this.currentPage - 1;
    if (previous < 1) previous = 1;
    var next = this.currentPage + 1;
    if (next > endPage) next = endPage;

    var buttonHtml = "<button class='buttonSmall'>";
    var buttonSelHtml = "<button class='buttonSmall green'>";
    var term = "</button>";
    var injectButtons =
      buttonHtml.replace("class=", "_page='" + 1 + "' class=") +
      "First" +
      term +
      buttonHtml.replace("class=", "_page='" + previous + "' class=") +
      "Previous" +
      term;

    var maxDisplayItens = 10;
    while (initPage < endPage) {
      if (initPage == this.currentPage)
        injectButtons += buttonSelHtml + initPage + term;
      else
        injectButtons +=
          buttonHtml.replace("class=", "_page='" + initPage + "' class=") +
          initPage +
          term;
      initPage++;
      maxDisplayItens--;
      if (maxDisplayItens == -1) break;
    }

    injectButtons +=
      buttonHtml.replace("class=", "_page='" + next + "' class=") +
      "Next" +
      term +
      buttonHtml.replace("class=", "_page='" + endPage + "' class=") +
      "Last" +
      term;

    return `<div align='center'>
                <table width='900px'>
                    <tr height='52px'>
                        <td valign='middle' width='250px'><span style='font-size:small'>Showing results [${startRes} to ${endRes}] of ${injectTotItens}</span></td>
                        <td valign='middle'>${injectButtons}</td>
                    </tr>
                </table>
            </div>`;
  }
}
