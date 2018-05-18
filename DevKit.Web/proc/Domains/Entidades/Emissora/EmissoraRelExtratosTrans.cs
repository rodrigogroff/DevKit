using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;
using DataModel;

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
                        mat,
                        assoc,
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
            var dtInicial = Request.GetQueryStringValue("dtInicial");
            var dtFinal = Request.GetQueryStringValue("dtFinal");
            var sit = Request.GetQueryStringValue("sit");

            DateTime? dt_inicial = ObtemData(dtInicial),
                      dt_final = ObtemData(dtFinal);

            var dt = DateTime.Now;

            if (dt_inicial == null)
                dt_inicial = new DateTime(dt.Year, dt.Month, dt.Day);

            if (dt_final == null)
                dt_final = new DateTime(dt.Year, dt.Month, dt.Day).AddDays(1);
            else
                dt_final = Convert.ToDateTime(dt_final).AddDays(1);
            
            if (dt_final != null && dt_inicial != null)
                if (dt_final < dt_inicial)
                    return BadRequest("Datas inválidas");

            T_Cartao cart = null;
            T_Proprietario prop = null;

            var lstCarts = new List<int>();

            if (!string.IsNullOrEmpty(mat))
            {
                cart = (from e in db.T_Cartao
                            where e.st_empresa == db.currentEmpresa.st_empresa
                            where e.st_matricula == mat.PadLeft(6, '0')
                            where e.st_titularidade == "01"
                            select e).
                        FirstOrDefault();

                if (cart == null)
                    return BadRequest("Cartão inválido");

                lstCarts = (from e in db.T_Cartao
                            where e.st_empresa == db.currentEmpresa.st_empresa
                            where e.st_matricula == mat.PadLeft(6, '0')
                            select (int)e.i_unique).
                            ToList();

                prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.i_unique
                        select e).
                        FirstOrDefault();
            }

            var lstTotCarts = (from e in db.T_Cartao
                               where e.st_empresa == db.currentEmpresa.st_empresa
                               select e).
                               ToList();

            var trans = (from e in db.LOG_Transacoes
                         where e.fk_empresa == db.currentEmpresa.i_unique
                         where lstCarts.Contains((int)e.fk_cartao) || lstCarts.Count() == 0
                         where e.dt_transacao >= dt_inicial && e.dt_transacao <= dt_final
                         orderby e.dt_transacao descending
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

            var mon = new money();

            var lst = new List<EmissoraRelExtratosTrans>
            {
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans(),
                new EmissoraRelExtratosTrans(),                
            };

            long serial = 0, tot = 0;

            // confirmadas
            if (sit == "1" || sit == "2")
            {
                var lstIT = lst[0];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    tot += (long) tran.vr_total;

                    string assocNome = "", _mat = "";
                    var _cart = lstTotCarts.FirstOrDefault(y => y.i_unique == tran.fk_cartao);

                    if (_cart != null)
                    {
                        _mat = _cart.st_matricula;

                        if (_cart.st_titularidade != "01")
                            assocNome = db.T_Dependente.
                                            FirstOrDefault(y => y.nu_titularidade == Convert.ToInt32(_cart.st_titularidade) &&
                                                           y.fk_proprietario == _cart.i_unique).
                                                             st_nome;
                        else
                            assocNome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == _cart.fk_dadosProprietario).st_nome;
                    }

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        mat = _mat,
                        assoc = assocNome,
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total)
                    });
                }
            }

            // canceladas
            if (sit == "1" || sit == "3")
            {
                var lstIT = lst[1];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    string assocNome = "", _mat = "";
                    var _cart = lstTotCarts.FirstOrDefault(y => y.i_unique == tran.fk_cartao);

                    if (_cart != null)
                    {
                        _mat = _cart.st_matricula;

                        if (_cart.st_titularidade != "01")
                            assocNome = db.T_Dependente.
                                            FirstOrDefault(y => y.nu_titularidade == Convert.ToInt32(_cart.st_titularidade) &&
                                                           y.fk_proprietario == _cart.i_unique).
                                                             st_nome;
                        else
                            assocNome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == _cart.fk_dadosProprietario).st_nome;
                    }

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        mat = _mat,
                        assoc = assocNome,
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total)
                    });
                }
            }

            // negadas
            if (sit == "1" || sit == "4")
            {
                var lstIT = lst[2];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Erro ||
                                                      y.tg_confirmada.ToString() == TipoConfirmacao.Negada ))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    string assocNome = "", _mat = "";
                    var _cart = lstTotCarts.FirstOrDefault(y => y.i_unique == tran.fk_cartao);

                    if (_cart != null)
                    {
                        _mat = _cart.st_matricula;

                        if (_cart.st_titularidade != "01")
                            assocNome = db.T_Dependente.
                                            FirstOrDefault(y => y.nu_titularidade == Convert.ToInt32(_cart.st_titularidade) &&
                                                           y.fk_proprietario == _cart.i_unique).
                                                             st_nome;
                        else
                            assocNome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == _cart.fk_dadosProprietario).st_nome;
                    }

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja != null ? loja.st_nome : "[*Não disponível*]",
                        nsu = tran.nu_nsu.ToString(),
                        mat = _mat,
                        assoc = assocNome,
                        parcelas = tran.nu_parcelas != null ? tran.nu_parcelas.ToString() : "",
                        terminal = term != null ? term.nu_terminal.ToString() : "[*Não disponível*]",
                        valorTot = tran.vr_total != null ? mon.setMoneyFormat((long)tran.vr_total) : "(?)",
                        motivo = tran.nu_cod_erro + " " + tran.st_msg_transacao
                    });
                }
            }

            // pends
            if (sit == "1" || sit == "5")
            {
                var lstIT = lst[3];

                foreach (var tran in trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente))
                {
                    serial++;

                    var loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();
                    var term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    string assocNome = "", _mat = "";
                    var _cart = lstTotCarts.FirstOrDefault(y => y.i_unique == tran.fk_cartao);

                    if (_cart != null)
                    {
                        _mat = _cart.st_matricula;

                        if (_cart.st_titularidade != "01")
                            assocNome = db.T_Dependente.
                                            FirstOrDefault(y => y.nu_titularidade == Convert.ToInt32(_cart.st_titularidade) &&
                                                           y.fk_proprietario == _cart.i_unique).
                                                             st_nome;
                        else
                            assocNome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == _cart.fk_dadosProprietario).st_nome;
                    }

                    lstIT.itens.Add(new ItensTrans
                    {
                        serial = serial.ToString(),
                        dt = ObtemData(tran.dt_transacao),
                        loja = loja.st_nome,
                        nsu = tran.nu_nsu.ToString(),
                        mat = _mat,
                        assoc = assocNome,
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
                cartao = cart != null ? cart.st_matricula + " - " + prop.st_nome : "",
                periodo = ObtemData(dt_inicial).Substring(0, 10) + " a " + ObtemData(dt_final).Substring(0, 10),
                total = mon.setMoneyFormat(tot)
            });            
        }
    }
}
