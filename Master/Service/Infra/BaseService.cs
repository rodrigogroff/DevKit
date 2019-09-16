using Entities.Api;
using System;
using System.Net.Mail;

namespace Master.Service
{
    public class BaseService
    {
        public const string _defaultError = "Ops, aconteceu um imprevisto",
                            databaseName = "portal_on";

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
    }
}
