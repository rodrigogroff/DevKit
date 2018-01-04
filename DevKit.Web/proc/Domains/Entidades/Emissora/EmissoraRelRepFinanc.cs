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
                            count = lst.Count(),
                            empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                            total = mon.setMoneyFormat(vrTotalMax),
                            totalBonus = mon.setMoneyFormat(vrBonusMax),
                            totalRep = mon.setMoneyFormat(vrRepasseMax),
                            results = lst,
                            dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                    }
            }

            return BadRequest();
        }
    }
}
