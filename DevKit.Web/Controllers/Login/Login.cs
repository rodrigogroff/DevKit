using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using LinqToDB;
using DataModel;
using System.Threading;

namespace DevKit.Web
{
	public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			await Task.FromResult(0);
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			Thread.Sleep(500);

			using (var db = new DevKitDB())
			{
				var usuario = (from e in db.Users
							   where e.stLogin.ToUpper() == context.UserName.ToUpper()
							   where e.stPassword.ToUpper() == context.Password.ToUpper()
							   where e.bActive == true
							   select e).FirstOrDefault();

				if (usuario != null)
				{
					usuario.dtLastLogin = DateTime.Now;

					db.Update(usuario);
					
					usuario = usuario.LoadAssociations(db);

					var identity = new ClaimsIdentity(context.Options.AuthenticationType);

					identity.AddClaim(new Claim(ClaimTypes.Name, usuario.stLogin));
					identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Profile.stName));
					identity.AddClaim(new Claim(ClaimTypes.Sid, usuario.Profile.id.ToString()));
					identity.AddClaim(new Claim("IdUser", usuario.id.ToString()));
					identity.AddClaim(new Claim("IdProfile", usuario.Profile.id.ToString()));

					var ticket = new AuthenticationTicket(identity, null);
					context.Validated(ticket);
				}
				else
				{
					context.SetError("invalid_grant", "Invalid login / password!");
					return;
				}

				await Task.FromResult(0);
			}
		}
		
		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			string nameUser = context.Identity.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
			string IdProfile = context.Identity.Claims.Where(x => x.Type == "IdProfile").Select(x => x.Value).FirstOrDefault();

			context.AdditionalResponseParameters.Add("nameUser", nameUser);
			context.AdditionalResponseParameters.Add("idProfile", Convert.ToInt32(IdProfile));

			return Task.FromResult<object>(null);
		}
	}
}