﻿using System.Linq;
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
        public string   nome,
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
                        cartao,
                        matricula,
                        vlrParcela;
    }

    public class EmissoraRelExtratoFornController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");
            var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa");
            var codigo = Request.GetQueryStringValue("codigo");
            var mes = Request.GetQueryStringValue("mes", 0);
            var ano = Request.GetQueryStringValue("ano", 0);

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var lojistaEsp = (from e in db.T_Loja
                              where e.st_loja == codigo
                              select e).
                              FirstOrDefault();

            var tEmp = db.currentEmpresa;

            if (idEmpresa != null)
            {
                tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa);
            }

            if (tEmp == null)
                return BadRequest();

            var t_txAdmin = (from e in db.LINK_LojaEmpresa
                             where e.fk_empresa == tEmp.i_unique 
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
                    {
                        var lstParcelasEmAberto = (from e in db.T_Parcelas
                                                   join ltr in db.LOG_Transacoes on e.fk_log_transacoes equals (int)ltr.i_unique
                                                   where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                                   where e.fk_empresa == tEmp.i_unique
                                                   where e.fk_loja == lojistaEsp.i_unique
                                                   where e.nu_parcela >= 1
                                                   select e);

                        if (!lstParcelasEmAberto.Any())
                            return BadRequest();

                        var diaFech = (from e in db.I_Scheduler
                                       where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + tEmp.st_empresa)
                                       select e).
                                       FirstOrDefault().
                                       nu_monthly_day;

                        var dtNow = DateTime.Now;

                        if (dtNow.Day > diaFech)
                            dtNow = dtNow.AddMonths(1);

                        int nuParc = 1;

                        var target = ano + mes.ToString().PadLeft(2, '0');                        
                        while (target != dtNow.Year + dtNow.Month.ToString("00"))
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

                        var ids = lstParcelasEmAberto.Select(y => y.fk_log_transacoes).ToList();

                        var ltrs = (from e in db.LOG_Transacoes
                                    where ids.Contains ((int)e.i_unique)
                                    select e).
                                    ToList();

                        ids = ltrs.Select(y => y.fk_cartao).ToList();

                        var carts = (from e in db.T_Cartao
                                     where ids.Contains((int)e.i_unique)
                                     select e).
                                    ToList();

                        ids = carts.Select(y => y.fk_dadosProprietario).ToList();

                        var props = (from e in db.T_Proprietario
                                     where ids.Contains((int)e.i_unique)
                                     select e).
                                    ToList();

                        long serial = 1;

                        foreach (var term in termsAbertos)
                        {
                            var terminal = new EmissoraRelExtratosFornDTO();

                            lst.Add(terminal);

                            var t = terms.Where(y => y.i_unique == term).FirstOrDefault();

                            terminal.nome = t.nu_terminal;
                            terminal.empresa = tEmp.st_fantasia;

                            terminal.itens = new List<ItensForn>();

                            long vrTotal = 0, vrRepasse = 0;

                            foreach (var parc in from e in lstParcelasEmAberto
                                                 where e.nu_parcela == nuParc
                                                 where e.fk_terminal == term
                                                 select e)
                            {
                                var lt = ltrs.Where(y => y.i_unique == parc.fk_log_transacoes).FirstOrDefault();

                                var cart = carts.FirstOrDefault(y => y.i_unique == lt.fk_cartao);
                                var prop = props.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario);

                                terminal.itens.Add(new ItensForn
                                {
                                    serial = serial.ToString(),
                                    dtVenda = ObtemData(parc.dt_inclusao),
                                    nsu = parc.nu_nsu.ToString(),
                                    cartao = prop.st_nome,
                                    matricula = cart.st_matricula,
                                    parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                                    valor = mon.setMoneyFormat((long)lt.vr_total),
                                    vlrParcela = mon.setMoneyFormat((long)parc.vr_valor)
                                });

                                vrTotal += (long)parc.vr_valor;

                                var t_vrBonus = parc.vr_valor * t_txAdmin / 10000;

                                vrBonusMax += (long)t_vrBonus;

                                vrRepasse += (long)parc.vr_valor - (long)t_vrBonus;

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
                            empresa = tEmp.st_fantasia + " (" + tEmp.st_empresa + ")",
                            lojista = lojistaEsp.st_nome + " - " + lojistaEsp.st_social + " - CNPJ: " + lojistaEsp.nu_CNPJ,
                            total = mon.setMoneyFormat(vrTotalMax),
                            totalBonus = mon.setMoneyFormat(vrBonusMax),
                            totalRep = mon.setMoneyFormat(vrRepasseMax),
                            txAdmin,
                            results = lst,
                            dtEmissao = ObtemData(DateTime.Now)
                        });
                    }

                // ------------------------
                // encerrado
                // ------------------------

                case "2":
                    {
                        string sMes = mes.ToString().PadLeft(2,'0');
                        string sAno = ano.ToString();

                        var lstFechamento = (from e in db.LOG_Fechamento
                                             join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                             join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int)ltr.i_unique
                                             where ltr.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                             where e.fk_empresa == tEmp.i_unique
                                             where e.fk_loja == lojistaEsp.i_unique
                                             where e.st_mes == sMes && e.st_ano == sAno
                                             select e);

                        if (!lstFechamento.Any())
                            return BadRequest();

                        var termsAbertos = (from e in lstFechamento
                                            join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                            join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int)ltr.i_unique
                                            select ltr.fk_terminal).
                                            Distinct().
                                            ToList();

                        var terms = (from e in db.T_Terminal
                                     where termsAbertos.Contains((int)e.i_unique)
                                     select e).
                                     ToList();

                        var parcs = (from e in lstFechamento
                                    join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                    select parc).
                                    ToList();

                        var ltrs = (from e in lstFechamento
                                    join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                    join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int)ltr.i_unique
                                    select ltr).
                                    ToList();

                        long serial = 1;

                        foreach (var term in termsAbertos)
                        {
                            var terminal = new EmissoraRelExtratosFornDTO();

                            lst.Add(terminal);
                            
                            var t = terms.Where(y => y.i_unique == term).FirstOrDefault();

                            terminal.nome = t.nu_terminal;
                            terminal.empresa = tEmp.st_fantasia;

                            terminal.itens = new List<ItensForn>();

                            long vrTotal = 0, vrRepasse = 0;

                            foreach (var parc in from e in lstFechamento
                                                 join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                                 join ltr in db.LOG_Transacoes on parc.fk_log_transacoes equals (int)ltr.i_unique
                                                 where ltr.fk_terminal == term
                                                 select parc)
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

                                vrTotal += (long)parc.vr_valor;

                                var t_vrBonus = parc.vr_valor * t_txAdmin / 10000;

                                vrBonusMax += (long)t_vrBonus;

                                vrRepasse += (long)parc.vr_valor - (long)t_vrBonus;

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
                            empresa = tEmp.st_fantasia + " (" + tEmp.st_empresa + ")",
                            lojista = lojistaEsp.st_nome + " - " + lojistaEsp.st_social + " - CNPJ: " + lojistaEsp.nu_CNPJ,
                            total = mon.setMoneyFormat(vrTotalMax),
                            totalBonus = mon.setMoneyFormat(vrBonusMax),
                            totalRep = mon.setMoneyFormat(vrRepasseMax),
                            txAdmin,
                            results = lst,
                            dtEmissao = ObtemData(DateTime.Now)
                        });
                    }
            }

            return BadRequest();
        }
    }
}
