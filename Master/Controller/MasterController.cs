using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Entities.Api.User;
using Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Api.Master.Controllers
{
    [Authorize]
    [ApiController]
    public partial class MasterController : ControllerBase
    {
        public LocalNetwork network { get; set; }
        public IMemoryCache _cache { get; set; }

        public bool _doNotSendEmail = false,
                      _doNotUseCache = false;


        public MasterController(IOptions<LocalNetwork> _network)
        {
            if (_network != null)
                this.network = _network.Value;

            _cache = new MemoryCache(new MemoryCacheOptions { });
        }

        [NonAction]
        protected DtoAuthenticatedUser GetCurrentAuthenticatedUser()
        {
            #region - code -

            var handler = new JwtSecurityTokenHandler();
            var authHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(authHeader))
                return null;

            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var claims = tokenS.Claims;

            return new DtoAuthenticatedUser
            {
                _id = claims.FirstOrDefault(claim => claim.Type == "_id")?.Value,
                email = claims.FirstOrDefault(claim => claim.Type == "email")?.Value,
                nome = claims.FirstOrDefault(claim => claim.Type == "login")?.Value,
                _type = claims.FirstOrDefault(claim => claim.Type == "userType")?.Value,
            };

            #endregion
        }

        [NonAction]
        public string ComposeTokenForSession(DtoAuthenticatedUser au)
        {
            #region - code - 

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(LocalNetwork.Secret);

            if (au.email == null) au.email = "";
            if (au.nome == null) au.nome = "";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("_id", au._id),
                    new Claim("_type", au._type),
                    new Claim("nome", au.nome),
                    new Claim("email", au.email),                    
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

            #endregion
        }

        [NonAction]
        public long I(string myNumber)
        {
            return Convert.ToInt64(myNumber);
        }

        [NonAction]
        public DateTime D(string myDate)
        {
            return new DateTime((int)I(myDate.Substring(6)),
                                (int)I(myDate.Substring(3, 2)),
                                (int)I(myDate.Substring(0, 2)));
        }

        [NonAction]
        public void SendEmail(string _email, string subject, string texto, LocalNetwork network, List<string> attachs = null)
        {
            if (!_doNotSendEmail)
                return;

            #region - code - 

            MailMessage email = new MailMessage
            {
                From = new MailAddress("<" + network._emailSmtp + ">")
            };
            email.To.Add(_email);
            email.Priority = MailPriority.Normal;
            email.IsBodyHtml = false;
            email.Subject = subject;
            email.Body = texto;

            email.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
            email.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
            SmtpClient emailSmtp = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(network._emailSmtp, network._passwordSmtp),//e-mail e senha do remetente
                Host = network._hostSmtp, // "smtp." + "nanojs.com.br",
                Port = (int)I(network._smtpPort) // 587
            };

            try
            {
                emailSmtp.Send(email);
            }
            catch (Exception erro)
            {
                throw new Exception("erro: " + erro.Message);
            }

            #endregion
        }

        [NonAction]
        public string GetCachedData(string tagCache, string cacheServer, int minutes_boost)
        {
            #region - code - 

            if (_doNotUseCache)
                return null;

            try
            {
                // check for internal cache
                string data;
                if (_cache.TryGetValue(tagCache, out data))
                    return data;

                // search cache server
                var restRequest = new RestRequest("api/getCache", Method.GET);
                restRequest.AddParameter("_tag", tagCache);
                var cli = new RestClient(cacheServer);
                var retCache = cli.Execute(restRequest);
                if (retCache.StatusCode.ToString() == "BadRequest")
                    return null;
                var re = retCache.Content;
                var r1 = re.Substring(1, re.Length - 2);
                var final = r1.Replace("\\\"", "\"");

                // update internal
                _cache.Set(tagCache, final, DateTimeOffset.Now.AddMinutes(minutes_boost));

                // return to service
                return final;
            }
            catch
            {
                return null;
            }

            #endregion
        }

        [NonAction]
        public void UpdateCachedData(string tagCache, string cacheValue, string cacheServer, int minutes_boost)
        {
            #region - code - 

            if (_doNotUseCache)
                return;

            try
            {
                // update cache server
                var restRequest = new RestRequest("api/updateCache", Method.GET);
                var final = JObject.Parse(cacheValue).ToString().Replace("\r\n", "");
                restRequest.AddParameter("_tag", tagCache);
                restRequest.AddParameter("_value", final);
                var cli = new RestClient(cacheServer);
                cli.Execute(restRequest);

                // update internal
                _cache.Set(tagCache, final, DateTimeOffset.Now.AddMinutes(minutes_boost));
            }
            catch
            {

            }

            #endregion
        }

        [NonAction]
        public string OnlyNumbers(string text)
        {
            #region - code - 

            var strResp = "";

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (Char.IsNumber(c))
                    strResp += c.ToString();
            }

            return strResp;

            #endregion
        }

        [NonAction]
        public static string DESdeCript(string dados, string chave = "12345678")
        {
            #region - code - 

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

            #endregion
        }
    }
}
