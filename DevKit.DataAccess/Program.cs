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
        static void Main(string[] args)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("(0.22) Patch?");
            Console.WriteLine("------------------------");

          //  LimpaScheduler();
        }

        static void LimpaScheduler()
        {
            using (var db = new AutorizadorCNDB())
            {
                var lstSchedul = (from e in db.I_Scheduler
                                   where e.st_job.StartsWith("schedule_fech_mensal;empresa;")
                                   select e).
                                   ToList();

                foreach (var item in lstSchedul)
                {
                    db.Delete(item);
                }
            }
        }

        static void CopiaDadosDoScheduler()
        {
            using (var db = new AutorizadorCNDB())
            {
                foreach (var item in db.T_Empresa.ToList())
                {
                    var t_scheduler = (from e in db.I_Scheduler
                                       where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + item.st_empresa)
                                       select e).
                                       FirstOrDefault();

                    if (t_scheduler == null)
                        continue;

                    var dbEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == item.i_unique);

                    dbEmp.st_horaFech = t_scheduler.st_monthly_hhmm;
                    dbEmp.nu_diaFech = t_scheduler.nu_monthly_day;

                    db.Update(dbEmp);
                }
            }
        }

        static void AjustaParcela(int idParcela)
        {
            #region - code - 
            Console.WriteLine(idParcela);

            using (var db = new AutorizadorCNDB())
            {
                var fech = db.LOG_Fechamento.FirstOrDefault(y => y.fk_parcela == idParcela);

                var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == idParcela);
                parcUpd.nu_parcela++;
                db.Update(parcUpd);

                db.Delete(fech);
            }
            #endregion
        }

        static void AjustaParcelasErradas(string arquivo, int fkEmpresa, string mes, string ano)
        {
            #region - code - 

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

                            var cartao_mat = dados[3].Split('.')[0].PadLeft(6, '0');
                            var cartao_tit = dados[3].Split('.')[1];

                            var valor = Convert.ToInt64(dados[5].Replace(",", "").Replace(".", ""));
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

            #endregion
        }

        static void ReFecha(string mes, string ano, int fkEmpresa, int anoF, int mesF, int diaF)
        {
            #region - code -
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
            #endregion
        }
    }
}
