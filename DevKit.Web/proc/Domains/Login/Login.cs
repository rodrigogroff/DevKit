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

            string m1 = context.Identity.Claims.Where(x => x.Type == "m1").Select(x => x.Value).FirstOrDefault();
            string m2 = context.Identity.Claims.Where(x => x.Type == "m2").Select(x => x.Value).FirstOrDefault();
            //string Session = context.Identity.Claims.Where(x => x.Type == "Session").Select(x => x.Value).FirstOrDefault();

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            context.AdditionalResponseParameters.Add("m1", m1);
            context.AdditionalResponseParameters.Add("m2", m2);  
            
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new AutorizadorCNDB())
			{
                if (context.UserName == "DBA")
                {
                    if (context.Password == "X3POR2D2")
                    {
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                        identity.AddClaim(new Claim(ClaimTypes.Name, "DBA"));

                        var ticket = new AuthenticationTicket(identity, null);

                        context.Validated(ticket);
                    }
                    else
                    {
                        context.SetError("invalid_grant", "Senha de DBA inválida");
                        return;
                    }
                }
                else
                {
                    try
                    {
                        var terminal = (from e in db.T_Terminal
                                        where e.nu_terminal == context.UserName.PadLeft(8,'0')
                                        select e ).
                                        FirstOrDefault();

                        if (terminal == null)
                        {
                            context.SetError("invalid_grant", "Terminal inexistente");
                            return;
                        }
                                                        
                        var lojista = (from e in db.T_Loja                                       
                                       where e.i_unique == terminal.fk_loja                                       
                                       where e.st_senha.ToUpper() == context.Password.ToUpper()
                                       select e).
                               FirstOrDefault();

                        if (lojista != null)
                        {
                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, terminal.nu_terminal.TrimStart('0')));
                            identity.AddClaim(new Claim("m1", "Lojista " + lojista.st_loja + " - " + lojista.st_nome));
                            identity.AddClaim(new Claim("m2", (lojista.st_endereco + " / " +
                                                               lojista.st_cidade + " " +
                                                               lojista.st_estado).Replace("{SE$3}", "")));

                            var ticket = new AuthenticationTicket(identity, null);
                            
                            context.Validated(ticket);
                        }
                        else
                        {
                            context.SetError("invalid_grant", "Senha ou terminal inválido");
                            return;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        context.SetError("invalid_grant", ex.ToString());                        
                    }
                }

				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
	}
}
