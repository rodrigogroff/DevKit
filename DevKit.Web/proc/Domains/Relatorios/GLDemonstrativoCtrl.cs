using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class GLDemonstrativoController : ApiControllerBase
    {
        public class DemoTransacao
        {
            public string serial, data, nsu, cartao, vlr, parcela, vlrPar;
        }

        public class Demonstrativo
        {
            public string mesAno, 
                          anoMesSort,
                          convenio, 
                          totalVendas, 
                          terminal,
                          vlrRepasseMensal, 
                          situacao,
                          dtFechamento;

            public List<DemoTransacao> lst = new List<DemoTransacao>();
        }

        public class DemonstrativoTerminal
        {
            public string   terminal,
                            totGeral,
                            totGeralRepasse,
                            totAtual,
                            totFuturo,
                            totAtualRepasse,
                            totFuturoRepasse;

            public List<Demonstrativo> lstDemonstrativos = new List<Demonstrativo>();
        }

        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue<int>("tipoDemonstrativo", 0);
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);
            var idTerminal = Request.GetQueryStringValue<int?>("idTerminal", null);
            var separarPorTerminal = Request.GetQueryStringValue<bool?>("porTerminal", false);

            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var lstTerminais = (from e in db.T_Terminal
                                where e.fk_loja == db.currentLojista.i_unique
                                where idTerminal == null || e.i_unique == idTerminal
                                select e).
                                ToList();
                
            var mon = new money();

            switch (tipo)
            {
                case 1:
                    {
                        #region - atuais e futuros -

                        var listConvenios = (from e in db.LINK_LojaEmpresa
                                             where e.fk_loja == db.currentLojista.i_unique
                                             where idEmpresa == null || e.fk_empresa == idEmpresa
                                             select e).
                                             ToList();

                        // --------------------------------------------------
                        // todos os terminais, sem separação de terminal
                        // --------------------------------------------------

                        if (separarPorTerminal == false)
                        {
                            var results = new List<Demonstrativo>();

                            long totAtual = 0,
                                 totFuturo = 0,
                                 totAtualRepasse = 0,
                                 totFuturoRepasse = 0;

                            #region - agrupado / total geral -

                            foreach (var convenioAtual in listConvenios)
                            {
                                var dt = DateTime.Now;

                                var tEmpresa = (from e in db.T_Empresa
                                                where e.i_unique == convenioAtual.fk_empresa
                                                select e).
                                                FirstOrDefault();

                                var diaFech = tEmpresa.nu_diaFech;

                                if (dt.Day > diaFech)
                                    dt = dt.AddMonths(1);

                                var st_mes = dt.Month.ToString("00");
                                var st_ano = dt.Year.ToString();

                                // ----------------
                                // atuais
                                // ----------------

                                {
                                    var totalVendas = (from e in db.T_Parcelas
                                                       join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                       where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                       where e.fk_empresa == tEmpresa.i_unique
                                                       where e.fk_loja == db.currentLojista.i_unique
                                                       where idTerminal == null || e.fk_terminal == idTerminal
                                                       where e.nu_parcela == 1
                                                       select (long?)e.vr_valor).
                                                       Sum();

                                    if (totalVendas == null)
                                        totalVendas = 0;

                                    if (totalVendas == 0)
                                        continue;

                                    totAtual += (long)totalVendas;

                                    var repasse = totalVendas -
                                                  totalVendas * convenioAtual.tx_admin / 10000;

                                    totAtualRepasse += (long)repasse;

                                    results.Add(new Demonstrativo
                                    {
                                        convenio = tEmpresa.st_fantasia,
                                        mesAno = st_mes + "/" + st_ano,
                                        anoMesSort = st_ano + st_mes,
                                        totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                        vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                        situacao = "ATUAL",
                                    });
                                }

                                var dtFut = dt;
                                var nuParc = 1;

                                // ----------------
                                // futuros
                                // ----------------

                                while (true)
                                {
                                    dtFut = dtFut.AddMonths(1);

                                    st_mes = dtFut.Month.ToString("00");
                                    st_ano = dtFut.Year.ToString();

                                    nuParc++;

                                    var totalVendas = (from e in db.T_Parcelas
                                                       join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                       where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                       where e.fk_empresa == tEmpresa.i_unique
                                                       where e.fk_loja == db.currentLojista.i_unique
                                                       where e.nu_parcela == nuParc
                                                       select (long?)e.vr_valor).
                                                        Sum();

                                    if (totalVendas == null)
                                        break;

                                    totFuturo += (long)totalVendas;

                                    var repasse = totalVendas -
                                                  totalVendas * convenioAtual.tx_admin / 10000;

                                    totFuturoRepasse += (long)repasse;

                                    results.Add(new Demonstrativo
                                    {
                                        convenio = tEmpresa.st_fantasia,
                                        mesAno = st_mes + "/" + st_ano,
                                        anoMesSort = st_ano + st_mes,
                                        totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                        vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                        situacao = "FUTURO",
                                    });
                                }
                            }

                            results = results.OrderBy(y => y.anoMesSort).ToList();

                            return Ok(new
                            {
                                count = results.Count,
                                results,
                                totAtual = "R$ " + mon.setMoneyFormat(totAtual),
                                totFuturo = "R$ " + mon.setMoneyFormat(totFuturo),
                                totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse),
                                totFuturoRepasse = "R$ " + mon.setMoneyFormat(totFuturoRepasse),
                            });

                            #endregion
                        }

                        if (separarPorTerminal == true)
                        {
                            #region - padrão - 

                            var results = new List<DemonstrativoTerminal>();

                            long supertotAtual = 0,
                                 supertotFuturo = 0,
                                 supertotAtualRepasse = 0,
                                 supertotFuturoRepasse = 0;

                            foreach (var term in lstTerminais)
                            {
                                long totAtual = 0,
                                 totFuturo = 0,
                                 totAtualRepasse = 0,
                                 totFuturoRepasse = 0;

                                var resultItem = new DemonstrativoTerminal { terminal = term.nu_terminal + " " + term.st_localizacao };

                                foreach (var convenioAtual in listConvenios)
                                {
                                    var dt = DateTime.Now;

                                    var tEmpresa = (from e in db.T_Empresa
                                                    where e.i_unique == convenioAtual.fk_empresa
                                                    select e).
                                                    FirstOrDefault();

                                    var diaFech = tEmpresa.nu_diaFech;

                                    if (dt.Day > diaFech)
                                        dt = dt.AddMonths(1);

                                    // ----------------
                                    // atuais
                                    // ----------------

                                    {
                                        var totalVendas = (from e in db.T_Parcelas
                                                           join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                           where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                           where e.fk_empresa == tEmpresa.i_unique
                                                           where e.fk_loja == db.currentLojista.i_unique
                                                           where ltr.fk_terminal == term.i_unique
                                                           where e.nu_parcela == 1
                                                           select (long?)e.vr_valor).
                                                            Sum();

                                        if (totalVendas == null)
                                            totalVendas = 0;

                                        if (totalVendas == 0)
                                            continue;

                                        var st_mes = dt.Month.ToString("00");
                                        var st_ano = dt.Year.ToString();

                                        totAtual += (long)totalVendas;
                                        supertotAtual += totAtual;

                                        var repasse = totalVendas -
                                                        totalVendas * convenioAtual.tx_admin / 10000;

                                        totAtualRepasse += (long)repasse;
                                        supertotAtualRepasse += totAtualRepasse;

                                        var nItem = new Demonstrativo
                                        {
                                            convenio = tEmpresa.st_fantasia,
                                            mesAno = st_mes + "/" + st_ano,
                                            totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                            vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                            situacao = "ATUAL",
                                        };

                                        var tListParcelas = (from e in db.T_Parcelas
                                                             join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                             where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                             where e.fk_empresa == tEmpresa.i_unique
                                                             where e.fk_loja == db.currentLojista.i_unique
                                                             where ltr.fk_terminal == term.i_unique
                                                             where e.nu_parcela == 1
                                                             select e).
                                                     ToList();

                                        int serial = 1;

                                        var tempIdCarts = tListParcelas.Select(y => (int)y.fk_cartao).ToList();
                                        var tempIdTrans = tListParcelas.Select(y => (int)y.fk_log_transacoes).ToList();

                                        var lstCarts = (from e in db.T_Cartao where tempIdCarts.Contains((int)e.i_unique) select e).ToList();
                                        var lstTrans = (from e in db.LOG_Transacoes where tempIdTrans.Contains((int)e.i_unique) select e).ToList();

                                        foreach (var itemParcela in tListParcelas)
                                        {
                                            var cart = lstCarts.Where(y => y.i_unique == itemParcela.fk_cartao).FirstOrDefault();
                                            var tr = lstTrans.Where(y => y.i_unique == itemParcela.fk_log_transacoes).FirstOrDefault();

                                            nItem.lst.Add(new DemoTransacao
                                            {
                                                serial = serial.ToString(),
                                                data = Convert.ToDateTime(itemParcela.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                                                nsu = itemParcela.nu_nsu.ToString(),
                                                cartao = cart.st_empresa + "." + cart.st_matricula,
                                                vlr = "R$ " + mon.setMoneyFormat((long)tr.vr_total),
                                                parcela = itemParcela.nu_indice.ToString() + " / " + itemParcela.nu_tot_parcelas.ToString(),
                                                vlrPar = "R$ " + mon.setMoneyFormat((long)itemParcela.vr_valor),
                                            });

                                            serial++;
                                        }

                                        resultItem.lstDemonstrativos.Add(nItem);
                                    }

                                    var dtFut = dt;
                                    var nuParc = 1;

                                    // ----------------
                                    // futuros
                                    // ----------------

                                    while (true)
                                    {
                                        dtFut = dtFut.AddMonths(1);

                                        var st_mes = dtFut.Month.ToString("00");
                                        var st_ano = dtFut.Year.ToString();

                                        nuParc++;

                                        var totalVendas = (from e in db.T_Parcelas
                                                           join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                           where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                           where e.fk_empresa == tEmpresa.i_unique
                                                           where ltr.fk_terminal == term.i_unique
                                                           where e.fk_loja == db.currentLojista.i_unique
                                                           where e.nu_parcela == nuParc
                                                           select (long?)e.vr_valor).
                                                        Sum();

                                        if (totalVendas == null)
                                            break;
                                        
                                        totFuturo += (long)totalVendas;
                                        supertotFuturo += totFuturo;

                                        var repasse = totalVendas -
                                                        totalVendas * convenioAtual.tx_admin / 10000;

                                        totFuturoRepasse += (long)repasse;

                                        var nItem = new Demonstrativo
                                        {
                                            convenio = tEmpresa.st_fantasia,
                                            mesAno = st_mes + "/" + st_ano,
                                            totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                            vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                            situacao = "FUTURO",
                                        };

                                        var tListParcelas = (from e in db.T_Parcelas
                                                             join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                             where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                             where e.fk_empresa == tEmpresa.i_unique
                                                             where e.fk_loja == db.currentLojista.i_unique
                                                             where ltr.fk_terminal == term.i_unique
                                                             where e.nu_parcela == nuParc
                                                             select e).
                                                     ToList();

                                        int serial = 1;

                                        var tempIdCarts = tListParcelas.Select(y => (int)y.fk_cartao).ToList();
                                        var tempIdTrans = tListParcelas.Select(y => (int)y.fk_log_transacoes).ToList();

                                        var lstCarts = (from e in db.T_Cartao where tempIdCarts.Contains((int)e.i_unique) select e).ToList();
                                        var lstTrans = (from e in db.LOG_Transacoes where tempIdTrans.Contains((int)e.i_unique) select e).ToList();

                                        foreach (var itemParcela in tListParcelas)
                                        {
                                            var cart = lstCarts.Where(y => y.i_unique == itemParcela.fk_cartao).FirstOrDefault();
                                            var tr = lstTrans.Where(y => y.i_unique == itemParcela.fk_log_transacoes).FirstOrDefault();

                                            nItem.lst.Add(new DemoTransacao
                                            {
                                                serial = serial.ToString(),
                                                data = Convert.ToDateTime(itemParcela.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                                                nsu = itemParcela.nu_nsu.ToString(),
                                                cartao = cart.st_empresa + "." + cart.st_matricula,
                                                vlr = "R$ " + mon.setMoneyFormat((long)tr.vr_total),
                                                parcela = itemParcela.nu_indice.ToString() + " / " + itemParcela.nu_tot_parcelas.ToString(),
                                                vlrPar = "R$ " + mon.setMoneyFormat((long)itemParcela.vr_valor),
                                            });

                                            serial++;
                                        }

                                        resultItem.lstDemonstrativos.Add(nItem);
                                    }

                                    resultItem.totAtual = "R$ " + mon.setMoneyFormat(totAtual);
                                    resultItem.totFuturo = "R$ " + mon.setMoneyFormat(totFuturo);
                                    resultItem.totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse);
                                    resultItem.totFuturoRepasse = "R$ " + mon.setMoneyFormat(totFuturoRepasse);

                                    resultItem.totGeral = "R$ " + mon.setMoneyFormat(totAtual + totFuturo);
                                    resultItem.totGeralRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse + totFuturoRepasse);
                                }

                                if (totAtual > 0)
                                    results.Add(resultItem);
                            }

                            return Ok(new
                            {
                                count = results.Count,
                                results,
                                totAtual = "R$ " + mon.setMoneyFormat(supertotAtual),
                                totFuturo = "R$ " + mon.setMoneyFormat(supertotFuturo),
                                totAtualRepasse = "R$ " + mon.setMoneyFormat(supertotAtualRepasse),
                                totFuturoRepasse = "R$ " + mon.setMoneyFormat(supertotFuturoRepasse),
                            });

                            #endregion
                        }

                        #endregion

                        break;
                    } 

                case 2:
                    {
                        #region - fechados - 

                        var mes_inicial = Request.GetQueryStringValue<int?>("mes_inicial", null);
                        var ano = Request.GetQueryStringValue<int?>("ano", null);

                        var listConvenios = (from e in db.LINK_LojaEmpresa
                                             where e.fk_loja == db.currentLojista.i_unique
                                             where idEmpresa == null || e.fk_empresa == idEmpresa
                                             select e).
                                             ToList();

                        var st_mes = "00";

                        if (mes_inicial != null)
                            st_mes = ((int)mes_inicial).ToString("00");

                        var st_ano = ano.ToString();

                        // --------------------------------------------------
                        // todos os terminais, sem separação de terminal
                        // --------------------------------------------------

                        if (!(from e in db.LOG_Fechamento
                              where e.fk_loja == db.currentLojista.i_unique
                              where e.st_ano.ToString() == st_ano
                              where st_mes == "00" || e.st_mes == st_mes
                              select e).Any())
                        {
                            return Ok(new
                            {
                                count = 0,
                                results = new List<string>()
                            });
                        }

                        if (separarPorTerminal == false)
                        {
                            var results = new List<Demonstrativo>();

                            long totAtual = 0,                                 
                                 totAtualRepasse = 0;

                            #region - agrupado / total geral -

                            foreach (var convenioAtual in listConvenios)
                            {
                                var tEmpresa = (from e in db.T_Empresa
                                                where e.i_unique == convenioAtual.fk_empresa
                                                select e).
                                                FirstOrDefault();

                                var hora = tEmpresa.st_horaFech;

                                {
                                    var totalVendas = (from e in db.LOG_Fechamento
                                                       join ltr in db.T_Parcelas on e.fk_parcela equals (int)ltr.i_unique
                                                       where e.fk_empresa == tEmpresa.i_unique
                                                       where e.fk_loja == db.currentLojista.i_unique
                                                       where idTerminal == null || ltr.fk_terminal == idTerminal 
                                                       where e.st_ano.ToString() == st_ano
                                                       where st_mes == "00" || e.st_mes == st_mes
                                                       select (long?)e.vr_valor).
                                                       Sum();

                                    if (totalVendas == null)
                                        totalVendas = 0;

                                    if (totalVendas == 0)
                                        continue;

                                    totAtual += (long)totalVendas;

                                    var repasse = totalVendas -
                                                  totalVendas * convenioAtual.tx_admin / 10000;

                                    totAtualRepasse += (long)repasse;

                                    results.Add(new Demonstrativo
                                    {
                                        convenio = tEmpresa.st_fantasia,
                                        mesAno = st_mes == "00" ? st_ano : st_mes + "/" + st_ano,
                                        totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                        vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                        situacao = "ENCERRADO",
                                        dtFechamento = tEmpresa.nu_diaFech + " / " + hora.Substring(0,2) + ":" + hora.Substring(2)
                                    });
                                }
                            }

                            return Ok(new
                            {
                                count = results.Count,
                                results,
                                totAtual = "R$ " + mon.setMoneyFormat(totAtual),
                                totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse),
                            });

                            #endregion
                        }

                        if (separarPorTerminal == true)
                        {
                            #region - padrão - 

                            var results = new List<DemonstrativoTerminal>();

                            long supertotAtual = 0,                                 
                                 supertotAtualRepasse = 0;

                            var _lst_ids_terms = lstTerminais.Select(y => (int)y.i_unique).Distinct().ToList();
                            var _lst_ids_empresas = listConvenios.Select ( y=> (int) y.fk_empresa).Distinct().ToList();

                            var _lstFech = (from e in db.LOG_Fechamento
                                           join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                           where _lst_ids_terms.Contains((int)parc.fk_terminal)
                                           where _lst_ids_empresas.Contains((int)e.fk_empresa)
                                           where e.fk_loja == db.currentLojista.i_unique
                                           where st_mes == "00" || e.st_mes == st_mes
                                           where st_ano == e.st_ano
                                           select new LOG_Fechamento
                                           {
                                               fkTerminal = parc.fk_terminal,
                                               fk_cartao = e.fk_cartao,
                                               fk_empresa = e.fk_empresa,
                                               fk_loja = e.fk_loja,
                                               fk_parcela = e.fk_parcela,
                                               vr_valor = e.vr_valor,
                                               st_mes = e.st_mes,
                                               st_ano = e.st_ano
                                           }).
                                           ToList();

                            var _parc_ids = _lstFech.Select(y => (int)y.fk_parcela).Distinct().ToList();

                            var _lstParcelas = db.T_Parcelas.Where(y => _parc_ids.Contains((int)y.i_unique)).ToList();

                            foreach (var term in lstTerminais)
                            {
                                long totAtual = 0,
                                     totAtualRepasse = 0;
                                
                                var resultItem = new DemonstrativoTerminal { terminal = term.nu_terminal + " " + term.st_localizacao };

                                foreach (var convenioAtual in listConvenios)
                                {
                                    //if (convenioAtual.fk_empresa == 11) // convey testes
                                      //  continue;

                                    var tEmpresa = (from e in db.T_Empresa
                                                    where e.i_unique == convenioAtual.fk_empresa
                                                    select e).
                                                    FirstOrDefault();

                                    // ----------------
                                    // atuais
                                    // ----------------

                                    {
                                        var totalVendas = (from e in _lstFech
                                                           where e.fkTerminal == term.i_unique
                                                           where e.fk_empresa == tEmpresa.i_unique
                                                           where e.fk_loja == db.currentLojista.i_unique
                                                           where st_mes == "00" || e.st_mes == st_mes
                                                           where st_ano == e.st_ano
                                                           select (long?)e.vr_valor).
                                                           Sum();

                                        if (totalVendas == null)
                                            totalVendas = 0;

                                        if (totalVendas == 0)
                                            continue;

                                        totAtual += (long)totalVendas;
                                        supertotAtual += totAtual;

                                        var repasse = totalVendas -
                                                        totalVendas * convenioAtual.tx_admin / 10000;

                                        totAtualRepasse += (long)repasse;
                                        supertotAtualRepasse += totAtualRepasse;

                                        var nItem = new Demonstrativo
                                        {
                                            convenio = tEmpresa.st_fantasia,
                                            mesAno = st_mes + "/" + st_ano,
                                            totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                            vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                            situacao = "FECHADO",
                                        };

                                        var tListParcelas = (from e in _lstFech
                                                             join parc in _lstParcelas on e.fk_parcela equals (int)parc.i_unique
                                                             where parc.fk_terminal == term.i_unique
                                                             where e.fk_empresa == tEmpresa.i_unique
                                                             where e.fk_loja == db.currentLojista.i_unique
                                                             where st_mes == "00" || e.st_mes == st_mes
                                                             where st_ano == e.st_ano
                                                             select parc).
                                                             ToList();

                                        int serial = 1;

                                        var tempIdCarts = tListParcelas.Select(y => (int)y.fk_cartao).ToList();
                                        var tempIdTrans = tListParcelas.Select(y => (int)y.fk_log_transacoes).ToList();

                                        var lstCarts = (from e in db.T_Cartao where tempIdCarts.Contains((int)e.i_unique) select e).ToList();
                                        var lstTrans = (from e in db.LOG_Transacoes where tempIdTrans.Contains((int)e.i_unique) select e).ToList();

                                        foreach (var itemParcela in tListParcelas)
                                        {
                                            var cart = lstCarts.Where(y => y.i_unique == itemParcela.fk_cartao).FirstOrDefault();
                                            var tr = lstTrans.Where(y => y.i_unique == itemParcela.fk_log_transacoes).FirstOrDefault();

                                            nItem.lst.Add(new DemoTransacao
                                            {
                                                serial = serial.ToString(),
                                                data = Convert.ToDateTime(itemParcela.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                                                nsu = itemParcela.nu_nsu.ToString(),
                                                cartao = cart.st_empresa + "." + cart.st_matricula,
                                                vlr = "R$ " + mon.setMoneyFormat((long)tr.vr_total),
                                                parcela = itemParcela.nu_indice.ToString() + " / " + itemParcela.nu_tot_parcelas.ToString(),
                                                vlrPar = "R$ " + mon.setMoneyFormat((long)itemParcela.vr_valor),
                                            });

                                            serial++;
                                        }

                                        resultItem.lstDemonstrativos.Add(nItem);
                                    }

                                    resultItem.totAtual = "R$ " + mon.setMoneyFormat(totAtual);
                                    resultItem.totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse);

                                    resultItem.totGeral = "R$ " + mon.setMoneyFormat(totAtual);
                                    resultItem.totGeralRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse);
                                }

                                if (totAtual > 0)
                                    results.Add(resultItem);
                            }

                            return Ok(new
                            {
                                count = results.Count,
                                results = results,
                                totGeral = "R$ " + mon.setMoneyFormat(supertotAtual),
                                totGeralRepasse = "R$ " + mon.setMoneyFormat(supertotAtualRepasse),
                            });

                            #endregion
                        }

                        break;

                        #endregion
                    }
            }

            return BadRequest();            
        }
    }
}
