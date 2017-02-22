using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using LinqToDB;
using DataModel;

namespace App.Web
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
			using (var db = new SuporteCITDB())
			{
				var usuario = (from element in db.Usuarios
							   where element.StLogin.ToUpper() == context.UserName.ToUpper()
							   where element.StPassword.ToUpper() == context.Password.ToUpper()
							   select element).FirstOrDefault();

				if (usuario != null)
				{
					usuario.DtUltimoLogin = DateTime.Now;

					db.Update(usuario);
					
					usuario = usuario.Load(db);

					var identity = new ClaimsIdentity(context.Options.AuthenticationType);

					identity.AddClaim(new Claim(ClaimTypes.Name, usuario.StLogin));
					identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Perfil.StNome));
					identity.AddClaim(new Claim(ClaimTypes.Sid, usuario.Perfil.Id.ToString()));

					identity.AddClaim(new Claim("IdUsuario", usuario.Id.ToString()));
					identity.AddClaim(new Claim("IdPerfil", usuario.Perfil.Id.ToString()));					

					var ticket = new AuthenticationTicket(identity, null);
					context.Validated(ticket);
				}
				else
				{
					context.SetError("invalid_grant", "Usuário ou senha inválidos");
					return;
				}

				await Task.FromResult(0);
			}
		}
		
		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			string nomeUsuario = context.Identity.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
			string idPerfil = context.Identity.Claims.Where(x => x.Type == "IdPerfil").Select(x => x.Value).FirstOrDefault();

			context.AdditionalResponseParameters.Add("nomeUsuario", nomeUsuario);
			context.AdditionalResponseParameters.Add("idPerfil", Convert.ToInt32(idPerfil));

			return Task.FromResult<object>(null);
		}
	}
}