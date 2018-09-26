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
            Console.WriteLine("------------------------");
            Console.WriteLine("(0.21) Patch?");
            Console.WriteLine("------------------------");

            using (var db = new AutorizadorCNDB())
            {
                var strPatch = Console.ReadLine();

                strPatch = strPatch.ToUpper();

                switch (strPatch)
                {
                    case "VLRDIFERE":
                        {
                            Console.WriteLine("Buscando parcelas....");
                            var lstParcsConfirmadas = (from e in db.T_Parcelas
                                                       join eTr in db.LOG_Transacoes on (int)e.fk_log_transacoes equals eTr.i_unique
                                                       where eTr.tg_confirmada.ToString() == "1"
                                                       where eTr.dt_transacao > DateTime.Now.AddMonths(-9)
                                                       select e).
                                                       OrderByDescending ( y=> y.dt_inclusao ).
                                                       ToList();

                            Console.WriteLine("Buscando parcelas.... " + lstParcsConfirmadas.Count());

                            var lstVendas = lstParcsConfirmadas.Select(y => y.fk_log_transacoes).Distinct().ToList();
                         //   var trans = db.LOG_Transacoes.Where ( y=> lstVendas.Contains((int)y.i_unique)).ToList();

                            int tot = lstVendas.Count();
                            int t = 0;

                            foreach (var venda in lstVendas)
                            {
                                ++t;

                                Console.Write("\r Vendas.... " + t + " / " + tot);

                                var tLog = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == venda);
                                var total = tLog.vr_total;

                                // busca parcelas

                                var totalParc = lstParcsConfirmadas.Where (y=> y.fk_log_transacoes == venda).Sum(y => y.vr_valor);

                                if (total != totalParc)
                                    Console.WriteLine(" >>>> " + venda + " => T " + total + " P " + totalParc + " {" + tLog.st_msg_transacao.Trim() 
                                        + "} [" + Convert.ToDateTime(tLog.dt_transacao).ToString("dd/MM/yyyy") + " - NSU:" + tLog.nu_nsu);
                            }

                            break;
                        }

                    default:
                    case "semTrans":
                        {
                            var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes == null).ToList();

                            var nsuLst = lstParcs.Select(y => y.nu_nsu).Distinct().ToList();

                            foreach (var nsu in nsuLst)
                            {
                                var t_parcOriginal = lstParcs.FirstOrDefault(y => y.nu_nsu == nsu && y.nu_indice == 1);

                                var novaTrans = new LOG_Transaco
                                {
                                    dt_transacao = t_parcOriginal.dt_inclusao,
                                    en_operacao = "0",
                                    fk_cartao = t_parcOriginal.fk_cartao,
                                    fk_empresa = t_parcOriginal.fk_empresa,
                                    fk_loja = t_parcOriginal.fk_loja,
                                    fk_terminal = t_parcOriginal.fk_terminal,
                                    nu_cod_erro = 0,
                                    nu_nsu = t_parcOriginal.nu_nsu,
                                    nu_nsuOrig = 0,
                                    nu_parcelas = lstParcs.Count(y => y.nu_nsu == nsu),
                                    st_doc = "",
                                    st_msg_transacao = "Corrigida",
                                    tg_confirmada = '1',
                                    tg_contabil = '1',
                                    vr_saldo_disp = 0,
                                    vr_saldo_disp_tot = 0,
                                    vr_total = lstParcs.Sum(y => y.vr_valor)
                                };

                                // --------------------------------------------------------------------------------------
                                // insere a LOG_TRANSACAO Faltante
                                // --------------------------------------------------------------------------------------

                                novaTrans.i_unique = Convert.ToDecimal(db.InsertWithIdentity(novaTrans));

                                foreach (var parc in lstParcs.Where (y => y.nu_nsu == nsu))
                                {
                                    var parcUpd = db.T_Parcelas.Find(parc.i_unique);

                                    parcUpd.fk_log_transacoes = Convert.ToInt32(novaTrans.i_unique);
                                    parcUpd.nu_parcela--;

                                    // --------------------------------------------------------------------------------------
                                    // atualiza nas T_Parcelas a LOG_TRANS correspondente 
                                    // --------------------------------------------------------------------------------------

                                    db.Update(parcUpd);
                                }

                                // insere a LOG_FECHAMENTO (só uma) de acordo

                                foreach (var parc in lstParcs.Where(y => y.nu_nsu == nsu && y.nu_indice == 1))
                                {
                                    var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == parc.fk_cartao);

                                    var fechNew = new LOG_Fechamento
                                    {
                                        dt_compra = parc.dt_inclusao,
                                        dt_fechamento = DateTime.Now,
                                        fk_cartao = parc.fk_cartao,
                                        fk_empresa = parc.fk_empresa,
                                        fk_loja = parc.fk_loja,
                                        fk_parcela = Convert.ToInt32(parc.i_unique),
                                        nu_parcela = 1,
                                        st_afiliada = "",
                                        st_ano = "2018",
                                        st_mes = "06",
                                        st_cartao = cart.st_empresa + cart.st_matricula + cart.st_titularidade,
                                        vr_valor = parc.vr_valor
                                    };

                                    fechNew.i_unique = Convert.ToDecimal(db.InsertWithIdentity(fechNew));
                                }
                            }

                            break;
                        }

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
