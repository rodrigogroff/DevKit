// --------------------------------------------------
//  back and front end contracts / endpoints HERE
// --------------------------------------------------

export function Endpoints() {
  return {
    authenticate: "authenticate_v1",
    socialID: "socialID_v1",
    onboarding: "onboarding_v1",
    checkToken: "checkToken_v1",
    resendToken: "resendToken_v1",
    passRecovery: "passRecovery_v1",
    brandListing: "brandListing_v1",
  };
}

export function DtoLoginInformation(empresa,matricula,codAcesso,venc,email,login,senha,userType ) {
  return {
    empresa: empresa,
    matricula: matricula,
    codAcesso: codAcesso,
    venc: venc,
    email: email,
    login: login,
    senha: senha,
    userType: userType,
    _language: "2"
  };
}
/*
public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public string userType { get; set; }
        */

export function DtoAuthenticatedUser(payload) {
  return {
    token: payload.token,
    user: {
      email: payload.user.email,
      login: payload.user.login,
      hsh: payload.user.hsh,
    },
  };
}

export function DtoCheckNewSocial_ID(sID, _language) {
  return {
    sID: sID,
    _language: _language,
  };
}

export function DtoResendToken(sID, _language) {
  return {
    sID: sID,
    _language: _language,
  };
}

export function DtoOnboarding(sID, sName, sEmail, sPass, _language) {
  return {
    sID: sID,
    sName: sName,
    sEmail: sEmail,
    sPass: sPass,
    _language: _language,
  };
}

export function DtoCheckToken(sID, sToken, _language) {
  return {
    sID: sID,
    sToken: sToken,
    _language: _language,
  };
}

export function DtoPasswordRecovery(sEmail, _language) {
  return {
    email: sEmail,
    _language: _language,
  };
}

export function DtoBrandListing(_tag, _page, _pageSize, _orderBy) {
  return {
    tag: _tag,
    page: _page,
    pageSize: _pageSize,
    orderBy: _orderBy,    
  };
}
