
export class AppLanguage {
  constructor() {
    this.availableLanguages = ["English", "Español", "Português BR"];

    this.langs = [
      // english (0)
      {
        labels: [
          "Exit System?" /*0*/,
          "Confirm" /*1*/,
        ],
      },
      // spanish (1)
      {
        labels: [
          "¿Salir del sistema?" /*0*/,
          "Confirmar" /*1*/,
        ],
      },
      // ptBr (2)
      {
        labels: [
          "Sair do Sistema?" /*0*/,
          "Confirmar" /*1*/,
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
