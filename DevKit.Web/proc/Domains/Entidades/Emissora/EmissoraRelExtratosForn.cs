using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DevKit.Web.Controllers
{
    public class EmissoraRelExtratosFornDTO
    {
        public string nome,
                        empresa,
                        total, 
                        totalRep;

        public List<ItensForn> itens = new List<ItensForn>();
    }

    public class ItensForn
    {
        public string serial, 
                        dtVenda, 
                        nsu, 
                        valor, 
                        parcela, 
                        vlrParcela;
    }

    public class EmissoraRelExtratoFornController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");
            var codigo = Request.GetQueryStringValue("codigo");
            var mes = Request.GetQueryStringValue("mes",0);
            var ano = Request.GetQueryStringValue("ano", 0);

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var lojistaEsp = (from e in db.T_Loja
                              where e.st_loja == codigo
                              select e).
                              FirstOrDefault();

            var t_txAdmin = (from e in db.LINK_LojaEmpresa
                             where e.fk_empresa == db.currentEmpresa.i_unique
                             where e.fk_loja == lojistaEsp.i_unique
                             select (long)e.tx_admin).FirstOrDefault();

            var txAdmin = mon.setMoneyFormat(t_txAdmin);

            long vrTotalMax = 0, 
                 vrBonusMax = 0, 
                 vrRepasseMax = 0;

            var lst = new List<EmissoraRelExtratosFornDTO>();

            switch (tipo)
            {
                // ------------------------
                // em aberto
                // ------------------------

                case "1":

                    var lstParcelasEmAberto = (from e in db.T_Parcelas
                                               join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                               where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                               where e.fk_empresa == db.currentEmpresa.i_unique
                                               where e.fk_loja == lojistaEsp.i_unique
                                               where e.nu_parcela >= 1
                                               select e);

                    if (!lstParcelasEmAberto.Any())
                        return BadRequest();

                    var diaFech = (from e in db.I_Scheduler
                                   where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                   select e).
                                   FirstOrDefault().
                                   nu_monthly_day;

                    var dtNow = DateTime.Now;

                    if (dtNow.Day > diaFech)
                        dtNow = dtNow.AddMonths(1);

                    int nuParc = 1;

                    if (dtNow.Month != mes && dtNow.Year != ano)
                        while (dtNow.Month != mes && dtNow.Year != ano)
                        {
                            nuParc++;
                            dtNow = dtNow.AddMonths(1);
                        }

                    var termsAbertos = (from e in lstParcelasEmAberto
                                        where e.nu_parcela == nuParc
                                        select e.fk_terminal).
                                        Distinct().
                                        ToList();

                    var terms = (from e in db.T_Terminal
                                 where termsAbertos.Contains((int)e.i_unique)
                                 select e).
                                 ToList();

                    var ltrs = (from e in lstParcelasEmAberto
                                join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int) ltr.i_unique
                                select ltr).
                                ToList();

                    long serial = 1;

                    foreach (var term in termsAbertos)
                    {
                        var terminal = new EmissoraRelExtratosFornDTO();

                        lst.Add(terminal);

                        var t = terms.Where(y => y.i_unique == term).FirstOrDefault();
                        
                        terminal.nome = t.nu_terminal;
                        terminal.empresa = db.currentEmpresa.st_fantasia;

                        terminal.itens = new List<ItensForn>();

                        long vrTotal = 0, vrRepasse = 0;

                        foreach (var parc in from e in lstParcelasEmAberto
                                              where e.nu_parcela == nuParc
                                              where e.fk_terminal == term
                                              select e)
                        {
                            var lt = ltrs.Where(y => y.i_unique == parc.fk_log_transacoes).FirstOrDefault();

                            terminal.itens.Add(new ItensForn
                            {
                                serial = serial.ToString(),                                
                                dtVenda = ObtemData(parc.dt_inclusao),
                                nsu = parc.nu_nsu.ToString(),
                                parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,                                
                                valor = mon.setMoneyFormat((long)lt.vr_total),
                                vlrParcela = mon.setMoneyFormat((long)parc.vr_valor)
                            });

                            vrTotal += (long)lt.vr_total;

                            var t_vrBonus = lt.vr_total * t_txAdmin / 10000;

                            vrBonusMax += (long) t_vrBonus;

                            vrRepasse += (long)lt.vr_total - (long)t_vrBonus;

                            serial++;
                        }

                        terminal.total = mon.setMoneyFormat(vrTotal);
                        terminal.totalRep = mon.setMoneyFormat(vrRepasse);

                        vrTotalMax += vrTotal;
                        vrRepasseMax += vrRepasse;
                    }                    

                    return Ok(new
                    {
                        referencia = meses[mes] + " / " + ano,
                        empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                        total = mon.setMoneyFormat(vrTotalMax),
                        totalBonus = mon.setMoneyFormat(vrBonusMax),
                        totalRep = mon.setMoneyFormat(vrRepasseMax),
                        txAdmin,
                        results = lst,
                        dtEmissao = ObtemData(DateTime.Now)
                    });

                // ------------------------
                // encerrado
                // ------------------------

                case "2":

                    return Ok(new
                    {
                        referencia = meses[mes] + " / " + ano,
                        empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                        total = mon.setMoneyFormat(vrTotalMax),
                        totalBonus = mon.setMoneyFormat(vrBonusMax),
                        totalRep = mon.setMoneyFormat(vrRepasseMax),
                        txAdmin,
                        results = lst,
                        dtEmissao = ObtemData(DateTime.Now)
                    });
            }

            return BadRequest();
        }
    }
}
