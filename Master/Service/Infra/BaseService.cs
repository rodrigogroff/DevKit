using Entities.Api;
using Master.Repository;
using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;

namespace Master.Service
{
    public class BaseService
    {
        public IDapperRepository repository;

        public const string _defaultError = "Ops, aconteceu um imprevisto",
                            databaseName = "portal_on";

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
