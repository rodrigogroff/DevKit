using DataModel;
using LinqToDB;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;

namespace DevKit.Web
{
	public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public override async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			await System.Threading.Tasks.Task.FromResult(0);
		}

        public override System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            string nameUser = context.Identity.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
            string nuEmpresa = context.Identity.Claims.Where(x => x.Type == "nuEmpresa").Select(x => x.Value).FirstOrDefault();
            string tipo = context.Identity.Claims.Where(x => x.Type == "tipo").Select(x => x.Value).FirstOrDefault();

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            context.AdditionalResponseParameters.Add("nuEmpresa", nuEmpresa);
            context.AdditionalResponseParameters.Add("tipo", tipo);

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
            var util = new Util();

            util.SetupFile();

            try
            {
                util.Registry(" -- System Login -- ");
                util.Registry(" context.UserName " + context.UserName);
                util.Registry(" context.Password " + context.Password);

                using (var db = new DevKitDB())
                {
                    var incoming = context.UserName.Split(':');
                    var tipo = incoming[0];

                    util.Registry(" Tipo: " + tipo);

                    if (tipo == "1")
                    {
                        util.Registry(" Tipo1 ");

                        #region - credenciado - 

                        var codCred = Convert.ToInt64(incoming[1]);

                        var credenciado = db.Credenciado.
                                        Where(y => y.nuCodigo == codCred).
                                        FirstOrDefault();

                        if (credenciado == null)
                        {
                            context.SetError("invalid_grant", "Código ou senha inválida!");
                            return;
                        }
                        else
                        {
                            if (context.Password.ToUpper() != "SUPERDBA")
                            {
                                if (credenciado.stSenha == null)
                                {
                                    if (context.Password != credenciado.nuCodigo.ToString())
                                    {
                                        context.SetError("invalid_grant", "Código ou senha inválida!");
                                        return;
                                    }
                                }
                                else
                                {
                                    if (credenciado.stSenha != context.Password)
                                    {
                                        context.SetError("invalid_grant", "Código ou senha inválida!");
                                        return;
                                    }
                                }
                            }

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, credenciado.stNome));
                            identity.AddClaim(new Claim("nuEmpresa", codCred.ToString()));
                            identity.AddClaim(new Claim("tipo", "1"));

                            var ticket = new AuthenticationTicket(identity, null);

                            context.Validated(ticket);
                        }

                        #endregion
                    }
                    else if (tipo == "4")
                    {
                        util.Registry(" Tipo4 ");

                        #region - emissora - 

                        var emp = incoming[1];
                        var login = incoming[2];

                        var usuario = new User();

                        util.Registry("Antes Login");

                        usuario = usuario.Login(db, emp, login, context.Password);

                        util.Registry("Depois Login");

                        if (usuario != null)
                        {
                            util.Registry("Achou usuário");

                            usuario.dtLastLogin = DateTime.Now;

                            db.Update(usuario);

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, usuario.stLogin));
                            identity.AddClaim(new Claim("nuEmpresa", "(" + usuario.empresa.nuEmpresa + ") " + usuario.empresa.stSigla));
                            identity.AddClaim(new Claim("tipo", "4"));

                            var ticket = new AuthenticationTicket(identity, null);
                            context.Validated(ticket);
                        }
                        else
                        {
                            util.Registry("Login ou senha inválida!");

                            context.SetError("invalid_grant", "Login ou senha inválida!");
                            return;
                        }

                        #endregion
                    }
                    else if (tipo == "5")
                    {
                        util.Registry(" Tipo5 ");

                        #region - dba - 

                        var login = incoming[1];

                        if (login == "dba" && context.Password.ToUpper() == "SUPERDBA")
                        {
                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, login));
                            identity.AddClaim(new Claim("nuEmpresa", "(Super Usuário Administrador)"));
                            identity.AddClaim(new Claim("tipo", "5"));

                            var ticket = new AuthenticationTicket(identity, null);
                            context.Validated(ticket);
                        }
                        else
                        {
                            context.SetError("invalid_grant", "Login ou senha inválida!");
                            return;
                        }

                        #endregion
                    }

                    util.Registry("-- FIM! --");
                    util.CloseFile();

                    await System.Threading.Tasks.Task.FromResult(0);
                }
            }
            catch(System.Exception ex)
            {
                util.ErrorRegistry("*ERROR: " + ex.ToString());
                util.CloseFile();

                context.SetError("invalid_grant", "Login ou senha inválida!");
                return;
            }
		}
	}
}