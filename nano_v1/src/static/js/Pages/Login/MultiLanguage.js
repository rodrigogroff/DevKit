
export class AppLanguage {
  constructor() {
    this.availableLanguages = ["English", "Español", "Português BR"];

    this.langs = [
      // english (0)
      {
        labels: [
          "Forgot your password?" /*0*/,
          "Login" /*1*/,
          "Not registered?" /*2*/,
          "Sign up!" /*3*/,
          "Email" /*4*/,
          "Error" /*5*/,
          "Invalid email!" /*6*/,
          "Password" /*7*/,
          "See password" /*8*/,
          "Invalid password" /*9*/,
          "Language" /*10*/,
          "Keep me logged in" /*11*/,
          "Dashboard" /*12*/,
          "User" /*13*/,
          "Login" /*14*/,
          "Register" /*15*/,
          "Next" /*16*/,
          "Step" /*17*/,
          "Inform your social ID" /*18*/,
          "Social ID" /*19*/,
          "Inform your full Name" /*20*/,
          "Your full name" /*21*/,
          "Previous" /*22*/,
          "Inform your email" /*23*/,
          "Confirmation" /*24*/,
          "Invalid social ID!" /*25*/,
          "Invalid name!" /*26*/,
          "Numeric Code" /*27*/,
          "Enter your code sent by email" /*28*/,
          "Invalid number!" /*29*/,
        ],
      },
      // spanish (1)
      {
        labels: [
          "¿Olvidó su contraseña?" /*0*/,
          "Iniciar sesión" /*1*/,
          "¿No estás registrado?" /*2*/,
          "¡Regístrate!" /*3*/,
          "Correo electrónico" /*4*/,
          "Error" /*5*/,
          "¡Correo electrónico no válido!" /*6*/,
          "Contraseña" /*7*/,
          "Ver contraseña" /*8*/,
          "Contraseña no válida" /*9*/,
          "Idioma" /*10*/,
          "Mantenerme conectado" /*11*/,
          "Panel de control" /*12*/,
          "Usuario" /*13*/,
          "Iniciar sesión" /*14*/,
          "Registrarse" /*15*/,
          "Siguiente" /*16*/,
          "Paso" /*17*/,
          "Informe su identificación social" /*18*/,
          "Identificación social" /*19*/,
          "Informe su nombre completo" /*20*/,
          "Su nombre completo" /*21*/,
          "Anterior" /*22*/,
          "Informe su correo electrónico" /*23*/,
          "Confirmación" /*24*/,
          "¡ID social no válido!" /*25*/,
          "¡Nombre no válido!" /*26*/,
          "Código numérico" /*27*/,
          "Ingrese su código enviado por correo electrónico" /*28*/,
          "¡Número no válido!" /*29*/,
          "Por favor, informe un correo electrónico registrado válido para recibir su última contraseña" /*30*/,
          "¡Correo electrónico enviado!" /*31*/,
        ],
      },
      // ptBr (2)
      {
        labels: [
          "Esqueceu sua senha?" /*0*/,
          "Login" /*0*/,
          "Não registrado?" /*0*/,
          "Inscreva-se!" /*0*/,
          "Email" /*0*/,
          "Erro" /*0*/,
          "Email inválido" /*0*/,
          "Senha" /*0*/,
          "Ver senha" /*0*/,
          "Senha inválida" /*0*/,
          "Idioma" /*0*/,
          "Mantenha-me conectado" /*0*/,
          "Dashboard" /*0*/,
          "Usuário" /*0*/,
          "Login" /*0*/,
          "Registrar" /*0*/,
          "Próximo" /*0*/,
          "Passo" /*0*/,
          "Informe seu CPF" /*0*/,
          "CPF" /*0*/,
          "Informe nome completo" /*0*/,
          "Seu nome completo" /*0*/,
          "Anterior" /*0*/,
          "Informe seu email" /*0*/,
          "Confirmação" /*0*/,
          "CPF inválido!" /*0*/,
          "Nome inválido!" /*0*/,
          "Código numérico" /*0*/,
          "Informe seu código enviado por email" /*0*/,
          "Numero inválido!" /*0*/,
        ],
      },
    ];
  }
}

export function getCurrentLanguage() {
  return parseInt(localStorage.getItem("appLanguage"));
}

export function MultiLanguageChange(index) {
  var curLanguage = localStorage.getItem("appLanguage");
  if (curLanguage != index - 1) {
    localStorage.setItem("appLanguage", index - 1);
    return true;
  }
  return false;
}

export function MultiLanguage(index) {
  var curLanguage = localStorage.getItem("appLanguage");
  if (curLanguage == undefined) {
    curLanguage = 2;
    // ptBR default
    localStorage.setItem("appLanguage", curLanguage);
  }
  curLanguage = parseInt(curLanguage);

  var appLang = new AppLanguage();

  return appLang.langs[curLanguage].labels[index];
}
