using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class EmissoraRelExtratosTrans
    {
        public List<ItensTrans> itens = new List<ItensTrans>();
    }

    public class ItensTrans
    {
        public string serial,
                        loja,
                        terminal,
                        nsu,
                        valorTot,
                        parcelas,
                        dt,
                        motivo;
    }

    public class EmissoraRelExtratoTransController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mat = Request.GetQueryStringValue("mat");
            var dia = Request.GetQueryStringValue("dia", 0);
            var mes = Request.GetQueryStringValue("mes", 0);
            var ano = Request.GetQueryStringValue("ano", 0);
            var diaf = Request.GetQueryStringValue("diaf", 0);
            var mesf = Request.GetQueryStringValue("mesf", 0);
            var anof = Request.GetQueryStringValue("anof", 0);

            var cart = (from e in db.T_Cartao
                        where e.st_empresa == db.currentEmpresa.st_empresa
                        where e.st_matricula == mat.PadLeft(6, '0')
                        where e.st_titularidade == "01"
                        select e).
                        FirstOrDefault();

            if (cart == null)
                return BadRequest();

            var lstCarts = (from e in db.T_Cartao
                            where e.st_empresa == db.currentEmpresa.st_empresa
                            where e.st_matricula == mat.PadLeft(6, '0')
                            select (int)e.i_unique).
                            ToList();

            var prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();

            DateTime dt_inicial = new DateTime(ano, mes, dia);
            DateTime dt_final = dt_inicial.AddDays(1).AddSeconds(-1);

            if (mesf > 0 && anof > 0 && diaf > 0)
                dt_final = new DateTime(anof, mesf, diaf).AddDays(1).AddSeconds(-1);

            if (dt_final < dt_inicial)
                return BadRequest();

            var trans = (from e in db.LOG_Transacoes
                         where lstCarts.Contains((int)e.fk_cartao)
                         where e.dt_transacao >= dt_inicial && e.dt_transacao <= dt_final
                         orderby e.tg_confirmada, e.dt_transacao
                         select e).
                        ToList();

            var lojas = (from e in trans
                         join loja in db.T_Loja on e.fk_loja equals (int)loja.i_unique
                         select loja).
                         ToList();

            var terminais = (from e in trans
                             join term in db.T_Terminal on e.fk_terminal equals (int)term.i_unique
                             select term).
                             ToList();

            var parcelasConf = (from e in trans
                                join pa in db.T_Parcelas on (int)e.i_unique equals pa.fk_log_transacoes
                                where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                select pa).
                                ToList();
            
            var mon = new money();

            var lst = new List<EmissoraRelExtratosTrans>
            {
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans()
            };

            long serial = 0, tot = 0;

            // confirmadas
            {
                var lstIT = lst[0];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    tot += (long) tran.vr_total;

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total)
                    });

                    if (tran.nu_parcelas > 0)
                    {
                        foreach (var par in from e in parcelasConf
                                            where e.fk_log_transacoes == tran.i_unique
                                            orderby e.nu_indice
                                            select e)

                        {

                            serial++;

                            lstIT.itens.Add(new ItensTrans
                            {
                                serial = serial.ToString(),
                                dt = "",
                                loja = "",
                                nsu = "",
                                parcelas = par.nu_indice + " / " + par.nu_tot_parcelas,
                                terminal = "",
                                valorTot = mon.setMoneyFormat((long)par.vr_valor)
                            });
                        }
                    }
                }
            }

            // canceladas
            {
                var lstIT = lst[1];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total)
                    });
                }
            }

            // negadas
            {
                var lstIT = lst[2];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Erro ||
                                                      y.tg_confirmada.ToString() == TipoConfirmacao.Negada ))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja != null ? loja.st_nome : "[*Não disponível*]",
                        nsu = tran.nu_nsu.ToString(),
                        parcelas = tran.nu_parcelas != null ? tran.nu_parcelas.ToString() : "",
                        terminal = term != null ? term.nu_terminal.ToString() : "[*Não disponível*]",
                        valorTot = tran.vr_total != null ? mon.setMoneyFormat((long)tran.vr_total) : "(?)",
                        motivo = tran.nu_cod_erro + " " + tran.st_msg_transacao
                    });
                }
            }

            // pends
            {
                var lstIT = lst[3];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total)
                    });
                }
            }

            return Ok(new
            {
                results = lst,
                dtEmissao = ObtemData(DateTime.Now),
                cartao = cart.st_matricula + " - " + prop.st_nome,
                periodo = ObtemData(dt_inicial).Substring(0, 10) + " a " + ObtemData(dt_final).Substring(0, 10),
                total = mon.setMoneyFormat(tot)
            });            
        }
    }
}
