
import { isAuthenticated } from "@app/Infra/Util";
import { LogoWhite } from "@app/Components/Images/Logo";

export default class {

  static getHtml() {
    
    var usr = isAuthenticated();

    var fullMenu = `<li class='active'>
                    <a href="javascript:void(0);"><b>Acesso ao Sistema</b></a>
                    <ul>
                      <li><a href="/login" style='cursor:pointer'>Login Associado</a></li>
                    </ul>
                  </li>`;

    var usrMsg = '';

    if (usr != null) {
      usrMsg = `<p style='color:white;padding-left:20px'>Bem-vindo,<br>
                  ${usr.nome}<br>
                  <br>
                  <a href="/exit" style='cursor:pointer'>Sair</a>
                </p>`;

      fullMenu = `<li class='active'>
                  <a href="javascript:void(0);"><b>Cartão Convênio</b></a>
                  <ul>
                    <li><a href="/" style='cursor:pointer'>Cartão virtual</a></li>
                    <li><a href="/" style='cursor:pointer'>Solicitações</a></li>
                    <li><a href="/" style='cursor:pointer'>Limites</a></li>
                    <li><a href="/" style='cursor:pointer'>Extrato Atual</a></li>
                    <li><a href="/" style='cursor:pointer'>Meus Parcelamentos</a></li>
                    <li><a href="/" style='cursor:pointer'>Faturas Passadas</a></li>
                    <li><a href="/" style='cursor:pointer'>Rede Lojas</a></li>
                  </ul>
                </li>`;
    }

    return `<div class="nav-menu" align='left'><nav class="menu">
              <div class="nav-container">
                <br>  
                ${LogoWhite()}
                <br>    
                ${usrMsg}
                <br>
                <ul class="main-menu">
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
