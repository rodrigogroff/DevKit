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
        public string data, nsu, associado, valor, parcelas, terminal, totalTransacoes, totalValor;
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
                         //where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()
                         where e.fk_loja == db.currentUser.i_unique
                         select e);

            if (confirmada != null)
                if (confirmada == true)
                    query = (from e in query
                             where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()
                             select e);

            if (cancelada != null)
                if (cancelada == true)
                    query = (from e in query
                             where e.tg_confirmada.ToString() == TipoConfirmacao.Cancelada.ToString()
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
                }
            }

            var totTrans = query.Count();
            var totValor = query.Sum(y => y.vr_total);
            
            var res = new List<RelLojistaTransItem>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var cart = (from e in db.T_Cartao
                            where e.i_unique == item.fk_cartao
                            select e).
                            FirstOrDefault();

                var assoc = (from e in db.T_Proprietario
                             where e.i_unique == cart.fk_dadosProprietario
                             select e).
                             FirstOrDefault();

                var term = (from e in db.T_Terminal
                             where e.i_unique == item.fk_terminal
                             select e).
                             FirstOrDefault();

                var nomeAssoc = "(não definido)";

                if (assoc != null)
                    nomeAssoc = assoc.st_nome;

                var mon = new money();

                res.Add(new RelLojistaTransItem
                {
                    data = Convert.ToDateTime(item.dt_transacao).ToString("dd/MM/yyyy HH:mm:ss"),
                    nsu = item.nu_nsu.ToString(),
                    associado = nomeAssoc,
                    valor = mon.setMoneyFormat((long)item.vr_total),
                    parcelas = item.nu_parcelas.ToString(),
                    terminal = term.nu_terminal.ToString(),
                    totalTransacoes = totTrans.ToString(),
                    totalValor = mon.setMoneyFormat((long)totValor)
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
