
  export default class {
    static getHtml(id,length) {
      var numeric = 'type="tel" pattern="[0-9]*" inputmode="numeric"';
      if (length == undefined || length== null)
        length = 20;
      return `<input id="${id}" type="text" class="form-element" ${numeric} placeholder="" maxlength='${length}'>`;
    }

  static validate(id) {
    var val = document.getElementById(id).value.trim()
    if (isNaN(val) || val == undefined || val == null || val == '')
      return false;
    return true;
  }
}
