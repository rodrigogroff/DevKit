using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    
    public class EmissoraLancCCExtratoReceberController : ApiControllerBase
    {
        public class DtoExtratoReceber_LancCC
        {
            public long id { get; set; }
            public string cartao { get; set; }
            public string associado { get; set; }
            public string vlrTotal { get; set; }            
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var mon = new money();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            long totCC = 0;

            var tEmp = db.currentEmpresa;
            
            var lstCC = new List<DtoExtratoReceber_LancCC>();

            // ---------------------------------------------------------------

            var queryLancCC = ( from e in db.LancamentosCC
                                where e.fkEmpresa == tEmp.i_unique
                                where e.nuAno == ano
                                where e.nuMes == mes
                                select e).
                                ToList();

            var queryLancFech = (from e in db.LOG_Fechamento
                               where e.fk_empresa == tEmp.i_unique
                               where e.st_ano == ano.ToString()
                               where e.st_mes == mes.ToString().PadLeft(2, '0')
                               select e).
                                ToList();

            var idsCartoes = queryLancCC.Select(y => (long)y.fkCartao).Distinct().ToList();

            idsCartoes.AddRange(queryLancFech.Select(y => (long)y.fk_cartao).Distinct().ToList());

            var lstCards = db.T_Cartao.Where(y => idsCartoes.Contains((long)y.i_unique)).ToList();

            var _id = 1;

            foreach (var item in idsCartoes)
            {
                var t_card = lstCards.FirstOrDefault(y => y.i_unique == item);
                var t_prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == t_card.fk_dadosProprietario);

                var tot = queryLancCC.Where(y => y.fkCartao == item).Sum(y => (long)y.vrValor);

                tot += queryLancFech.Where(y => y.fk_cartao == item).Sum(y => (long)y.vr_valor);

                var rec = new DtoExtratoReceber_LancCC
                {
                    id = 1,
                    associado = t_prop.st_nome,
                    cartao = t_card.st_matricula,
                    vlrTotal = mon.setMoneyFormat(tot),
                };

                lstCC.Add(rec);
                totCC += tot;
            }

            lstCC = lstCC.OrderBy(y => y.associado).ToList();

            var idN = 1;
            foreach (var item in lstCC)
                item.id = idN++;

            return Ok(new 
            {
                vlrTotCC = "R$ " + mon.setMoneyFormat(totCC),
                listDespCC = lstCC,
            }); 
        }        
    }
}
