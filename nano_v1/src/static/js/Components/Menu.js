
import { isAuthenticated } from "@app/Infra/Util";

export class MenuAppLanguage {
  constructor() {
    this.availableLanguages = [
      "English",
      "Español",
      "Português BR",
    ];

    this.langs = [
      // english (0)
      {
        labels: [
          "Control panel" /* 0 */,
          "User" /* 1 */,
          "Login" /* 2 */,
          "Register" /* 3 */,
          "Dashboard" /* 4 */,
          "Welcome" /* 5 */,
          "Exit" /* 6 */,
          "Admin" /* 7 */,
          "Brand" /* 8 */,
        ],
      },
      // spanish (1)
      {
        labels: [
          "Panel de control" /* 0 */,
          "Usuario" /* 1 */,
          "Iniciar sesión" /* 2 */,
          "Registrarse" /* 3 */,
          "Dashboard" /* 4 */,
          "Bienvenido" /* 5 */,
          "Salir" /* 6 */,
          "Administración" /* 7 */,
          "Marca" /* 8 */,
        ],
      },
      // ptBr (2)
      {
        labels: [
          "Painel de controle" /* 0 */,
          "Usuário" /* 1 */,
          "Login" /* 2 */,
          "Registrar" /* 3 */,
          "Dashboard" /* 4 */,
          "Bem-vindo" /* 5 */,
          "Sair" /* 6 */,
          "Administração" /* 7 */,
          "Marca" /* 8 */,
        ],
      },
    ];
  }
}

export function Menu_MultiLanguage(index) {
  var curLanguage = localStorage.getItem("appLanguage");
  if (curLanguage == undefined) {
    curLanguage = 0;
    // ptBR default
    localStorage.setItem("appLanguage", curLanguage);
  }
  curLanguage = parseInt(curLanguage);
  var appLang = new MenuAppLanguage();
  return appLang.langs[curLanguage].labels[index];
}

export function BuildMenuTranslation() {
  return {
    pnlControl: Menu_MultiLanguage(0),
    userItem: Menu_MultiLanguage(1),
    loginItem: Menu_MultiLanguage(2),
    registerItem: Menu_MultiLanguage(3),
    dashboardItem: Menu_MultiLanguage(4),
    welcome: Menu_MultiLanguage(5),
    exit: Menu_MultiLanguage(6),
    admin: Menu_MultiLanguage(7),
    brand: Menu_MultiLanguage(8),
  };
}

export default class {
  static getHtml() {
    
    var itens = BuildMenuTranslation();

    var usrMsg = "";
    var fullMenu = `<li>
                    <a href="javascript:void(0);">${itens.userItem}</a>
                    <ul>
                      <li><a href="/login" style='cursor:pointer'>${itens.loginItem}</a></li>
                      <li><a href="/register" style='cursor:pointer'>${itens.registerItem}</a></li>
                    </ul>
                  </li>                  
                  `;

    var usr = isAuthenticated();

    if (usr != null) {
      usrMsg = `<p style='color:white;padding-left:20px'>${itens.welcome}<br>
                  <span style='padding-left:30px'>${usr.user_name}</span><br>
                  <br>
                  <a href="/exit" style='cursor:pointer'>${itens.exit}</a>
                </p>
                `;
      fullMenu = `<li>
                  <a href="javascript:void(0);">${itens.admin}</a>
                  <ul>
                    <li><a href="/brand" style='cursor:pointer'>${itens.brand}</a></li>
                  </ul>
                </li>
                `;
    }

    return `<div class="nav-menu" align='left'><nav class="menu">                
        <div class="nav-container">
          ${usrMsg}
          <ul class="main-menu">
            <li class="active">
              <a href="javascript:void(0);">${itens.pnlControl}</a>
              <ul>
								<li><a href="/" style='cursor:pointer'>${itens.dashboardItem}</a></li>								
							</ul>
            </li>
						${fullMenu}
          </ul>
          <br>          
          <br>
          <br>
				</div>
			</nav>
		</div>`;
  }
}
