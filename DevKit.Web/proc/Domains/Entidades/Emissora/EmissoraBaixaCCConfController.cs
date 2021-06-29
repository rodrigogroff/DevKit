using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoBaixaConf
    {
        public long idCartao { get; set; }
        public string nome { get; set; }
        public string mat { get; set; }
        public string vlrTotLanc { get; set; }
        public string vlrTotBaixa { get; set; }
        public string status { get; set; }
    }

    public class EmissoraBaixaCCConfController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var lst = new List<DtoBaixaConf>();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var tEmp = db.currentEmpresa;

            var lstTipos = db.EmpresaDespesa.
                                Where(y => y.fkEmpresa == tEmp.i_unique).
                                Where(y => y.stCodigo == "10" || y.stCodigo == "11").
                                Select ( y => (int) y.id).
                                ToList();

            var queryLancs = db.LancamentosCC.
                                    Where(y => y.fkEmpresa == tEmp.i_unique).
                                    Where(y => y.nuAno == ano).
                                    Where(y => y.nuMes == mes).
                                    ToList();

            var queryBaixa = db.LancamentosCC.
                                    Where(y => y.fkEmpresa == tEmp.i_unique).
                                    Where(y => y.nuAno == ano).
                                    Where(y => y.nuMes == mes).
                                    Where ( y=> y.fkBaixa > 0 || lstTipos.Contains((int)y.fkTipo)).
                                    ToList();

            var lstCarts = queryLancs.Select(y => (int) y.fkCartao).Distinct().ToList();
            var lstDbCards = db.T_Cartao.Where(y => lstCarts.Contains((int)y.i_unique)).ToList();
            var lstProps = lstDbCards.Select(y => (int) y.fk_dadosProprietario).ToList();
            var lstDbProp = db.T_Proprietario.Where(y => lstProps.Contains((int)y.i_unique)).ToList();

            foreach (var item in lstCarts)
            {
                var t_card = lstDbCards.FirstOrDefault(y => y.i_unique == item);
                var t_prop = lstDbProp.FirstOrDefault(y => y.i_unique == t_card.fk_dadosProprietario);

                var vlrTotLanc = queryLancs.Where (y=> y.fkCartao == item).Sum(y => (long)y.vrValor);
                var vlrTotBaixa = queryBaixa.Where(y => y.fkCartao == item).Sum(y => (long)y.vrValor);

                lst.Add(new DtoBaixaConf
                {
                    idCartao = Convert.ToInt64(t_card.i_unique),
                    mat = t_card.st_matricula,
                    nome = t_prop.st_nome,
                    vlrTotBaixa = mon.setMoneyFormat(vlrTotBaixa),
                    vlrTotLanc = mon.setMoneyFormat(vlrTotLanc),
                    status = !queryBaixa.Any( y=> y.fkCartao == item) ? "Pendente" : vlrTotLanc == vlrTotBaixa ? "OK" : "Postergado",
                });
            }

            lst = lst.OrderBy(y => y.nome).ToList();

            return Ok(new { count = lst.Count(), results = lst });
        }        
    }
}
