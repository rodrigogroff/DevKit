using DataModel;
using DevKit.Web.Controllers;
using LinqToDB;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
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
            string tipo = context.Identity.Claims.Where(x => x.Type == "tipo").Select(x => x.Value).FirstOrDefault();

            context.AdditionalResponseParameters.Add("nameUser", nameUser);
            context.AdditionalResponseParameters.Add("m1", m1);
            context.AdditionalResponseParameters.Add("m2", m2);
            context.AdditionalResponseParameters.Add("tipo", tipo);

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new AutorizadorCNDB())
			{
                var tipo = context.UserName[0].ToString();

                var UserName = context.UserName.Substring(1);

                switch (tipo)
                {
                    case "1": // lojista e DBA
                        {
                            if (UserName == "DBA")
                            {
                                #region - code - 

                                if (context.Password == "X3POR2D2")
                                {
                                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                                    identity.AddClaim(new Claim(ClaimTypes.Name, "DBA"));
                                    identity.AddClaim(new Claim("tipo", "1"));

                                    var ticket = new AuthenticationTicket(identity, null);

                                    context.Validated(ticket);
                                }
                                else
                                {
                                    context.SetError("invalid_grant", "Senha de DBA inválida");
                                    return;
                                }

                                #endregion
                            }
                            else
                            {
                                #region - DBA e lojista - 

                                try
                                {
                                    var terminal = (from e in db.T_Terminal
                                                    where e.nu_terminal == UserName.PadLeft(8, '0')
                                                    select e).
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

                                        identity.AddClaim(new Claim("tipo", "1"));

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

                                #endregion
                            }

                            break;
                        }

                    case "2": // usuário
                        {
                            var dados = UserName.Split('.');

                            string  empresa = dados[1].PadLeft(6,'0'),
                                    matricula = dados[2].PadLeft(6, '0'),
                                    codAcesso = dados[3],
                                    venc = dados[4];

                            var associadoPrincipal = (from e in db.T_Cartao
                                                      where e.st_empresa == empresa
                                                      where e.st_matricula == matricula
                                                      where e.st_titularidade == "01"
                                                      select e).
                                                      FirstOrDefault();

                            if (associadoPrincipal == null)
                            {
                                context.SetError("invalid_grant", "Autenticação de cartão inválida");
                                return;
                            }

                            var tEmp = (from e in db.T_Empresa where e.st_empresa == empresa select e).FirstOrDefault();

                            if (associadoPrincipal.st_venctoCartao != venc)
                            {
                                context.SetError("invalid_grant", "Autenticação de cartão inválida");
                                return;
                            }

                            var dadosProprietario = (from e in db.T_Proprietario
                                                     where e.i_unique == associadoPrincipal.fk_dadosProprietario
                                                     select e).
                                                     FirstOrDefault();
                                                        
                            var codAcessoCalc = new CodigoAcesso().Obter (  empresa,
                                                                            matricula,
                                                                            associadoPrincipal.st_titularidade,
                                                                            associadoPrincipal.nu_viaCartao,
                                                                            dadosProprietario.st_cpf );
                            
                            if (codAcessoCalc != codAcesso)
                            {
                                context.SetError("invalid_grant", "Autenticação de cartão inválida");
                                return;
                            }

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, associadoPrincipal.st_empresa +  "." + associadoPrincipal.st_matricula));
                            
                            identity.AddClaim(new Claim("m1", tEmp.st_fantasia ));
                            identity.AddClaim(new Claim("m2", dadosProprietario.st_nome ));

                            identity.AddClaim(new Claim("tipo", "2"));

                            var ticket = new AuthenticationTicket(identity, null);

                            context.Validated(ticket);

                            break;
                        }
                }

				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
	}
}
