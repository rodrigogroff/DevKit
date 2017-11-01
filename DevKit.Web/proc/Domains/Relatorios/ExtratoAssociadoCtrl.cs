using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class ExtratoAssociado
    {
        public string dataHora = "", 
                      nsu = "", 
                      valor = "", 
                      parcela = "",
                      estab = "";
    }

    public class ExtratoAssociadoFuturo
    {
        public string total = "",
                      compras = "",
                      mes_ano = "",
                      pct_real = "",
                      disp = "";

        public List<ExtratoAssociado> vendas = new List<ExtratoAssociado>();
    }

    public class ExtratoAssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue<int>("tipo", 0);
            var mes = Request.GetQueryStringValue("extrato_fech_mes");
            var ano = Request.GetQueryStringValue("extrato_fech_ano_inicial");

            if (mes != null && mes != "")
                mes = mes.PadLeft(2, '0');

            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var mon = new money();
            var eMonth = new EnumMonth();
            var sd = new SaldoDisponivel();

            long dispM = 0, dispT = 0;

            sd.Obter(db, db.currentAssociado, ref dispM, ref dispT);

            switch (tipo)
            {
                case 1: // extrato fechado
                    #region - code - 
                    {
                        var lst = new List<ExtratoAssociado>();

                        long total = 0;

                        foreach (var item in (from e in db.LOG_Fechamento
                                                where e.fk_cartao == db.currentAssociado.i_unique
                                                where e.st_ano == ano
                                                where e.st_mes == mes
                                                orderby e.dt_compra descending
                                                select e).
                                                ToList())
                        {
                            var parcela = (from e in db.T_Parcelas
                                           where e.i_unique == item.fk_parcela
                                           select e).
                                           FirstOrDefault();

                            var loja = (from e in db.T_Loja
                                        where e.i_unique == item.fk_loja
                                        select e).
                                        FirstOrDefault();

                            total += (long) item.vr_valor;

                            lst.Add(new ExtratoAssociado
                            {
                                dataHora = Convert.ToDateTime(item.dt_compra).ToString("dd/MM/yyyy"),
                                nsu = parcela.nu_nsu.ToString(),
                                valor = mon.formatToMoney(parcela.vr_valor.ToString()),
                                parcela = parcela.nu_indice.ToString() + " / " + parcela.nu_tot_parcelas.ToString(),
                                estab = loja.st_nome
                            });
                        }
        
                        return Ok(new
                        {
                            count = 1,
                            mesAtual = eMonth.Get(Convert.ToInt32(mes)).stName,
                            total = "R$ " + mon.setMoneyFormat(total),
                            saldoDisp = "R$ " + mon.setMoneyFormat(dispM),
                            results = lst
                        });
                    }
                    #endregion

                case 2: // atual
                    #region - code -
                    {
                        var lst = new List<ExtratoAssociado>();

                        long total = 0;

                        foreach (var item in (from e in db.T_Parcelas
                                              join tr in db.LOG_Transacoes on e.fk_log_transacoes equals (int) tr.i_unique
                                              where e.fk_cartao == db.currentAssociado.i_unique
                                              where e.nu_parcela == 1
                                              where tr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                              orderby tr.dt_transacao descending
                                              select e).
                                              ToList())
                        {
                            var ltr = (from e in db.LOG_Transacoes
                                       where e.i_unique == item.fk_log_transacoes
                                       select e).
                                       FirstOrDefault();
                            
                            var loja = (from e in db.T_Loja
                                        where e.i_unique == item.fk_loja
                                        select e).
                                        FirstOrDefault();

                            total += (long)item.vr_valor;

                            lst.Add(new ExtratoAssociado
                            {
                                dataHora = Convert.ToDateTime(ltr.dt_transacao).ToString("dd/MM/yyyy"),
                                nsu = item.nu_nsu.ToString(),
                                valor = mon.formatToMoney(item.vr_valor.ToString()),
                                parcela = item.nu_indice.ToString() + " / " + item.nu_tot_parcelas.ToString(),
                                estab = loja.st_nome
                            });
                        }

                        var dt = DateTime.Now;

                        int _mes = DateTime.Now.Month;

                        if ( (int) db.currentAssociadoEmpresa.nu_diaVenc > DateTime.Now.Day )
                        {
                            dt = DateTime.Now.AddMonths(-1);
                            _mes = dt.Month;
                        }                            

                        return Ok(new
                        {
                            count = 1,
                            mesAtual = eMonth.Get(_mes).stName + " / " + dt.Year,
                            saldoDisp = "R$ " + mon.setMoneyFormat(dispM),
                            total = "R$ " + mon.setMoneyFormat(total),
                            results = lst
                        });
                    }
                    #endregion

                case 3: // futuro
                    #region - code - 
                    {
                        var dt = DateTime.Now.AddMonths(1);
                        var lstFinal = new List<ExtratoAssociadoFuturo>();

                        int t_parc = 2;

                        while (true)
                        {
                            var lst = (from e in db.T_Parcelas
                                       join tr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)tr.i_unique
                                       where e.fk_cartao == db.currentAssociado.i_unique
                                       where e.nu_parcela == t_parc
                                       where tr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                       orderby tr.dt_transacao descending
                                       select e).
                                        ToList();

                            if (lst.Count > 0)
                            {
                                var tot = lst.Sum(y => y.vr_valor);

                                var lstParc = new List<ExtratoAssociado>();

                                foreach (var item in lst)
                                {
                                    var ltr = (from e in db.LOG_Transacoes
                                               where e.i_unique == item.fk_log_transacoes
                                               select e).
                                       FirstOrDefault();

                                    var loja = (from e in db.T_Loja
                                                where e.i_unique == item.fk_loja
                                                select e).
                                                FirstOrDefault();

                                    lstParc.Add(new ExtratoAssociado
                                    {
                                        dataHora = Convert.ToDateTime(ltr.dt_transacao).ToString("dd/MM/yyyy"),
                                        nsu = item.nu_nsu.ToString(),
                                        valor = mon.formatToMoney(item.vr_valor.ToString()),
                                         parcela = item.nu_indice.ToString() + " / " + item.nu_tot_parcelas.ToString(),
                                        estab = loja.st_nome
                                    });
                                }

                                long vrPctReal = (long) tot * 10000/ (long)db.currentAssociado.vr_limiteMensal;

                                lstFinal.Add(new ExtratoAssociadoFuturo
                                {
                                    mes_ano = eMonth.Get(dt.Month).stName + " / " + dt.Year,
                                    compras = lst.Count.ToString(),
                                    total = mon.setMoneyFormat((long)tot),
                                    pct_real = mon.setMoneyFormat((long)vrPctReal),
                                    disp = mon.setMoneyFormat((long)db.currentAssociado.vr_limiteMensal - (long)tot),
                                    vendas = lstParc
                                });
                            }
                            else
                                break;

                            t_parc++;
                            dt = dt.AddMonths(1);
                        }

                        return Ok(new
                        {
                            count = 1,
                            results = lstFinal
                        });
                    }
                    #endregion
            }

            return BadRequest();            
        }
    }
}
