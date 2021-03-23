using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using Npgsql;
using System.Text;
using RestSharp;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Master.Infra;
using Master.Data.Domains;

namespace Master.Service
{
    public class SrvBaseService 
    {
        public bool _sendEmail = true;
        public IMemoryCache serverCache = null;

        public DtoServiceError Error;
        
        #region - const values - 

        public const string _emailSmtp = "server@nanojs.com.br";
        public const string _passwordSmtp = "Fernanda@2012";
        public const string _smtp = "email-ssl.com.br";
        public const int _smtpPort = 587;

        #endregion

        public NpgsqlConnection GetConnection(LocalNetwork network)
        {
            #region - code - 

            var ret = new NpgsqlConnection(network.sqlServer);
            ret.Open();
            return ret;

            #endregion
        }

        public int I(string myNumber)
        {
            return Convert.ToInt32(myNumber);
        }

        public string money(long vr)
        {
            decimal d = Convert.ToDecimal(vr) / 100;
            return d.ToString("C2").Split(' ')[1];
        }

        public DateTime D(string myDate)
        {
            return new DateTime(I(myDate.Substring(6)),
                                I(myDate.Substring(3, 2)),
                                I(myDate.Substring(0, 2)));
        }
       
        public void SendEmail(string _email, string subject, string texto, List<string> attachs = null)
        {
            if (!_sendEmail)
                return;

            #region - code - 

            MailMessage email = new MailMessage
            {
                From = new MailAddress("<" + _emailSmtp + ">")
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
                Credentials = new System.Net.NetworkCredential(_emailSmtp, _passwordSmtp),//e-mail e senha do remetente
                Host = "smtp." + "nanojs.com.br",                    
                Port = 587
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

        public string GetCachedData(string tagCache, string cacheServer, int minutes_boost)
        {
            #region - code - 

            try
            {
                // check for internal cache
                string data;
                if (serverCache.TryGetValue(tagCache, out data))
                    return data;

                if (cacheServer == null)
                    return null;
                
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
                if (minutes_boost > 0)
                    serverCache.Set(tagCache, final, DateTimeOffset.Now.AddMinutes(minutes_boost));

                // return to service
                return final;
            }
            catch
            {
                return null;
            }

            #endregion
        }

        public void UpdateCachedData(string tagCache, string cacheValue, string cacheServer, int minutes_boost)
        {
            #region - code - 

            try
            {
                if (cacheServer == null)
                {
                    if (minutes_boost > 0)
                        serverCache.Set(tagCache, cacheValue, DateTimeOffset.Now.AddMinutes(minutes_boost));
                    else
                        serverCache.Set(tagCache, cacheValue);
                }
                else
                {
                    // update cache server
                    var restRequest = new RestRequest("api/updateCache", Method.GET);
                    var final = JObject.Parse(cacheValue).ToString().Replace("\r\n", "");
                    restRequest.AddParameter("_tag", tagCache);
                    restRequest.AddParameter("_value", final);
                    var cli = new RestClient(cacheServer);
                    cli.Execute(restRequest);

                    // update internal
                    if (minutes_boost > 0)
                        serverCache.Set(tagCache, cacheValue, DateTimeOffset.Now.AddMinutes(minutes_boost));
                }                
            } 
            catch (System.Exception ex)
            {
                var st = ex.ToString();
            }

            #endregion
        }

        public string OnlyNumbers(string cpf)
        {
            #region - code - 

            var strResp = "";

            for (int i = 0; i < cpf.Length; i++)
            {
                var c = cpf[i];
                if (Char.IsNumber(c))
                    strResp += c.ToString();
            }

            return strResp;

            #endregion
        }

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

            return retorno.TrimStart('*');

            #endregion
        }

        public static string DESCript(string dados, string chave = "12345678")
        {
            #region - code -

            dados = dados.PadLeft(8, '*');

            byte[] key = Encoding.ASCII.GetBytes(chave);
            byte[] data = Encoding.ASCII.GetBytes(dados);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = key;
            des.Mode = CipherMode.ECB;

            ICryptoTransform DESt = des.CreateEncryptor();
            DESt.TransformBlock(data, 0, 8, data, 0);

            string retorno = "";
            for (int n = 0; n < 8; n++)
            {
                retorno += String.Format("{0:X2}", data[n]);
            }

            return retorno;

            #endregion
        }

        public string ObterCodigoAcesso (long? empresa, long? matricula, int titularidade, long? via, string cpf)
        {
            #region - code - 

            string chave = matricula.ToString().PadLeft(6,'0') + empresa.ToString().PadLeft(6, '0') + titularidade.ToString().PadLeft(2, '0') + via + cpf.PadRight(14, ' ');

            int temp = 0;
            for (int n = 0; n < chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");

            temp += int.Parse(calculado[3].ToString()) * 1000;
            temp += int.Parse(calculado[2].ToString());
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring(2, 1) +
                        calculado.Substring(0, 1) +
                        calculado.Substring(3, 1) +
                        calculado.Substring(1, 1);

            return calculado.ToString().PadLeft(4, '0');

            #endregion
        }        
    }
}
