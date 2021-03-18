
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
  };
}
