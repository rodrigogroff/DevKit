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
			using (var db = new DevKitDB())
			{
                var incoming = context.UserName.Split(':');
                var tipo = incoming[0];

                if (tipo == "1")
                {
                    #region - médico - 

                    var codMedico = Convert.ToInt64(incoming[1]);

                    var medico = db.Medico.
                                    Where(y => y.nuCodigo == codMedico).
                                    FirstOrDefault();

                    if (medico == null)
                    {
                        context.SetError("invalid_grant", "Código ou senha inválida!");
                        return;
                    }
                    else
                    {
                        if (medico.stSenha == null)
                        {
                            if (context.Password != medico.nuCodigo.ToString())
                            {
                                context.SetError("invalid_grant", "Código ou senha inválida!");
                                return;
                            }
                        }
                        else
                        {
                            if (medico.stSenha != context.Password)
                            {
                                context.SetError("invalid_grant", "Código ou senha inválida!");
                                return;
                            }
                        }

                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                        identity.AddClaim(new Claim(ClaimTypes.Name, medico.stNome));
                        identity.AddClaim(new Claim("nuEmpresa", codMedico.ToString()));
                        identity.AddClaim(new Claim("tipo", "1"));

                        var ticket = new AuthenticationTicket(identity, null);

                        context.Validated(ticket);
                    }

                    #endregion
                }
                else if (tipo == "4")
                {
                    #region - emissora - 

                    var emp = incoming[1];
                    var login = incoming[2];

                    var usuario = new User();

                    usuario = usuario.Login(db, emp, login, context.Password);

                    if (usuario != null)
                    {
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
                        context.SetError("invalid_grant", "Login ou senha inválida!");
                        return;
                    }

                    #endregion
                }
                else if (tipo == "5")
                {
                    #region - dba - 

                    var login = incoming[1];

                    if (login == "dba" && context.Password =="superdba")
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
                
				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
	}
}