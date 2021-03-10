
export function buildTable2TD(html1, html2, id) {
  return `<table align='center'><tr><td id='${id}' valign='middle'>${html1}</td><td valign='middle'>${html2}</td></tr></table>`;
}

export function buildTable2TDEqual(html1, html2, id) {
  return `<table align='center' width='100%'><tr><td width='50%' id='${id}' valign='middle'>${html1}</td><td valign='middle'>${html2}</td></tr></table>`;
}

export function BaseLoader(id, _style) {
  if (id == undefined) id = "loading";
  if (_style == undefined) _style = "display:none;";
  return `<p align='center'><div><span class="loadingio-spinner-spinner-4rb4hgyrsge" id='loading' style="display:none;">
          <div class="ldio-k0jb5gkv3kn"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div>
          </div>
          </span><br>
          </div>
          </p>`;
}

export function BaseLoaderIcon(id, _style) {
  if (id == undefined) id = "loading";
  if (_style == undefined) _style = "display:none;";
  return `<span class="loadingio-spinner-spinner-4rb4hgyrsge" id='loading' style="display:none;">
          <div class="ldio-k0jb5gkv3kn"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div>
          </div>
          </span>`;
}
