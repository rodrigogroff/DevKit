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

		public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new DevKitDB())
			{
				var usuario = new User().Login(db,context.UserName, context.Password);

				if (usuario != null)
				{
					usuario.dtLastLogin = DateTime.Now;
                    usuario.stCurrentSession = usuario.GetRandomString(16);

                    db.Update(usuario);

					var identity = new ClaimsIdentity(context.Options.AuthenticationType);

					identity.AddClaim(new Claim(ClaimTypes.Name, usuario.stLogin));
					identity.AddClaim(new Claim(ClaimTypes.Role, usuario.profile.stName));
					identity.AddClaim(new Claim(ClaimTypes.Sid, usuario.profile.id.ToString()));
					identity.AddClaim(new Claim("IdUser", usuario.id.ToString()));
                    identity.AddClaim(new Claim("Session", usuario.stCurrentSession.ToString()));

                    var ticket = new AuthenticationTicket(identity, null);
					context.Validated(ticket);
				}
				else
				{
					context.SetError("invalid_grant", "Invalid login / password!");
					return;
				}

				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
		
		public override System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			string nameUser = context.Identity.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
            string IdUser = context.Identity.Claims.Where(x => x.Type == "IdUser").Select(x => x.Value).FirstOrDefault();
            string Session = context.Identity.Claims.Where(x => x.Type == "Session").Select(x => x.Value).FirstOrDefault();

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            context.AdditionalResponseParameters.Add("idUser", IdUser);
            context.AdditionalResponseParameters.Add("session", Session);

            return System.Threading.Tasks.Task.FromResult<object>(null);
		}
	}
}