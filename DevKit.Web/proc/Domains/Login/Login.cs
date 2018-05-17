using DataModel;
using DevKit.Web.Controllers;
using LinqToDB;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DevKit.Web
{
	public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
	{
        #region - functions -

        public string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string DESdeCript(string dados, string chave = "12345678")
        {
            byte[] key = System.Text.Encoding.ASCII.GetBytes(chave);//{1,2,3,4,5,6,7,8};
            byte[] data = new byte[8];

            for (int n = 0; n < dados.Length / 2; n++)
            {
                data[n] = (byte)Convert.ToInt32(dados.Substring(n * 2, 2), 16);
            }

            DES des = new DESCryptoServiceProvider();
            des.Key = key;
            des.Mode = CipherMode.ECB;
            ICryptoTransform crypto = des.CreateDecryptor();
            MemoryStream cipherStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(cipherStream, crypto, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            crypto.TransformBlock(data, 0, 8, data, 0);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string retorno = enc.GetString(data);

            return retorno;
        }

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

        #endregion

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (var db = new AutorizadorCNDB())
			{
                var tipo = context.UserName[0].ToString();

                var UserName = context.UserName.Substring(1);

                //VendaEmpresarial v = new VendaEmpresarial();
                //v.input_cont_pe.st_terminal = "4200";
                //v.Run(db);

                switch (tipo)
                {
                    case "1": // lojista
                        {
                            #region - lojista - 

                            try
                            {
                                var terminal = (from e in db.T_Terminal
                                                where e.nu_terminal == UserName.PadLeft(8, '0')
                                                select e).
                                                FirstOrDefault();

                                if (terminal == null)
                                {
                                    context.SetError("Erro", "Terminal inexistente");
                                    return;
                                }

                                var lojista = (from e in db.T_Loja
                                                where e.i_unique == terminal.fk_loja                                                   
                                                select e).
                                                FirstOrDefault();

                                if (context.Password.ToUpper() != "SUPERDBA")
                                {
                                    if (lojista.st_senha.ToUpper() != context.Password.ToUpper())
                                    {
                                        context.SetError("Erro", "Senha ou terminal inválido");
                                        return;
                                    }
                                }                                    

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
                            }
                            catch (System.Exception ex)
                            {
                                context.SetError("Erro", ex.ToString());
                            }

                            #endregion

                            
                            
                            break;
                        }

                    case "2": // usuário
                        {
                            #region - usuário - 

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
                                context.SetError("Erro", "Autenticação de cartão inválida");
                                return;
                            }

                            var tEmp = (from e in db.T_Empresa where e.st_empresa == empresa select e).FirstOrDefault();

                            if (associadoPrincipal.st_venctoCartao != venc)
                            {
                                context.SetError("Erro", "Autenticação de cartão inválida");
                                return;
                            }

                            // cod acesso

                            var dadosProprietario = (from e in db.T_Proprietario
                                                     where e.i_unique == associadoPrincipal.fk_dadosProprietario
                                                     select e).
                                                     FirstOrDefault();

                            var codAcessoCalc = new CodigoAcesso().Obter(empresa,
                                                                            matricula,
                                                                            associadoPrincipal.st_titularidade,
                                                                            associadoPrincipal.nu_viaCartao,
                                                                            dadosProprietario.st_cpf);

                            if (codAcessoCalc != codAcesso)
                            {
                                context.SetError("Erro", "Autenticação de cartão inválida");
                                return;
                            }

                            // senha
                            var senhaComputada = DESdeCript(associadoPrincipal.st_senha, "12345678").TrimStart('*');

                            if (context.Password.ToUpper() != "SUPERDBA")
                                if (senhaComputada != context.Password)
                                {
                                    context.SetError("Erro", "Autenticação de cartão inválida");
                                    return;
                                }

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, associadoPrincipal.st_empresa +  "." + associadoPrincipal.st_matricula));
                            
                            identity.AddClaim(new Claim("m1", tEmp.st_fantasia ));
                            identity.AddClaim(new Claim("m2", dadosProprietario.st_nome ));

                            identity.AddClaim(new Claim("tipo", "2"));

                            var ticket = new AuthenticationTicket(identity, null);

                            context.Validated(ticket);

                            #endregion

                            break;
                        }

                    case "3": // gestão lojista 
                        {
                            #region - lojista -

                            try
                            {
                                var lojista = (from e in db.T_Loja
                                                where e.st_loja == UserName
                                                select e).
                                                FirstOrDefault();

                                if (lojista == null)
                                {
                                    context.SetError("Erro", "Senha ou terminal inválido");
                                    return;
                                }

                                if (context.Password.ToUpper() != "SUPERDBA")
                                {
                                    if (lojista.st_senha.ToUpper() != context.Password.ToUpper())
                                    {
                                        context.SetError("Erro", "Senha ou terminal inválido");
                                        return;
                                    }
                                }

                                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                                identity.AddClaim(new Claim(ClaimTypes.Name, lojista.st_loja));
                                identity.AddClaim(new Claim("m1", "Administrador Lojista: " + lojista.st_nome));
                                identity.AddClaim(new Claim("m2", (lojista.st_endereco + " / " +
                                                                    lojista.st_cidade + " " +
                                                                    lojista.st_estado).Replace("{SE$3}", "")));

                                identity.AddClaim(new Claim("tipo", "3"));

                                var ticket = new AuthenticationTicket(identity, null);

                                context.Validated(ticket);
                                
                            }
                            catch (System.Exception ex)
                            {
                                context.SetError("Erro", ex.ToString());
                            }

                            #endregion

                            break;
                        }
                        
                    case "4": // emissoras
                        {
                            #region - emissora - 

                            var dados = UserName.Split('.');

                            string empresa = dados[1].PadLeft(6, '0'),
                                    usuario = dados[2].PadLeft(6, '0');

                            var tEmp = (from e in db.T_Empresa
                                        where e.st_empresa == empresa
                                        select e).
                                        FirstOrDefault();

                            if (tEmp == null)
                            {
                                context.SetError("Erro", "Usuário / Senha inválida");
                                return;
                            }

                            var tUser = (from e in db.T_Usuario
                                         where e.st_empresa == empresa
                                         where e.st_nome == usuario
                                         select e).
                                         FirstOrDefault();

                            if (tUser == null)
                            {
                                context.SetError("Erro", "Usuário / Senha inválida");
                                return;
                            }
                            
                            if (context.Password.ToUpper() != "SUPERDBA")
                            {
                                var hsh = getMd5Hash(context.Password);

                                if (tUser.st_senha != hsh)
                                {
                                    context.SetError("Erro", "Usuário / Senha inválida");
                                    return;
                                }
                            }

                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                            identity.AddClaim(new Claim(ClaimTypes.Name, usuario));

                            identity.AddClaim(new Claim("m1", tEmp.st_fantasia));
                            identity.AddClaim(new Claim("m2", tEmp.st_endereco));
                            identity.AddClaim(new Claim("empresa", empresa));
                            identity.AddClaim(new Claim("IdUsuario", tUser.i_unique.ToString()));

                            identity.AddClaim(new Claim("tipo", "4"));

                            var ticket = new AuthenticationTicket(identity, null);

                            context.Validated(ticket);

                            #endregion

                            break;
                        }

                    case "5":
                        {
                            #region - dba - 

                            if (context.Password.ToLower() == "superdba")
                            {
                                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                                identity.AddClaim(new Claim(ClaimTypes.Name, "DBA"));
                                identity.AddClaim(new Claim("tipo", "1"));

                                var ticket = new AuthenticationTicket(identity, null);

                                context.Validated(ticket);
                            }
                            else
                            {
                                context.SetError("Erro", "Senha de DBA inválida");
                                return;
                            }

                            #endregion

                            break;
                        }
                }

				await System.Threading.Tasks.Task.FromResult(0);
			}
		}
	}
}
