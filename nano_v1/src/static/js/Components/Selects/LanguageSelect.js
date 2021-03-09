export default class {
  static getHtml(langs) {
    var availableLanguages = "";
    for (let i = 0; i < langs.length; ++i)
      availableLanguages +=
        "<option value='" + (i + 1).toString() + "'>" + langs[i] + "</option>";
    return `<select class="form-element" id='languageSel'>${availableLanguages}</select>`;
  }
}
