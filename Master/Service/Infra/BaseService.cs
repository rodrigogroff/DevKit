using Entities.Api;
using Master.Repository;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Master.Service
{
    public class BaseService
    {
        public IDapperRepository repository;

        public const string _defaultError = "Ops, aconteceu um imprevisto",
                            databaseName = "portal_on";

        public BaseService()
        {

        }

        public BaseService(IDapperRepository repo)
        {
            repository = repo;
        }
               
        public ServiceError Error;                

        public int I(string myNumber)
        {
            return Convert.ToInt32(myNumber);
        }

        public DateTime D(string myDate)
        {
            return new DateTime(I(myDate.Substring(6)), 
                                I(myDate.Substring(3, 2)), 
                                I(myDate.Substring(0, 2)));
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

        public string DESCript(string dados, string chave = "12345678")
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

        /*
        public static void SendMailMessage ( LocalNetwork network, 
                                             string to, 
                                             bool isHtml, 
                                             string subject, 
                                             string body, 
                                             string caminhoAnexo )
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(network.mail_from);

            string[] parts = to.Split(';');
            foreach (string s in parts)
                mailMessage.To.Add(new MailAddress(s));

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtml;
            mailMessage.Priority = MailPriority.Normal;

            SmtpClient smtpClient;

            string host = network.mail_host;
            string portStr = network.mail_port;
            string sslStr = network.mail_ssl;
            string user = network.mail_user;
            string pass = network.mail_pass;
            
            int port;

            if (Int32.TryParse(portStr, out port))
            {
                smtpClient = new SmtpClient(host, port);

            }
            else
            {
                smtpClient = new SmtpClient(host);
            }

            if ((!String.IsNullOrEmpty(sslStr)) && (sslStr.Equals("S")))
            {
                smtpClient.EnableSsl = true;
            }
            else
            {
                smtpClient.EnableSsl = false;
            }

            if (!string.IsNullOrEmpty(user))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(user, pass);
            }

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception falha)
            {
                
            }
        }
        */
    }
}
