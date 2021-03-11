
export function getLocation() {
  var srvTag = '__API_indexPos';
  var _idx = sessionStorage.getItem(srvTag);
  if (_idx == null) _idx = 0;
  var indexPos = parseInt(_idx);

  var lstNodes = [
    //{ api_host: "https://meuconvey.conveynet.com.br", api_port: "18524" },
    { api_host: "http://localhost", api_port: "18524" },
  ];

  indexPos++;
  if (indexPos >= lstNodes.length) indexPos = 0;
  sessionStorage.setItem(srvTag, indexPos)
  return lstNodes[indexPos];
}

export function SetLanguageHTMLSelect() {
  $("#languageSel").val(parseInt(localStorage.getItem("appLanguage")) + 1);
}

export function getFromStorage(tag) {
  return localStorage.getItem(tag);
}

export function setToStorage(tag, val) {
  localStorage.setItem(tag, val);
}

export function mobileCheck() {
  return window.innerWidth < 900
}

export function CheckPopUpCloseClick(e) {
  if ($(e.target.parentElement)[0].id == "popupClose") {
    fadeOut($("#popUpSystem")[0], 120);
    return true;
  }
  return false;
}

export function IsLoading() {
  return $("#loading")[0].style.display != "none";
}

export function fadeIn(elem, ms) {
  if (!elem) return;
  if (elem.style == undefined) elem.style = {};

  elem.style.opacity = 0;
  elem.style.filter = "alpha(opacity=0)";
  elem.style.display = "inline-block";
  elem.style.visibility = "visible";

  if (ms) {
    var opacity = 0;
    var timer = setInterval(function () {
      opacity += 50 / ms;
      if (opacity >= 1) {
        clearInterval(timer);
        opacity = 1;
      }
      elem.style.opacity = opacity;
      elem.style.filter = "alpha(opacity=" + opacity * 100 + ")";
    }, 50);
  } else {
    elem.style.opacity = 1;
    elem.style.filter = "alpha(opacity=1)";
  }
}

export function fadeOut(elem, ms) {
  if (!elem) return;
  if (elem.style == undefined) elem.style = {};

  if (ms) {
    var opacity = 1;
    var timer = setInterval(function () {
      opacity -= 50 / ms;
      if (opacity <= 0) {
        clearInterval(timer);
        opacity = 0;
        elem.style.display = "none";
        elem.style.visibility = "hidden";
      }
      elem.style.opacity = opacity;
      elem.style.filter = "alpha(opacity=" + opacity * 100 + ")";
    }, 50);
  } else {
    elem.style.opacity = 0;
    elem.style.filter = "alpha(opacity=0)";
    elem.style.display = "none";
    elem.style.visibility = "hidden";
  }
}

export function slideUp(element, duration, finalheight) {
  if (element.style == undefined) element.style = {};
  var s = element.style;
  s.height = "0px";
  var y = 0;
  var framerate = 10;
  var one_second = 1000;
  var interval = (one_second * duration) / framerate;
  var totalframes = (one_second * duration) / interval;
  var heightincrement = finalheight / totalframes;
  var tweenUp = function () {
    y -= heightincrement;
    s.height = y + "px";
    if (y < 0) {
      setTimeout(tweenUp, interval);
    }
  };
  tweenUp();
}

export function slideDown(element, duration, finalheight) {
  if (element.style == undefined) element.style = {};
  var s = element.style;
  s.height = "0px";
  var y = 0;
  var framerate = 10;
  var one_second = 1000;
  var interval = (one_second * duration) / framerate;
  var totalframes = (one_second * duration) / interval;
  var heightincrement = finalheight / totalframes;
  var tweenDown = function () {
    y += heightincrement;
    s.height = y + "px";
    if (y < finalheight) {
      setTimeout(tweenDown, interval);
    }
  };
  tweenDown();
}

export function loadingOn(btn) {
  $("#loading").show();
  if (btn != undefined) {
    $(btn).removeClass("green");
    $(btn).addClass("light");
    sessionStorage.setItem("myBtn", btn);
  }
}

export function loadingOff(btn) {
  $("#loading").hide();
  if (btn != undefined) $(btn).addClass("green");
}

export function displaySystemSuccess(_title, _text) {
  $("#popupSymbol").removeClass("fa-exclamation-circle");
  $("#popupSymbol").addClass("fa-check-circle");
  document.getElementById("popupSymbol").style.color = "green";
  displaySystemPopup(_title, _text);
}

export function displaySystemPopup(_title, _text) {
  var btn = sessionStorage.getItem("myBtn");
  $("#loading").hide();
  if (btn != undefined) $(btn).addClass("green");
  fadeIn($("#popUpSystem")[0], 60);
  $("#popUpSystemTitle").text(_title);
  $("#popUpSystemText").text(_text);
}

export function displaySystemPopupConfirm(mdlID, _title, _text) {
  fadeIn($("#popUpSystemConfirm_" + mdlID)[0], 60);
  $("#popUpSystemConfTitle").text(_title);
  $("#popUpSystemConfText").text(_text);
}

export function imageChange(id, img) {
  $(id).attr("src", "src/static/img/" + img);
}

export function updateHTML(idElement, html) {
  $("#" + idElement).html(html);
}

export function disableButton(btn) {
  $(btn).prop("disabled", true);
}

export function enableButton(btn) {
  $(btn).prop("disabled", false);
}

export function getFromSession(tag) {
  return sessionStorage.getItem(tag);
}

export function setToSession(tag, val) {
  sessionStorage.setItem(tag, val);
}

export function mockServer(val) {
  sessionStorage.setItem("mockServer", val);
}

export function isFieldContentValid(val, maxLength, fieldType, minlength) {
  if (fieldType == undefined) fieldType = "text";
  if (maxLength == undefined) maxLength = 20;
  if (minlength == undefined) minlength = 4;
  if (fieldType == "email") maxLength = 99;

  if (val === null) return false;
  if (val === undefined) return false;
  if (val === "") return false;

  var len = val.trim().length;

  if (len === 0) return false;
  if (len > maxLength) return false;
  if (len < minlength) return false;

  switch (fieldType) {
    case "number":
      if (!/^\d*\.?\d*$/.test(String(val).toLowerCase())) return false;
      break;
    case "email":
      const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      if (!re.test(String(val).toLowerCase())) return false;
      break;
    case "name":
      if (len.length < 3) return false;
      var reN = /^\d$/;
      for (let i = 0; i < val.length; i++) {
        let c = val[i];
        if (reN.test(c)) return false;
      }
      break;
    case "fullName":
      if (val.trim().indexOf(" ") < 0) return false;
      const ren = "'!@#$%¨&*()-_+=,.;/<>:?`´^~{[]}";
      for (let i = 0; i < val.length; ++i)
        for (let t = 0; t < ren.length; ++t) if (val[i] == ren[t]) return false;
      break;
  }

  return true;
}

export function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("email");
  localStorage.removeItem("user_name");
}

export function loginOk(resp) {

  console.log('resp');
  console.log(resp);

  localStorage.setItem("token", resp.token);
  localStorage.setItem("email", resp.user.email);
  localStorage.setItem("nome", resp.user.nome);
}

export function isAuthenticated() {
  var ret  = localStorage.getItem("token");
  if (ret == null || ret == undefined)
    return null;

  var ret = {
    email: localStorage.getItem("email"),
    nome: localStorage.getItem("nome"),
  };

  console.log(ret);

  return ret;
}

export function getTokenPortal(location, parameters) {
  var mock = sessionStorage.getItem("mockServer");
  if (mock)
    return new Promise((resolve, reject) => {
      setTimeout(function () {
        var data = JSON.parse(mock);
        if (data.payload == true)
          resolve({
            ok: true,
            payload: data.obj,
          });
        else
          reject({
            ok: false,
            msg: data.obj.message,
          });
      }, 500);
    });

  var ApiLocation = getLocation();
  var ret = new Promise((resolve, reject) => {
    var _params = "";
    if (parameters !== null) _params = "?" + parameters;
    fetch(
      ApiLocation.api_host +
      ":" +
      ApiLocation.api_port +
      "/api/" +
      location +
      _params,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + localStorage.getItem("token"),
        },
      }
    )
      .then((res) => {
        if (res.status === 401) {
          resolve({
            ok: false,
            unauthorized: true,
          });
        } else if (res.status === 400) {
          res.json().then((data) => {
            reject({
              ok: false,
              msg: data.message,
            });
          });
        } else if (res.ok === true) {
          res.json().then((data) => {
            resolve({
              ok: true,
              payload: data,
            });
          });
        } else
          res.json().then((data) => {
            resolve({
              ok: false,
              msg: data.message,
            });
          });
      })
      .catch((errorMsg) => {
        ret = false;
        resolve({
          ok: false,
          msg: errorMsg.message,
        });
      });
  });
  return ret;
}

export function postTokenPortal(location, _obj) {
  var mock = sessionStorage.getItem("mockServer");
  if (mock)
    return new Promise((resolve, reject) => {
      setTimeout(function () {
        var data = JSON.parse(mock);
        if (data.payload == true)
          resolve({
            ok: true,
            payload: data.obj,
          });
        else
          reject({
            ok: false,
            msg: data.obj.message,
          });
      }, 500);
    });

  var ApiLocation = getLocation();
  var obj = JSON.stringify(_obj);
  return new Promise((resolve, reject) => {
    fetch(
      ApiLocation.api_host + ":" + ApiLocation.api_port + "/api/" + location,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + localStorage.getItem("token"),
        },
        body: obj,
      }
    )
      .then((res) => {
        if (res.status === 401) {
          reject({
            ok: false,
            unauthorized: true,
          });
        } else if (res.status === 400) {
          res.json().then((data) => {
            reject({
              ok: false,
              msg: data.message,
            });
          });
        } else if (res.ok === true) {
          res.json().then((data) => {
            resolve({
              ok: true,
              payload: data,
            });
          });
        } else
          res.json().then((data) => {
            resolve({
              ok: false,
              msg: data.message,
            });
          });
      })
      .catch((errorMsg) => {
        resolve({
          ok: false,
          msg: errorMsg.toString(),
        });
      });
  });
}

export function postPublicPortal(location, _obj) {
  var mock = sessionStorage.getItem("mockServer");
  if (mock)
    return new Promise((resolve, reject) => {
      setTimeout(function () {
        var data = JSON.parse(mock);
        if (data.payload == true)
          resolve({
            ok: true,
            payload: data.obj,
          });
        else
          reject({
            ok: false,
            msg: data.obj.message,
          });
      }, 500);
    });

  var ApiLocation = getLocation();
  var obj = JSON.stringify(_obj);
  return new Promise((resolve, reject) => {
    fetch(
      ApiLocation.api_host + ":" + ApiLocation.api_port + "/api/" + location,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: obj,
      }
    )
      .then((res) => {
        if (res.status === 400) {
          res.json().then((data) => {
            reject({
              ok: false,
              msg: data.message,
            });
          });
        } else if (res.ok === true) {
          res.json().then((data) => {
            resolve({
              ok: true,
              payload: data,
            });
          });
        } else {
          res.json().then((data) => {
            resolve({
              ok: false,
              msg: data.message,
            });
          });
        }
      })
      .catch((errorMsg) => {
        resolve({
          ok: false,
          msg: errorMsg.toString(),
        });
      });
  });
}

export function buildTableSimple(tableobj) {  

  var color = 'black';
  var bgColor = 'white';
  var styleClass = ''; 
  var noResultMsg = '';

  if (styleClass == undefined) styleClass = '';
  if (noResultMsg == undefined) noResultMsg = "No results found";
  var lineData = "";
  if (tableobj.data.length > 0) {
    var size = tableobj.header.length;
    var lineData =
      "<table class='" +
      styleClass +
      "' id='" +
      tableobj.id +
      "'><thead style='background-color:" +
      bgColor +
      "';'> <tr>";
    for (var h = 0; h < size; ++h)
      lineData +=
        "<th style='color:" +
        color +
        "' width='" +
        tableobj.sizes[h] +
        "'>" +
        tableobj.header[h] +
        "</th > ";
    lineData += "</tr></thead><tbody>";
    for (var d = 0; d < tableobj.data.length; ++d) {
      var ar = tableobj.data[d];
      lineData += "<tr>";
      for (var h = 0; h < size; ++h)
        lineData +=
          "<td id='" +
          d +
          "' _par_table='" +
          tableobj.id +
          "'>" +
          ar[h] +
          "</td>";
      lineData += "</tr>";
    }
    lineData += "</tbody></table>";
  } else {
    lineData =
      "<br><button class='button circle'>" + noResultMsg + "</button><br>";
  }

  return lineData;
}

export function buildTable(tableobj, color, bgColor, styleClass, noResultMsg) {  
  if (styleClass == undefined) styleClass = '';
  if (noResultMsg == undefined) noResultMsg = "No results found";
  var lineData = "";
  if (tableobj.data.length > 0) {
    var size = tableobj.header.length;
    var lineData =
      "<table class='" +
      styleClass +
      "' id='" +
      tableobj.id +
      "'><thead style='background-color:" +
      bgColor +
      "';'> <tr>";
    for (var h = 0; h < size; ++h)
      lineData +=
        "<th style='color:" +
        color +
        "' width='" +
        tableobj.sizes[h] +
        "'>" +
        tableobj.header[h] +
        "</th > ";
    lineData += "</tr></thead><tbody>";
    for (var d = 0; d < tableobj.data.length; ++d) {
      var ar = tableobj.data[d];
      lineData += "<tr>";
      for (var h = 0; h < size; ++h)
        lineData +=
          "<td id='" +
          d +
          "' _par_table='" +
          tableobj.id +
          "'>" +
          ar[h] +
          "</td>";
      lineData += "</tr>";
    }
    lineData += "</tbody></table>";
  } else {
    lineData =
      "<br><button class='button circle'>" + noResultMsg + "</button><br>";
  }
  return lineData;
}
