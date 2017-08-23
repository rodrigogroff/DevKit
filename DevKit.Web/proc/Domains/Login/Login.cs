﻿using DataModel;
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
            //string IdUser = context.Identity.Claims.Where(x => x.Type == "IdUser").Select(x => x.Value).FirstOrDefault();
            //string Session = context.Identity.Claims.Where(x => x.Type == "Session").Select(x => x.Value).FirstOrDefault();

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            //context.AdditionalResponseParameters.Add("idUser", IdUser);
            //context.AdditionalResponseParameters.Add("session", Session);

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new AutorizadorCNDB())
			{
                var lojista = (from e in db.T_Loja
                               where e.st_loja == context.UserName
                               where e.st_senha.ToUpper() == context.Password.ToUpper()
                               select e).
                               FirstOrDefault();

                if (lojista != null)
				{
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

					identity.AddClaim(new Claim(ClaimTypes.Name, lojista.st_loja));

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
	}
}
