using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Mail;

namespace GetStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Patch?");

            using (var db = new AutorizadorCNDB())
            {
                var strPatch = Console.ReadLine();

                strPatch = strPatch.ToUpper();

                switch (strPatch)
                {
                    case "TESTEEMAIL":
                        {
                            var param_usuario = "conveynet@conveynet.com.br";
                            var email = "gvnfraga@gmail.com,rodrigo.groff@gmail.com";

                            using (var client = new SmtpClient
                            {
                                Port = 587,
                                Host = "smtp.conveynet.com.br",
                                EnableSsl = false,
                                Timeout = 10000,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new System.Net.NetworkCredential(param_usuario, "c917800")
                            })
                            {
                                try
                                {
                                    string assunto = "bla bla assunto", texto = "este é o texto!";

                                    Console.WriteLine("Email:" + email);

                                    var mm = new MailMessage(param_usuario,
                                                               email,
                                                               assunto,
                                                               texto)
                                    {
                                        BodyEncoding = UTF8Encoding.UTF8,
                                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                                    };

                                    client.Send(mm);
                                }
                                catch (SystemException ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }

                            break;
                        }

                    case "FIXABRIL9620":
                    case "WHATEVER":
                    {
                        var dtIni = new DateTime(2018, 3, 2);
                        var dtFim = new DateTime(2018, 3, 28).AddDays(1);

                        var lstEmpresas = "009620,009621,009622,009623,009624".Split (',');

                        var lst = ( from e in db.LOG_Fechamento
                                    join empDb in db.T_Empresa on e.fk_empresa equals (int) empDb.i_unique
                                    join parc in db.T_Parcelas on e.fk_parcela equals (int) parc.i_unique
                                    join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int) ltr.i_unique
                                    where ltr.dt_transacao > dtIni && ltr.dt_transacao < dtFim
                                    where lstEmpresas.Contains(empDb.st_empresa)
                                    where e.st_mes == "03" && e.st_ano == "2018"
                                    select e).                                        
                                    ToList();

                        foreach (var item in lst)
                        {
                            item.st_mes = "04";

                            db.Update(item);
                        }

                        break;
                    }
                }
            }
        }
    }
}
