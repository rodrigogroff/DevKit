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

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            context.AdditionalResponseParameters.Add("nuEmpresa", nuEmpresa);

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new DevKitDB())
			{
                var incoming = context.UserName.Split(':');
                var emp = incoming[0];
                var login = incoming[1];

                var usuario = new User();

                usuario = usuario.Login(db, emp, login, context.Password);

				if (usuario != null)
				{
					usuario.dtLastLogin = DateTime.Now;
                 
                    db.Update(usuario);

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

					identity.AddClaim(new Claim(ClaimTypes.Name, usuario.stLogin));
                    identity.AddClaim(new Claim("nuEmpresa", "(" + usuario.empresa.nuEmpresa + ") " + usuario.empresa.stSigla));

                    var ticket = new AuthenticationTicket(identity, null);
					context.Validated(ticket);                    
                }
				else
				{
					context.SetError("invalid_grant", "Login ou senha inválida!");
					return;
				}

				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
	}
}