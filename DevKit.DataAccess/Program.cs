using DataModel;
using LinqToDB;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Mail;
using System.Collections.Generic;

namespace GetStarted
{
    class Program
    {
        static void AjustaParcela(int idParcela)
        {
            Console.WriteLine(idParcela);

            using (var db = new AutorizadorCNDB())
            {
                var fech = db.LOG_Fechamento.FirstOrDefault(y => y.fk_parcela == idParcela);

                var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == idParcela);
                parcUpd.nu_parcela++;
                db.Update(parcUpd);

                db.Delete(fech);
            }
        }

        static void AjustaParcelasErradas(string arquivo, int fkEmpresa, string mes, string ano)
        {
            using (var db = new AutorizadorCNDB())
            {
                var dbEmpresa = db.T_Empresa.FirstOrDefault(y => y.i_unique == fkEmpresa);

                var lstFech = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mes && y.st_ano == ano).ToList();

                using (var sr = new StreamReader(arquivo))
                {
                    var loja = new T_Loja();

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (line.StartsWith("="))
                        {
                            loja = db.T_Loja.FirstOrDefault(y => y.st_loja == line.Replace("=", ""));

                            if (loja == null)
                                throw new Exception("ERRO: " + line + " não existe loja!");
                            else
                            {
                                Console.WriteLine(">> " + loja.st_nome);
                            }
                        }
                        else if (line.StartsWith("*"))
                        {
                            Console.WriteLine(line);

                            var dados = line.Split(';');

                            var nsu = Convert.ToInt32(dados[1]);

                            var cartao_mat = dados[3].Split('.')[0].PadLeft(6,'0');
                            var cartao_tit = dados[3].Split('.')[1];

                            var valor = Convert.ToInt64(dados[5].Replace (",","").Replace (".",""));
                            var parcela = dados[6].Split('/')[0].Trim();

                            var dbCart = db.T_Cartao.FirstOrDefault(y => y.st_empresa == dbEmpresa.st_empresa && y.st_matricula == cartao_mat && y.st_titularidade == cartao_tit);

                            if (dbCart != null)
                            {
                                var log_fech = lstFech.Where(y => y.fk_loja == loja.i_unique && 
                                                                      y.nu_parcela.ToString() == parcela && 
                                                                      y.fk_cartao == dbCart.i_unique).ToList();

                                bool found = false;

                                foreach (var itemF in log_fech)
                                {
                                    var t_parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemF.fk_parcela);

                                    if (t_parc != null)
                                    {
                                        if (t_parc.nu_nsu == nsu)
                                        {
                                            // ajusta a parcela
                                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemF.fk_parcela);
                                            parcUpd.nu_parcela++;
                                            db.Update(parcUpd);

                                            // deleta o fechamento
                                            db.Delete(log_fech);
                                            Console.WriteLine("Ajustado!");

                                            found = true;
                                        }
                                    }
                                }

                                if (!found)
                                    Console.WriteLine("Não achou fechamento!");
                            }
                            else
                            {
                                Console.WriteLine("Não achou cartão!");
                            }
                        }
                    }
                }
            }
        }

        static void ReFecha(string mes, string ano, int fkEmpresa, int anoF, int mesF, int diaF)
        {
            using (var db = new AutorizadorCNDB())
            {
                // ------------------------------
                // desfaz fechamento
                // ------------------------------

                var lstDelFech = new List<long>();

                Console.WriteLine("--------- ajustando parcelas antigas da empresa " + fkEmpresa);

                var lstOld = db.LOG_Fechamento.Where(y => y.fk_empresa == fkEmpresa && y.st_mes == mes && y.st_ano == ano).ToList();

                int counterOld = 0;

                foreach (var itemFech in lstOld)
                {
                    ++counterOld;

                    Console.WriteLine("--------- ajustando parcelas antigas " + counterOld + " de " + lstOld.Count());

                    lstDelFech.Add((long)itemFech.i_unique);

                    var parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela);
                    var logTrans = parc.fk_log_transacoes.ToString();
                    var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                    var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                    foreach (var itemParc in lstParcs)
                    {
                        if (itemParc.i_unique >= itemFech.fk_parcela)
                        {
                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                            parcUpd.nu_parcela++;
                            db.Update(parcUpd);
                        }
                    }
                }

                Console.WriteLine("--------- Limpando antigo fechamento => " + lstDelFech.Count());

                foreach (var item in lstDelFech)
                {
                    var itemF = db.LOG_Fechamento.FirstOrDefault(y => y.i_unique == item);
                    db.Delete(itemF);
                }

                // reconstroi o fechamento
                Console.WriteLine("--------- Fechamento vai ser reconstruido");

                var lst = db.T_Parcelas.Where(y => y.fk_empresa == fkEmpresa && y.nu_parcela > 0).ToList();

                int index = 0;

                foreach (var parc in lst)
                {
                    ++index;

                    Console.WriteLine("--------- Fechamento vai ser reconstruido " + index + " de " + lst.Count());

                    var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                    if (logTrans.dt_transacao > new DateTime(anoF, mesF, diaF))
                        continue;

                    if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                        continue;

                    var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                    parcUpd.nu_parcela--;
                    db.Update(parcUpd);

                    if (parcUpd.nu_parcela == 0)
                        db.Insert(new LOG_Fechamento
                        {
                            dt_compra = logTrans.dt_transacao,
                            dt_fechamento = DateTime.Now,
                            fk_cartao = parc.fk_cartao,
                            fk_empresa = parc.fk_empresa,
                            fk_loja = parc.fk_loja,
                            fk_parcela = (int)parc.i_unique,
                            nu_parcela = parc.nu_parcela,
                            st_afiliada = "",
                            st_ano = ano,
                            st_mes = mes,                        
                            vr_valor = parc.vr_valor
                        });
                }

                Console.WriteLine("## Fechamento realizado!");
                
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("(0.21) Patch?");
            Console.WriteLine("------------------------");

            AjustaParcela(873707);
            AjustaParcela(873708);

            AjustaParcela(870361);
            AjustaParcela(870362);

            AjustaParcela(868858);
            AjustaParcela(868859);

            AjustaParcela(867683);
            AjustaParcela(867684);

            AjustaParcela(870753);
            AjustaParcela(870754);

            AjustaParcela(877472);
            AjustaParcela(877473);

            AjustaParcela(869217);
            AjustaParcela(869218);

            AjustaParcela(874762);
            AjustaParcela(874763);

            AjustaParcela(869164);
            AjustaParcela(869165);

            AjustaParcela(870448);
            AjustaParcela(870449);

            AjustaParcela(874648);
            AjustaParcela(874649);
            AjustaParcela(871368);
            AjustaParcela(871369);

            AjustaParcela(870058);
            AjustaParcela(870059);

            AjustaParcela(870744);
            AjustaParcela(870745);

            AjustaParcela(878444);
            AjustaParcela(878445);

            AjustaParcela(876327);
            AjustaParcela(876328);
            
            AjustaParcela(879924);
            AjustaParcela(879925);            

            //            AjustaParcelasErradas("c:\\logs\\limpeza_fechamento.txt", 38, "04", "2019");

            //ReFecha("04", "2019", 23, 2019, 04, 16);
            //ReFecha("04", "2019", 24, 2019, 04, 16);
            //ReFecha("04", "2019", 25, 2019, 04, 16);
            //ReFecha("04", "2019", 26, 2019, 04, 16);

            Console.ReadLine();

            using (var db = new AutorizadorCNDB())
            {
                // ------------------------------
                // desfaz fechamento
                // ------------------------------

                /*

                var lstDelFech = new List<long>();

                var lstTrans = new List<string>();
                var hshLogTrans = new Hashtable();

                foreach (var itemFech in db.LOG_Fechamento.Where(y => y.fk_empresa == 38 && y.st_mes == "04" && y.st_ano == "2019").ToList())
                {
                    lstDelFech.Add((long)itemFech.i_unique);

                    var parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela);
                    var logTrans = parc.fk_log_transacoes.ToString();
                    var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                    var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                    Console.WriteLine("C" + cart.st_matricula + " => LT " + logTrans + " VR => " + itemFech.vr_valor + " parcs " + lstParcs.Count);

                    foreach (var itemParc in lstParcs)
                    {
                        if (itemParc.i_unique >= itemFech.fk_parcela)
                        {
                            var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                            parcUpd.nu_parcela++;
                            db.Update(parcUpd);
                        }
                    }                    
                }

                foreach (var item in lstDelFech)
                {
                    var itemF = db.LOG_Fechamento.FirstOrDefault(y => y.i_unique == item);
                    db.Delete(itemF);
                }

                // reconstroi o fechamento
                Console.WriteLine("--------- Fechamento vai ser construido");
                

                foreach (var parc in db.T_Parcelas.Where(y => y.fk_empresa == 38 && y.nu_parcela > 0).ToList())
                {
                    var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                    if (logTrans.dt_transacao > new DateTime(2019, 04, 15))
                        continue;

                    if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                        continue;
                    
                    var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                    parcUpd.nu_parcela--;
                    db.Update(parcUpd);

                    db.Insert(new LOG_Fechamento
                    {
                        dt_compra = logTrans.dt_transacao,
                        dt_fechamento = DateTime.Now,
                        fk_cartao = parc.fk_cartao,
                        fk_empresa = parc.fk_empresa,
                        fk_loja = parc.fk_loja,
                        fk_parcela = (int) parc.i_unique,
                        nu_parcela = parc.nu_parcela,
                        st_afiliada = "",
                        st_ano = "2019",
                        st_mes = "04",
                        vr_valor = parc.vr_valor                        
                    });
                }

                Console.WriteLine("## Fechamento realizado pra 9086");
                Console.ReadLine();
                */

                /*
                {
                    int tot = 0;

                    var lstDelFech = new List<long>();

                    {
                        var lstTrans = new List<string>();
                        var hshLogTrans = new Hashtable();

                        Console.WriteLine("Empresa 008997");

                        foreach (var itemFech in db.LOG_Fechamento.
                                                    Where(y => y.fk_empresa == 28 &&
                                                              y.st_mes == "02" && y.st_ano == "2019").
                                                    OrderBy(y => y.fk_cartao).ThenBy(y => y.vr_valor).
                                                    ToList())
                        {
                            var logTrans = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela).fk_log_transacoes.ToString();
                            var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                            if (hshLogTrans[logTrans] == null)
                                hshLogTrans[logTrans] = 1;
                            else
                            {
                                hshLogTrans[logTrans] = (int)hshLogTrans[logTrans] + 1;
                                lstTrans.Add(logTrans);
                                tot++;

                                lstDelFech.Add((long)itemFech.i_unique);

                                Console.WriteLine("C" + cart.st_matricula + " => LT " + logTrans + " VR => " + itemFech.vr_valor);
                                Console.WriteLine("  >> parc " + itemFech.fk_parcela);

                                var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                                foreach (var itemParc in lstParcs)
                                {
                                    if (itemParc.i_unique >= itemFech.fk_parcela)
                                    {
                                        var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                                        parcUpd.nu_parcela++;
                                        db.Update(parcUpd);
                                    }
                                }
                            }
                        }                       
                    }

                    {
                        var lstTrans = new List<string>();
                        var hshLogTrans = new Hashtable();

                        Console.WriteLine("Empresa 008998");

                        foreach (var itemFech in db.LOG_Fechamento.
                                                    Where(y => y.fk_empresa == 29 &&
                                                              y.st_mes == "02" && y.st_ano == "2019").
                                                    OrderBy(y => y.fk_cartao).ThenBy(y => y.vr_valor).
                                                    ToList())
                        {
                            var logTrans = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela).fk_log_transacoes.ToString();
                            var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                            if (hshLogTrans[logTrans] == null)
                                hshLogTrans[logTrans] = 1;
                            else
                            {
                                hshLogTrans[logTrans] = (int)hshLogTrans[logTrans] + 1;
                                lstTrans.Add(logTrans);
                                tot++;

                                lstDelFech.Add((long)itemFech.i_unique);

                                Console.WriteLine("C" + cart.st_matricula + " => LT " + logTrans + " VR => " + itemFech.vr_valor);
                                Console.WriteLine("  >> parc " + itemFech.fk_parcela);

                                var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                                foreach (var itemParc in lstParcs)
                                {
                                    if (itemParc.i_unique >= itemFech.fk_parcela)
                                    {
                                        var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                                        parcUpd.nu_parcela++;
                                        db.Update(parcUpd);
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("Fechamentos errados: " + tot);

                    foreach (var item in lstDelFech)
                    {
                        var itemF = db.LOG_Fechamento.FirstOrDefault(y => y.i_unique == item);
                        db.Delete(itemF);
                    }
                }
                */
                /*var strPatch = Console.ReadLine();

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

                    case "FIXFECH":
                        {

                            var lstTrans = new List<string>();
                            var hshLogTrans = new Hashtable();

                            foreach (var itemFech in db.LOG_Fechamento.
                                                        Where ( y=> y.fk_empresa == 28 && 
                                                                    y.st_mes == "01" && y.st_ano == "2019" ).
                                                        OrderBy( y=> y.fk_cartao).ThenBy ( y=> y.vr_valor).
                                                        ToList())
                            {
                                var logTrans = db.T_Parcelas.FirstOrDefault(y=> y.i_unique == itemFech.fk_parcela).fk_log_transacoes.ToString();
                                var cart = db.T_Cartao.FirstOrDefault(y=> y.i_unique == itemFech.fk_cartao);

                                if (hshLogTrans[logTrans] == null)
                                    hshLogTrans[logTrans] = 1;
                                else
                                {
                                    hshLogTrans[logTrans] = (int)hshLogTrans[logTrans] + 1;
                                    lstTrans.Add(logTrans);

                                    Console.WriteLine("C" + cart.st_matricula + " => LT " + logTrans + " VR => " + itemFech.vr_valor);
                                }
                            }

                            break;
                        }
                        
                }
                */
            }
        }
    }
}
