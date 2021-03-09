
import ImgLoader from "@app/Components/Images/ImageLoader";

export default class {
  static getHtml() {
    return `<div id="formSplash" style='width:100%;height:100%;'>
            ${ImgLoader.getDirectHtml(
      "logoSplash",
      "src/static/img/splash.png",
      "max-width:100%;max-height:100%;padding-top:90px"
    )}
    </div>`;
  }
}
