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
        public string data, nsu, associado, valor, parcelas, terminal;
    }

    public class RelLojistaTransController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var idTerminal = Request.GetQueryStringValue<int?>("idTerminal", null);
            var idOrdem = Request.GetQueryStringValue<int?>("idOrdem", null);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.LOG_Transacoes
                         where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()
                         where e.fk_loja == db.currentUser.i_unique
                         select e);

            if (idTerminal != null)
            {
                query = (from e in query
                        where e.fk_terminal == idTerminal
                        select e);  
            }

            if (idOrdem != null)
            {
                switch ((long)idOrdem)
                {
                    case EnumOrdemRelLojistaTrans.data:
                        query = (from e in query orderby e.dt_transacao descending select e);
                        break;
                }
            }            
            
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
                    terminal = term.nu_terminal.ToString()
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
