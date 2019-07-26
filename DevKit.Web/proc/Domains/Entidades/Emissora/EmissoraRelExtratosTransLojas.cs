using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class EmissoraRelExtratosTransLojas
    {
        public string   nome,
                        qtdTrans,
                        qtd_conf,
                        qtd_canc,
                        qtd_pend,
                        qtd_erro,
                        totValor;

        public List<ItensTransLojas> itens = new List<ItensTransLojas>();
    }

    public class ItensTransLojas
    {
        public string   nsu,
                        dt,
                        cartao,
                        associado,
                        valor,
                        parcelas,
                        sit,
                        terminal;
    }

    public class EmissoraRelExtratoTransLojasController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa", null);
            var codLoja = Request.GetQueryStringValue("codLoja");
            var dtInicial = Request.GetQueryStringValue("dtInicial");
            var dtFinal = Request.GetQueryStringValue("dtFinal");

            DateTime? dt_inicial = ObtemData(dtInicial),
                      dt_final = ObtemData(dtFinal);

            var dt = DateTime.Now;

            if (dt_inicial == null)
                dt_inicial = new DateTime(dt.Year, dt.Month, dt.Day);

            if (dt_final == null)
                dt_final = new DateTime(dt.Year, dt.Month, dt.Day).AddDays(1).AddSeconds(-1);
            else
                dt_final = Convert.ToDateTime(dt_final).AddDays(1).AddSeconds(-1);
            
            if (dt_final != null && dt_inicial != null)
                if (dt_final < dt_inicial)
                    return BadRequest("Datas inválidas");

            var idLoja = 0;

            if (!string.IsNullOrEmpty(codLoja))
            {
                var t_loja = (from e in db.T_Loja
                              where e.st_loja == codLoja
                              select e).
                              FirstOrDefault();

                if (t_loja != null)
                {
                    idLoja = (int) t_loja.i_unique;
                }
            }

            var tEmp = db.currentEmpresa;

            if (idEmpresa != null)
                tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa);

            var tIdEmpresa = 0;

            if (tEmp != null)
                tIdEmpresa = (int) tEmp.i_unique;

            var t_trans = (from e in db.LOG_Transacoes
                           where tIdEmpresa == 0 || e.fk_empresa == tIdEmpresa
                           where e.dt_transacao >= dt_inicial && e.dt_transacao <= dt_final
                           where idLoja == 0 || e.fk_loja == idLoja
                           orderby e.dt_transacao descending
                           select e);

            if (t_trans.Count() > 5000)
                return BadRequest("Excede 5 mil registros");

            var trans = t_trans.ToList();

            var lojas = (from e in trans
                         join loja in db.T_Loja on e.fk_loja equals (int)loja.i_unique
                         select loja).
                         Distinct().
                         ToList();

            var cartoes = (from e in trans
                         join cart in db.T_Cartao on e.fk_cartao equals (int)cart.i_unique
                         select cart).
                         Distinct().
                         ToList();

            var props = (from e in cartoes
                           join prop in db.T_Proprietario on e.fk_dadosProprietario equals (int)prop.i_unique
                           select prop).
                           Distinct().
                         ToList();

            var terminais = (from e in trans
                             join term in db.T_Terminal on e.fk_terminal equals (int)term.i_unique
                             select term).
                             Distinct().
                             ToList();

            var mon = new money();

            var lst = new List<EmissoraRelExtratosTransLojas>();

            foreach (var item in lojas.OrderBy (y=> y.st_nome))
            {
                var relatLoja = new EmissoraRelExtratosTransLojas
                {
                    itens = new List<ItensTransLojas>()
                };

                var lojaTrans = trans.Where(y => y.fk_loja == item.i_unique).
                                        Where(y =>  y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada || 
                                                    y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada ||
                                                    y.tg_confirmada.ToString() == TipoConfirmacao.Erro ||
                                                    y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).
                                        ToList();

                relatLoja.qtdTrans = lojaTrans.Count().ToString();
                relatLoja.nome = item.st_loja + " - " + item.st_nome;
                relatLoja.totValor = mon.setMoneyFormat(lojaTrans.Where (y=> y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).Sum(y => (long)y.vr_total));

                relatLoja.qtd_conf = lojaTrans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).Count().ToString();
                relatLoja.qtd_canc = lojaTrans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada).Count().ToString();
                relatLoja.qtd_pend = lojaTrans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).Count().ToString();
                relatLoja.qtd_erro = lojaTrans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Erro).Count().ToString();

                foreach (var tran in lojaTrans.Where (y =>  y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada ||
                                                            y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada ||
                                                            y.tg_confirmada.ToString() == TipoConfirmacao.Erro ||
                                                            y.tg_confirmada.ToString() == TipoConfirmacao.Pendente))
                {
                    var cartao = cartoes.FirstOrDefault(y => y.i_unique == tran.fk_cartao);
                    var term = terminais.FirstOrDefault(y => y.i_unique == tran.fk_terminal);

                    var IsDep = Convert.ToInt32(cartao.st_titularidade) > 1;

                    T_Proprietario prop = null;

                    if (!IsDep)
                    {
                        prop = props.FirstOrDefault(y => y.i_unique == cartao.fk_dadosProprietario);
                    }

                    string _sit = "Confirmada";

                    switch (tran.tg_confirmada.ToString())
                    {
                        case TipoConfirmacao.Cancelada: _sit = "Cancelada"; break;
                        case TipoConfirmacao.Pendente: _sit = "Pendente"; break;
                        case TipoConfirmacao.Erro: _sit = "Erro"; break;
                    }

                    string _cart = cartao.st_matricula + "." + cartao.st_titularidade;

                    if (tEmp == null)
                        _cart = cartao.st_empresa + "." + _cart;

                    relatLoja.itens.Add(new ItensTransLojas
                    {
                        associado = prop != null ? prop.st_nome : "",
                        cartao = _cart,
                        dt = Convert.ToDateTime(tran.dt_transacao).ToString("dd/MM/yyyy HH:mm:ss"),
                        nsu = tran.nu_nsu.ToString(),
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal,
                        sit = _sit,
                        valor = mon.setMoneyFormat((long)tran.vr_total)
                    });
                }

                lst.Add(relatLoja);
            }

            return Ok(new
            {
                results = lst,
                dtEmissao = ObtemData(DateTime.Now),
                empresa = tEmp?.st_empresa + " - " + tEmp?.st_fantasia,
                periodo = ObtemData(dt_inicial).Substring(0, 10) + " a " + ObtemData(dt_final).Substring(0, 10),
                vendasConf = mon.formatToMoney(trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).Sum ( y=> y.vr_total).ToString()),
                qtdConf = trans.Count(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada),
                qtdPend = trans.Count(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente),
                qtdErro = trans.Count(y => y.tg_confirmada.ToString() == TipoConfirmacao.Erro),
                qtdCanc = trans.Count(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada),
            });            
        }
    }
}
