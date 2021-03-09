export default class {
  static getHtml(el, id, text) {
    return `<${el} id='${el}'>${text}</${el}>`;
  }
}
