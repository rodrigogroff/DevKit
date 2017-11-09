using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class GLDemonstrativoController : ApiControllerBase
    {
        public class Demonstrativo
        {
            public string mesAno, 
                          convenio, 
                          totalVendas, 
                          terminal,
                          vlrRepasseMensal, 
                          situacao;
        }

        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue<int>("tipoDemonstrativo", 0);
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);
            var idTerminal = Request.GetQueryStringValue<int?>("idTerminal", null);
            var porTerminal = Request.GetQueryStringValue<bool?>("porTerminal", false);

            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var lstTerminais = (from e in db.T_Terminal
                                where e.fk_loja == db.currentLojista.i_unique
                                select e).
                                ToList();
                
            var mon = new money();

            switch (tipo)
            {
                case 1: // atuais e futuros

                    var results = new List<Demonstrativo>();

                    var listConvenios = (from e in db.LINK_LojaEmpresa
                                         where e.fk_loja == db.currentLojista.i_unique
                                         where idEmpresa == null || e.fk_empresa == idEmpresa
                                         select e).
                                         ToList();
                    
                    var dt = DateTime.Now;

                    var st_mes = dt.Month.ToString("00");
                    var st_ano = dt.Year.ToString();

                    long    totAtual = 0, 
                            totFuturo = 0,
                            totAtualRepasse = 0,
                            totFuturoRepasse = 0;

                    if (idTerminal == null)
                    {
                        // agrupado

                        foreach (var convenioAtual in listConvenios)
                        {
                            if (convenioAtual.fk_empresa == 11) // convey testes
                                continue;

                            var tEmpresa = (from e in db.T_Empresa
                                            where e.i_unique == convenioAtual.fk_empresa
                                            select e).
                                            FirstOrDefault();

                            // ----------------
                            // atuais
                            // ----------------

                            {
                                var totalVendas = (from e in db.T_Parcelas
                                                   join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                   where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                   where e.fk_empresa == tEmpresa.i_unique
                                                   where e.fk_loja == db.currentLojista.i_unique
                                                   where e.nu_parcela == 1
                                                   select (long?)e.vr_valor).
                                                   Sum();

                                if (totalVendas == null)
                                    totalVendas = 0;

                                totAtual += (long)totalVendas;

                                var repasse = totalVendas -
                                              totalVendas * convenioAtual.tx_admin / 10000;

                                totAtualRepasse += (long)repasse;

                                results.Add(new Demonstrativo
                                {
                                    convenio = tEmpresa.st_fantasia,
                                    mesAno = st_mes + "/" + st_ano,
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
                                    totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                    vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                    situacao = "FUTURO",
                                });
                            }
                        }

                        return Ok(new
                        {
                            count = results.Count,
                            results = results,
                            totAtual = "R$ " + mon.setMoneyFormat(totAtual),
                            totFuturo = "R$ " + mon.setMoneyFormat(totFuturo),
                            totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse),
                            totFuturoRepasse = "R$ " + mon.setMoneyFormat(totFuturoRepasse),
                        });
                    }
                    else
                    {
                        // por terminal

                        if (porTerminal == true)
                        {
                            foreach (var convenioAtual in listConvenios)
                            {
                                if (convenioAtual.fk_empresa == 11) // convey testes
                                    continue;

                                var tEmpresa = (from e in db.T_Empresa
                                                where e.i_unique == convenioAtual.fk_empresa
                                                select e).
                                                FirstOrDefault();

                                // ----------------
                                // atuais
                                // ----------------

                                {
                                    var totalVendas = (from e in db.T_Parcelas
                                                       join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                       where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                       where e.fk_empresa == tEmpresa.i_unique
                                                       where e.fk_loja == db.currentLojista.i_unique
                                                       where idTerminal == null || ltr.fk_terminal == idTerminal
                                                       where e.nu_parcela == 1
                                                       select (long?)e.vr_valor).
                                                       Sum();

                                    if (totalVendas == null)
                                        totalVendas = 0;

                                    totAtual += (long)totalVendas;

                                    var repasse = totalVendas -
                                                  totalVendas * convenioAtual.tx_admin / 10000;

                                    totAtualRepasse += (long)repasse;

                                    results.Add(new Demonstrativo
                                    {
                                        convenio = tEmpresa.st_fantasia,
                                        mesAno = st_mes + "/" + st_ano,
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
                                                       where idTerminal == null || ltr.fk_terminal == idTerminal
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
                                        totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                        vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                        situacao = "FUTURO",
                                    });
                                }
                            }

                            return Ok(new
                            {
                                count = results.Count,
                                results = results,
                                totAtual = "R$ " + mon.setMoneyFormat(totAtual),
                                totFuturo = "R$ " + mon.setMoneyFormat(totFuturo),
                                totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse),
                                totFuturoRepasse = "R$ " + mon.setMoneyFormat(totFuturoRepasse),
                            });
                        }
                        else
                        {
                            foreach (var convenioAtual in listConvenios)
                            {
                                if (convenioAtual.fk_empresa == 11) // convey testes
                                    continue;

                                var tEmpresa = (from e in db.T_Empresa
                                                where e.i_unique == convenioAtual.fk_empresa
                                                select e).
                                                FirstOrDefault();

                                // ----------------
                                // atuais
                                // ----------------

                                {
                                    var totalVendas = (from e in db.T_Parcelas
                                                       join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                       where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                       where e.fk_empresa == tEmpresa.i_unique
                                                       where e.fk_loja == db.currentLojista.i_unique
                                                       where idTerminal == null || ltr.fk_terminal == idTerminal
                                                       where e.nu_parcela == 1
                                                       select (long?)e.vr_valor).
                                                       Sum();

                                    if (totalVendas == null)
                                        totalVendas = 0;

                                    totAtual += (long)totalVendas;

                                    var repasse = totalVendas -
                                                  totalVendas * convenioAtual.tx_admin / 10000;

                                    totAtualRepasse += (long)repasse;

                                    results.Add(new Demonstrativo
                                    {
                                        convenio = tEmpresa.st_fantasia,
                                        mesAno = st_mes + "/" + st_ano,
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
                                                       where idTerminal == null || ltr.fk_terminal == idTerminal
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
                                        totalVendas = "R$ " + mon.setMoneyFormat((long)totalVendas),
                                        vlrRepasseMensal = "R$ " + mon.setMoneyFormat((long)repasse),
                                        situacao = "FUTURO",
                                    });
                                }
                            }

                            return Ok(new
                            {
                                count = results.Count,
                                results = results,
                                totAtual = "R$ " + mon.setMoneyFormat(totAtual),
                                totFuturo = "R$ " + mon.setMoneyFormat(totFuturo),
                                totAtualRepasse = "R$ " + mon.setMoneyFormat(totAtualRepasse),
                                totFuturoRepasse = "R$ " + mon.setMoneyFormat(totFuturoRepasse),
                            });
                        }                        
                    }

                    break;                    
            }

            return BadRequest();            
        }
    }
}
