using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DevKit.Web.Controllers
{
    public class RelExtratoRepFinancMovResumido
    {
        public string mesAno,
                        tot,
                        bonus,
                        repasse,
                        totTrans;
    }

    public class RelExtratoRepFinancMovAberto
    {
        public string terminal,
                        lojista,
                        trans,
                        valor;

        public long _vlr;
    }

    public class RelExtratoRepFinancMovAbertoForn
    {
        public string   data,
                        nsu,
                        matricula,
                        associado,
                        vlrParcela,
                        vlrRepasse,
                        parcela;
    }

    public class EmissoraRelRepFinancController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");            

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            switch (tipo)
            {
                // ------------------------
                // movimento resumido
                // ------------------------

                case "1": 
                    {
                        var lst = new List<RelExtratoRepFinancMovResumido>();

                        var dtNow = DateTime.Now;

                        // lojas vinculadas à empresa

                        var hshLojaConvenio = new Hashtable();

                        foreach (var item in from e in db.T_Loja
                                             from eConv in db.LINK_LojaEmpresa
                                             where eConv.fk_empresa == db.currentEmpresa.i_unique
                                             where eConv.fk_loja == e.i_unique
                                             select eConv)
                        {
                            hshLojaConvenio[item.fk_loja] = item.tx_admin;
                        }

                        var diaFech = (from e in db.I_Scheduler
                                       where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                       select e).
                                       FirstOrDefault().
                                       nu_monthly_day;

                        if (dtNow.Day > diaFech)
                            dtNow = dtNow.AddMonths(1);

                        int nuParc = 1;

                        var totalParcsEmAberto = (from e in db.T_Parcelas
                                                  join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int) ltr.i_unique
                                                  where e.nu_parcela >= 1
                                                  where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                  where e.fk_empresa == db.currentEmpresa.i_unique
                                                  select e).
                                                  ToList();

                        long vrTotalMax = 0, vrBonusMax = 0, vrRepasseMax = 0;

                        while (true)
                        {
                            long vrTotal = 0, vrBonus = 0, vrRepasse = 0;

                            var query = (from e in totalParcsEmAberto
                                         where e.nu_parcela == nuParc  
                                         where e.fk_empresa == db.currentEmpresa.i_unique                                         
                                         select e);

                            if (!query.Any())
                                break;
                            
                            var lstVendasConfirmadas = query.ToList();

                            foreach (var item in (from e in lstVendasConfirmadas
                                                 select e.fk_loja).
                                                 Distinct())
                            {
                                var txConv = hshLojaConvenio[item] as int?;
                                if (txConv == null) continue;

                                var t_vrValor = lstVendasConfirmadas.
                                                Where(y => y.fk_loja == item).
                                                Sum(y => (long)y.vr_valor);

                                var t_vrBonus = t_vrValor * (long)txConv / 10000;
                                var t_repasse = t_vrValor - t_vrBonus;

                                vrTotal += t_vrValor;
                                vrBonus += t_vrBonus;
                                vrRepasse += t_repasse;
                            }

                            vrTotalMax += vrTotal;
                            vrBonusMax += vrBonus;
                            vrRepasseMax += vrRepasse;

                            lst.Add(new RelExtratoRepFinancMovResumido
                            {
                                mesAno = meses [dtNow.Month] + " / " + dtNow.Year,
                                tot = mon.setMoneyFormat(vrTotal),
                                bonus = mon.setMoneyFormat(vrBonus),
                                repasse = mon.setMoneyFormat(vrRepasse),
                                totTrans = lstVendasConfirmadas.Count().ToString()
                            });

                            nuParc++;
                            dtNow = dtNow.AddMonths(1);
                        }

                        return Ok(new
                        {
                            empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                            total = mon.setMoneyFormat(vrTotalMax),
                            totalBonus = mon.setMoneyFormat(vrBonusMax),
                            totalRep = mon.setMoneyFormat(vrRepasseMax),
                            results = lst,
                            dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                    }

                // ------------------------
                // movimento aberto
                // ------------------------

                case "2":
                    {
                        var mes = Request.GetQueryStringValue<int>("mes");
                        var ano = Request.GetQueryStringValue<int>("ano");
                        var lojista = Request.GetQueryStringValue("lojista");

                        if (string.IsNullOrEmpty(lojista))
                        {
                            // --------------------------------
                            // todos os lojistas
                            // --------------------------------
                            
                            var lst = new List<RelExtratoRepFinancMovAberto>();

                            var dtNow = DateTime.Now;

                            // lojas vinculadas à empresa

                            var hshLojaConvenio = new Hashtable();

                            foreach (var item in from e in db.T_Loja
                                                 from eConv in db.LINK_LojaEmpresa
                                                 where eConv.fk_empresa == db.currentEmpresa.i_unique
                                                 where eConv.fk_loja == e.i_unique
                                                 select eConv)
                            {
                                hshLojaConvenio[item.fk_loja] = item.tx_admin;
                            }

                            var diaFech = (from e in db.I_Scheduler
                                           where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                           select e).
                                           FirstOrDefault().
                                           nu_monthly_day;

                            if (dtNow.Day > diaFech)
                                dtNow = dtNow.AddMonths(1);

                            int nuParc = 1;

                            if (dtNow.Month != mes && dtNow.Year != ano)
                                while (dtNow.Month != mes && dtNow.Year != ano)
                                {
                                    nuParc++;
                                    dtNow = dtNow.AddMonths(1);
                                }

                            var totalParcsEmAberto = (from e in db.T_Parcelas
                                                      join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                      where e.nu_parcela == nuParc
                                                      where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                      where e.fk_empresa == db.currentEmpresa.i_unique
                                                      select e).
                                                      ToList();

                            var terminais = (from e in totalParcsEmAberto
                                             join t in db.T_Terminal on e.fk_terminal equals (int)t.i_unique
                                             select t).
                                             Distinct().
                                             ToList();

                            var lojas = (from e in totalParcsEmAberto
                                         join t in db.T_Loja on e.fk_loja equals (int)t.i_unique
                                         select t).
                                         Distinct().
                                         ToList();

                            long vrTotal = 0, vrBonus = 0, vrRepasse = 0;
                                                        
                            // soma todos os valores
                            
                            foreach (var item in (from e in totalParcsEmAberto
                                                  select e.fk_loja).
                                                    Distinct())
                            {
                                var txConv = hshLojaConvenio[item] as int?;
                                if (txConv == null) continue;

                                var t_vrValor = totalParcsEmAberto.
                                                Where(y => y.fk_loja == item).
                                                Sum(y => (long)y.vr_valor);

                                var t_vrBonus = t_vrValor * (long)txConv / 10000;
                                var t_repasse = t_vrValor - t_vrBonus;

                                vrTotal += t_vrValor;
                                vrBonus += t_vrBonus;
                                vrRepasse += t_repasse;
                            }

                            // totaliza fornecedores por terminal
                            
                            foreach (var loja in lojas)
                            {
                                foreach (var term in terminais.Where(y => y.fk_loja == loja.i_unique).ToList())
                                {
                                    var q = from e in totalParcsEmAberto
                                            where e.fk_terminal == term.i_unique
                                            where e.fk_loja == loja.i_unique
                                            select e;

                                    if (q.Any())
                                    {
                                        var vlr = q.Sum(y => (long)y.vr_valor);

                                        lst.Add(new RelExtratoRepFinancMovAberto
                                        {
                                            lojista = "(" + loja.st_loja.TrimStart('0') + ") " + loja.st_nome,
                                            terminal = term.nu_terminal,
                                            trans = q.Count().ToString(),
                                            valor = mon.setMoneyFormat(vlr),
                                            _vlr = vlr
                                        });
                                    }
                                }
                            }

                            var xlst = from e in lst
                                       orderby e._vlr descending
                                       select e;

                            return Ok(new
                            {
                                results = xlst,
                                empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                                total = mon.setMoneyFormat(vrTotal),
                                totalBonus = mon.setMoneyFormat(vrBonus),
                                totalRep = mon.setMoneyFormat(vrRepasse),
                                dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                            });
                        }
                        else
                        {
                            // --------------------------------
                            // fornecedor específico
                            // --------------------------------

                            var lojistaEspecifico = (from e in db.T_Loja
                                                     where e.st_loja == lojista
                                                     select e).
                                                     FirstOrDefault();

                            if (lojistaEspecifico == null)
                                return BadRequest();
                            
                            var lst = new List<RelExtratoRepFinancMovAbertoForn>();

                            var dtNow = DateTime.Now;

                            // lojas vinculadas à empresa

                            var convenio = (from e in db.T_Loja
                                            from eConv in db.LINK_LojaEmpresa
                                            where eConv.fk_empresa == db.currentEmpresa.i_unique
                                            where eConv.fk_loja == lojistaEspecifico.i_unique
                                            select eConv).
                                            FirstOrDefault();

                            var diaFech = (from e in db.I_Scheduler
                                           where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                           select e).
                                           FirstOrDefault().
                                           nu_monthly_day;

                            if (dtNow.Day > diaFech)
                                dtNow = dtNow.AddMonths(1);

                            int nuParc = 1;

                            if (dtNow.Month != mes && dtNow.Year != ano)
                                while (dtNow.Month != mes && dtNow.Year != ano)
                                {
                                    nuParc++;
                                    dtNow = dtNow.AddMonths(1);
                                }

                            var totalParcsEmAberto = (from e in db.T_Parcelas
                                                      join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                      where e.nu_parcela == nuParc
                                                      where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                      where e.fk_empresa == db.currentEmpresa.i_unique
                                                      where e.fk_loja == lojistaEspecifico.i_unique
                                                      select e).
                                                      ToList();

                            var terminais = (from e in totalParcsEmAberto
                                             join t in db.T_Terminal on e.fk_terminal equals (int)t.i_unique
                                             select t).
                                             Distinct().
                                             ToList();

                            var cartoes = (from e in totalParcsEmAberto
                                             join t in db.T_Cartao on e.fk_cartao equals (int)t.i_unique
                                             select t).
                                             Distinct().
                                             ToList();

                            var props = (from e in cartoes
                                         join t in db.T_Proprietario on e.fk_dadosProprietario equals (int)t.i_unique
                                         select t).
                                         Distinct().
                                         ToList();

                            long vrTotal = 0, vrBonus = 0, vrRepasse = 0;

                            // ------------------------------
                            // soma todos os valores
                            // ------------------------------

                            foreach (var item in (from e in totalParcsEmAberto
                                                  select e.fk_loja).
                                                    Distinct())
                            {
                                var t_vrValor = totalParcsEmAberto.
                                                Where(y => y.fk_loja == item).
                                                Sum(y => (long)y.vr_valor);

                                var t_vrBonus = t_vrValor * (long)convenio.tx_admin / 10000;
                                var t_repasse = t_vrValor - t_vrBonus;

                                vrTotal += t_vrValor;
                                vrBonus += t_vrBonus;
                                vrRepasse += t_repasse;
                            }

                            // -----------------------------------------
                            // totaliza fornecedores por terminal
                            // -----------------------------------------

                            foreach (var term in terminais.Where(y => y.fk_loja == lojistaEspecifico.i_unique).ToList())
                            {
                                foreach (var parc in from e in totalParcsEmAberto
                                                        where e.fk_terminal == term.i_unique
                                                        select e)
                                {
                                    var cart = cartoes.Where(y => y.i_unique == parc.fk_cartao).FirstOrDefault();
                                    var prop = props.Where(y => y.i_unique == cart.fk_dadosProprietario).FirstOrDefault();

                                    var t_vrBonus = parc.vr_valor * (long)convenio.tx_admin / 10000;
                                    var t_repasse = parc.vr_valor - t_vrBonus;

                                    lst.Add(new RelExtratoRepFinancMovAbertoForn
                                    {
                                        data = Convert.ToDateTime(parc.dt_inclusao).ToString("dd/MM/yyyy"),
                                        nsu = parc.nu_nsu.ToString(),
                                        associado = prop.st_nome,
                                        matricula = cart.st_matricula,
                                        parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                                        vlrParcela = mon.setMoneyFormat((long)parc.vr_valor),
                                        vlrRepasse = mon.setMoneyFormat((long)t_repasse)
                                    });
                                }                                       
                            }

                            return Ok(new
                            {
                                results = lst,
                                empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                                lojista = lojistaEspecifico.st_loja + " " + lojistaEspecifico.st_nome + " / " + lojistaEspecifico.st_social,
                                total = mon.setMoneyFormat(vrTotal),
                                totalBonus = mon.setMoneyFormat(vrBonus),
                                totalRep = mon.setMoneyFormat(vrRepasse),
                                dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                            });
                        }                        
                    }
            }

            return BadRequest();
        }
    }
}
