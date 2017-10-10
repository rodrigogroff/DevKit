using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class RelLojistaTransItem
    {
        public string data,
                        nsu, 
                        situacao,
                        associado, 
                        valor, 
                        parcelas, 
                        terminal, 
                        totalTransacoes,
                        totalValor;

        public List<string> cupom;
    }

    public class RelLojistaTransController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            
            var idTerminal = Request.GetQueryStringValue<int?>("idTerminal", null);
            var idOrdem = Request.GetQueryStringValue<int?>("idOrdem", null);
            var nsu = Request.GetQueryStringValue<int?>("nsu", null);

            var dia_inicial = Request.GetQueryStringValue<int?>("dia_inicial", null);
            var dia_final = Request.GetQueryStringValue<int?>("dia_final", null);
            var mes_inicial = Request.GetQueryStringValue<int?>("mes_inicial", null);
            var mes_final = Request.GetQueryStringValue<int?>("mes_final", null);
            var ano_inicial = Request.GetQueryStringValue<int?>("ano_inicial", null);
            var ano_final = Request.GetQueryStringValue<int?>("ano_final", null);

            var confirmada = Request.GetQueryStringValue<bool?>("confirmada", null);
            var cancelada = Request.GetQueryStringValue<bool?>("cancelada", null);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.LOG_Transacoes
                         where e.fk_loja == db.currentUser.i_unique
                         select e);

            var lstSits = new List<string>();

            if (confirmada == null) confirmada = false;
            if (cancelada == null) cancelada = false;

            if (confirmada == true)
                lstSits.Add(TipoConfirmacao.Confirmada);

            if (cancelada == true)
                lstSits.Add(TipoConfirmacao.Cancelada);

            if (confirmada == false && cancelada == false)
            {
                lstSits.Add(TipoConfirmacao.Pendente);
                lstSits.Add(TipoConfirmacao.Confirmada);
                lstSits.Add(TipoConfirmacao.Negada);
                lstSits.Add(TipoConfirmacao.Erro);
                lstSits.Add(TipoConfirmacao.Registro);
                lstSits.Add(TipoConfirmacao.Cancelada);
                lstSits.Add(TipoConfirmacao.Desfeita);
            }

            query = (from e in query
                     where lstSits.Contains(e.tg_confirmada.ToString())
                     select e);
            
            if (idTerminal != null)
            {
                query = (from e in query
                        where e.fk_terminal == idTerminal
                        select e);  
            }

            if (dia_inicial != null && mes_inicial != null && ano_inicial != null)
            {
                var dtInicial = new DateTime((int)ano_inicial, (int)mes_inicial, (int)dia_inicial);

                query = (from e in query
                         where e.dt_transacao >= dtInicial
                         select e);
            }

            if (dia_final != null && mes_final != null && ano_final != null)
            {
                var dtFinal = new DateTime((int)ano_final, (int)mes_final, (int)dia_final);

                dtFinal = dtFinal.AddDays(1);

                query = (from e in query
                         where e.dt_transacao < dtFinal
                         select e);
            }

            if (nsu != null)
                query = (from e in query
                         where e.nu_nsu == nsu
                         select e);

            if (idOrdem != null)
            {
                switch ((long)idOrdem)
                {
                    case EnumOrdemRelLojistaTrans.data:
                        query = (from e in query
                                 orderby e.dt_transacao descending
                                 select e);
                        break;

                    case EnumOrdemRelLojistaTrans.valor:
                        query = (from e in query
                                 orderby e.vr_total descending
                                 select e);
                        break;

                    case EnumOrdemRelLojistaTrans.associado:
                        query = (from e in query
                                 join c in db.T_Cartao on e.fk_cartao equals (int)c.i_unique
                                 join p in db.T_Proprietario on c.fk_dadosProprietario equals (int)p.i_unique
                                 orderby p.st_nome 
                                 select e);
                        break;
                }
            }

            var totTrans = query.Count();

            long totValor = (long)  query.
                                    Where (y=> y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                    Sum(y => (decimal) y.vr_total);

            var mon = new money();
            var res = new List<RelLojistaTransItem>();

            foreach (var trans in query.Skip(skip).Take(take).ToList())
            {
                var nomeAssoc = "(não definido)";
                var nomeTerm = "";

                var cart = (from e in db.T_Cartao
                            where e.i_unique == trans.fk_cartao
                            select e).
                            FirstOrDefault();

                T_Proprietario prop = null;

                if (cart!= null)
                {
                    prop = (from e in db.T_Proprietario
                                 where e.i_unique == cart.fk_dadosProprietario
                                 select e).
                                 FirstOrDefault();

                    if (prop != null)
                    {
                        nomeAssoc = prop.st_nome;
                    }
                }
                
                var term = (from e in db.T_Terminal
                             where e.i_unique == trans.fk_terminal
                             select e).
                             FirstOrDefault();

                if (term != null)
                    nomeTerm = term.nu_terminal;

                var sit = "";
                
                List<string> cupom = new List<string>();

                switch (trans.tg_confirmada.ToString())
                {
                    case TipoConfirmacao.Cancelada:
                        {
                            sit = "Cancelada";
                            
                            cupom = new Cupom().Cancelamento ( db, 
                                                               cart, 
                                                               trans.nu_nsuOrig.ToString(), 
                                                               nomeTerm, 
                                                               trans.nu_nsu.ToString(), 
                                                               trans, 
                                                               prop );

                            break;
                        }
                    
                    case TipoConfirmacao.Confirmada:
                        {
                            sit = "Confirmada";

                            int i = 0,  p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0, p6 = 0, p7 = 0, p8 = 0, p9 = 0, p10 = 0, p11 = 0, p12 = 0;

                            foreach (var parcela in (from e in db.T_Parcelas where e.fk_log_transacoes == trans.i_unique orderby e.nu_parcela select e.vr_valor).ToList())
                            {
                                switch (++i)
                                {
                                    case 1: p1 = (int)parcela; break;
                                    case 2: p2 = (int)parcela; break;
                                    case 3: p3 = (int)parcela; break;
                                    case 4: p4 = (int)parcela; break;
                                    case 5: p5 = (int)parcela; break;
                                    case 6: p6 = (int)parcela; break;
                                    case 7: p7 = (int)parcela; break;
                                    case 8: p8 = (int)parcela; break;
                                    case 9: p9 = (int)parcela; break;
                                    case 10: p10 = (int)parcela; break;
                                    case 11: p11 = (int)parcela; break;
                                    case 12: p12 = (int)parcela; break;
                                }
                            }

                            cupom = new Cupom().Venda ( db, 
                                                        cart, 
                                                        prop, 
                                                        trans.nu_nsu.ToString(), 
                                                        nomeTerm, 
                                                        (int) trans.nu_parcelas, 
                                                        (int) trans.vr_total, 
                                                        p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 ); 

                            break;
                        }

                    case TipoConfirmacao.Pendente: sit = "Pendente"; break;
                    case TipoConfirmacao.Negada: sit = "Negada"; break;
                    case TipoConfirmacao.Erro: sit = "Erro: " + trans.st_msg_transacao; break;
                    case TipoConfirmacao.Registro: sit = "Registro"; break;                    
                    case TipoConfirmacao.Desfeita: sit = "Desfeita"; break;
                }

                res.Add(new RelLojistaTransItem
                {
                    data = Convert.ToDateTime(trans.dt_transacao).ToString("dd/MM/yyyy HH:mm:ss"),
                    nsu = trans.nu_nsu.ToString(),
                    situacao = sit,
                    associado = nomeAssoc,
                    valor = mon.setMoneyFormat((long)trans.vr_total),
                    parcelas = trans.nu_parcelas.ToString(),
                    terminal = nomeTerm,
                    totalTransacoes = totTrans.ToString(),
                    totalValor = mon.setMoneyFormat(totValor),
                    cupom = cupom
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
