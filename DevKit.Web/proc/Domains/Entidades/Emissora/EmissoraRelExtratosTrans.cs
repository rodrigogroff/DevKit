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
        public string   loja,
                        terminal,
                        nsu,
                        mat,
                        assoc,
                        valorTot,
                        parcelas,
                        dt,
                        tipo,
                        idstatus,
                        status,
                        motivo;
    }

    public class EmissoraRelExtratoTransController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            // -------------------
            // filtros normais
            // -------------------

            var mat = Request.GetQueryStringValue("mat");
            var dtInicial = Request.GetQueryStringValue("dtInicial");
            var dtFinal = Request.GetQueryStringValue("dtFinal");
            var sit = Request.GetQueryStringValue("sit");

            DateTime? dt_inicial = ObtemData(dtInicial),
                      dt_final = ObtemData(dtFinal);

            // -------------------
            // filtros dba
            // -------------------

            var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa", null);
            var nsu = Request.GetQueryStringValue("nsu");
            var terminal = Request.GetQueryStringValue("terminal");
            var codLoja = Request.GetQueryStringValue("codLoja");
            var cnpjLoja = Request.GetQueryStringValue("cnpjLoja");
            var valorVenda = ObtemValor(Request.GetQueryStringValue("valorVenda"));
            var parcelas = Request.GetQueryStringValue("parcelas");
            var operacao = Request.GetQueryStringValue("operacao");
            var tipo = Request.GetQueryStringValue("tipo");

            // -------------------
            // query
            // -------------------

            #region - setup - 

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

            T_Cartao cart = null;
            T_Proprietario prop = null;

            var lstCarts = new List<int>();

            var tEmp = db.currentEmpresa;

            if (idEmpresa != null)
            {
                tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa);

                if (tEmp == null)
                    return BadRequest();
            }

            if (!string.IsNullOrEmpty(mat))
            {
                cart = (from e in db.T_Cartao
                            where tEmp == null || tEmp != null && e.st_empresa == tEmp.st_empresa
                            where e.st_matricula == mat.PadLeft(6, '0')
                            where e.st_titularidade == "01"
                            select e).
                        FirstOrDefault();

                if (cart == null)
                    return BadRequest("Cartão inválido");

                lstCarts = (from e in db.T_Cartao
                            where tEmp == null || tEmp != null && e.st_empresa == tEmp.st_empresa
                            where e.st_matricula == mat.PadLeft(6, '0')
                            select (int)e.i_unique).
                            ToList();

                prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();
            }

            var lstTotCarts = tEmp != null ? (from e in db.T_Cartao where e.st_empresa == tEmp.st_empresa select e).ToList() :
                                             (from e in db.T_Cartao select e).ToList();

            #endregion

            var q_trans = from e in db.LOG_Transacoes
                          join emp in db.T_Empresa on e.fk_empresa equals (int)emp.i_unique
                          where emp.tg_bloq == 0
                          where e.dt_transacao >= dt_inicial && e.dt_transacao <= dt_final
                          where lstCarts.Count() == 0 || lstCarts.Contains ((int)e.fk_cartao)                          
                          select e;

            if (tEmp != null)
                q_trans = q_trans.Where(y => y.fk_empresa == tEmp.i_unique);

            // --------------------
            // aplicando filtros 
            // --------------------

            #region - code - 

            if (!string.IsNullOrEmpty (nsu))
                q_trans = q_trans.Where(y => y.nu_nsu.ToString() == nsu);
            
            if (!string.IsNullOrEmpty(terminal))
            {
                T_Terminal ter = db.T_Terminal.FirstOrDefault(y => y.nu_terminal.PadLeft(8, '0') == terminal.PadLeft(8, '0'));

                if (ter != null)
                    q_trans = q_trans.Where(y => y.fk_terminal == ter.i_unique);
            }

            if (!string.IsNullOrEmpty(codLoja))
            {
                T_Loja loj_temp = db.T_Loja.FirstOrDefault(y => y.st_loja == codLoja);

                if (loj_temp != null)
                    q_trans = q_trans.Where(y => y.fk_loja == loj_temp.i_unique);
            }

            if (!string.IsNullOrEmpty(cnpjLoja))
            {
                T_Loja loj_temp = db.T_Loja.FirstOrDefault(y => y.nu_CNPJ == cnpjLoja);

                if (loj_temp != null)
                    q_trans = q_trans.Where(y => y.fk_loja == loj_temp.i_unique);
            }

            if (valorVenda > 0)
                q_trans = q_trans.Where(y => y.vr_total == valorVenda);

            if (!string.IsNullOrEmpty(parcelas))
                q_trans = q_trans.Where(y => y.nu_parcelas.ToString() == parcelas);

            if (!string.IsNullOrEmpty(operacao))
                q_trans = q_trans.Where(y => y.st_msg_transacao.ToUpper().Contains (operacao.ToUpper()));

            if (!string.IsNullOrEmpty(tipo))
                if (tipo != "0")
                    q_trans = q_trans.Where(y => y.tg_contabil.ToString() == tipo);

            if (!string.IsNullOrEmpty(sit))
            {
                if (sit == "2")
                    q_trans = q_trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada);
                else if (sit == "3")
                    q_trans = q_trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada);
                else if (sit == "4") 
                    q_trans = q_trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Negada);
                else if (sit == "5")
                    q_trans = q_trans.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente);
            }

            #endregion

            q_trans = from e in q_trans orderby e.dt_transacao descending select e;

            if (q_trans.Count() > 2000)
                return BadRequest("Pesquisa excede máximo de 2000 transações");

            if (q_trans.Count() == 0)
                return Ok( new { fail = true } );

            var trans = q_trans.ToList();

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
            };

            long tot = 0;
                        
            {
                var lstIT = lst[0];

                foreach (var tran in trans.OrderBy ( y=> y.dt_transacao).ThenBy ( y=> y.nu_nsu))
                {
                    if (lstCarts.Count() > 0)
                        if (!lstCarts.Contains((int)tran.fk_cartao))
                            continue;

                    T_Loja loja = null;
                    T_Terminal term = null;

                    string  assocNome = "",
                            _mat = "", 
                            _stat = "";

                    switch (tran.tg_confirmada.ToString())
                    {
                        case "0": _stat = "Pendente"; break;
                        case "1": _stat = "Confirmada"; break;
                        case "2": _stat = "Negada"; break;
                        case "3": _stat = "Erro"; break;
                        case "4": _stat = "Registro"; break;
                        case "5": _stat = "Cancelada"; break;
                        case "6": _stat = "Desfeita"; break;
                    }

                    if (tran.fk_loja > 0)
                        loja = lojas.Where(y => y.i_unique == tran.fk_loja).FirstOrDefault();

                    if (tran.fk_terminal > 0)
                        term = terminais.Where(y => y.i_unique == tran.fk_terminal).FirstOrDefault();

                    tot += (long) tran.vr_total;

                    if (tran.fk_cartao > 0)
                    {
                        var _cart = lstTotCarts.FirstOrDefault(y => y.i_unique == tran.fk_cartao);

                        if (_cart != null)
                        {
                            if (tEmp == null)
                                _mat = _cart.st_empresa + "." + _cart.st_matricula;
                            else
                                _mat = _cart.st_matricula;

                            if (_cart.st_titularidade != "01")
                            {
                                 var tx = db.T_Dependente.
                                                FirstOrDefault(y => y.nu_titularidade == Convert.ToInt32(_cart.st_titularidade) &&
                                                               y.fk_proprietario == _cart.fk_dadosProprietario);

                                if (tx != null)
                                    assocNome = tx.st_nome;
                            }
                            else
                                assocNome = db.T_Proprietario.
                                                FirstOrDefault(y => y.i_unique == _cart.fk_dadosProprietario).st_nome;
                        }
                    }

                    var tipoTrans = "";

                    switch(tran.tg_contabil.ToString())
                    {
                        case "1": tipoTrans = "SITEF"; break;
                        case "2": tipoTrans = "Web"; break;
                        case "3": tipoTrans = "Mobile"; break;
                    }

                    lstIT.itens.Add(new ItensTrans
                    {
                        idstatus = tran.tg_confirmada.ToString(),
                        status = _stat,
                        dt = ObtemDataSegundos(tran.dt_transacao),
                        loja = loja != null ?  "(" + loja.st_loja + ") " + loja.st_nome : "",
                        nsu = tran.nu_nsu.ToString(),
                        mat = _mat,
                        assoc = assocNome,
                        parcelas = tran.nu_parcelas.ToString(),
                        terminal = term?.nu_terminal.ToString(),
                        valorTot = mon.setMoneyFormat((long)tran.vr_total),
                        tipo = tipoTrans,
                        motivo = tran.st_msg_transacao
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
