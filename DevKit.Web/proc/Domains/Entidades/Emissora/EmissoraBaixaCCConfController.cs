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
        public string vlrSaldo { get; set; }
        public string status { get; set; }
        public string nuStatus { get; set; }
    }

    public class EmissoraBaixaCCConfController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var dtAtu = new DateTime(ano, mes, 1);
            
            var lst = new List<DtoBaixaConf>();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var tEmp = db.currentEmpresa;

            var lstTiposBaixa = db.EmpresaDespesa.
                                Where(y => y.fkEmpresa == tEmp.i_unique).
                                Where(y => y.stCodigo == "10" || y.stCodigo == "11").
                                Select ( y => (int) y.id).
                                ToList();

            var queryLancs = db.LancamentosCC.
                                    Where(y => y.fkEmpresa == tEmp.i_unique).
                                    Where(y => y.nuAno == ano).
                                    Where(y => y.nuMes == mes).
                                    Where(y=> !lstTiposBaixa.Contains( (int) y.fkTipo)).
                                    ToList();

            var queryFechs = db.LOG_Fechamento.
                                    Where(y => y.fk_empresa == tEmp.i_unique).
                                    Where(y => y.st_ano == ano.ToString()).
                                    Where(y => y.st_mes == mes.ToString().PadLeft(2, '0')).
                                    ToList();

            var queryBaixa = db.LancamentosCC.
                                    Where(y => y.fkEmpresa == tEmp.i_unique).
                                    Where(y => y.nuAno == dtAtu.Year).
                                    Where(y => y.nuMes == dtAtu.Month).
                                    Where(y=> lstTiposBaixa.Contains ( (int) y.fkTipo)).                                    
                                    ToList();

            var lstCarts = new List<int>();

            lstCarts.AddRange(queryLancs.Select(y => (int)y.fkCartao).ToList());
            lstCarts.AddRange(queryFechs.Select(y => (int)y.fk_cartao).ToList());

            lstCarts = lstCarts.Distinct().ToList();

            var lstDbCards = db.T_Cartao.Where(y => lstCarts.Contains((int)y.i_unique)).ToList();
            var lstProps = lstDbCards.Select(y => (int) y.fk_dadosProprietario).ToList();
            var lstDbProp = db.T_Proprietario.Where(y => lstProps.Contains((int)y.i_unique)).ToList();

            int nu_tot = 0,
                    nu_nliq = 0,
                    nu_liq = 0,
                    nu_pend = 0;

            foreach (var item in lstCarts)
            {
                var t_card = lstDbCards.FirstOrDefault(y => y.i_unique == item);
                var t_prop = lstDbProp.FirstOrDefault(y => y.i_unique == t_card.fk_dadosProprietario);

                var vlrTotLanc = queryLancs.Where (y=> y.fkCartao == item).Sum(y => (long)y.vrValor);                
                var vlrTotFech = queryFechs.Where(y => y.fk_cartao == item).Sum(y => (long)y.vr_valor);

                var vlrTots = vlrTotLanc + vlrTotFech;
                var vlrTotBaixa = queryBaixa.Where(y => y.fkCartao == item).Sum(y => (long)y.vrValor);

                nu_tot++;

                if (!queryBaixa.Any(y => y.fkCartao == item))
                    nu_pend++;
                else
                    if (vlrTotBaixa != vlrTots - vlrTotBaixa)
                    nu_nliq++;
                else
                    nu_liq++;

                var baixaConf = new DtoBaixaConf
                {
                    idCartao = Convert.ToInt64(t_card.i_unique),
                    mat = t_card.st_matricula +
                     " / " + t_card.stCodigoFOPA,
                    nome = t_prop.st_nome,
                    vlrTotLanc = mon.setMoneyFormat(vlrTots),
                    vlrTotBaixa = mon.setMoneyFormat(vlrTotBaixa),
                    vlrSaldo = mon.setMoneyFormat(vlrTots - vlrTotBaixa),
                    status = !queryBaixa.Any(y => y.fkCartao == item) ? "Não Processado" : vlrTotBaixa != vlrTots - vlrTotBaixa ? "Liquidado" : "Não Liquidado",
                    nuStatus = !queryBaixa.Any(y => y.fkCartao == item) ? "1" : vlrTotBaixa != vlrTots - vlrTotBaixa ? "3" : "2",
                };

                if (baixaConf.vlrSaldo != "0,00")
                    if (baixaConf.nuStatus == "3")
                        baixaConf.status += " (Parcial)";

                lst.Add(baixaConf);
            }

            lst = lst.OrderBy(y => y.nome).ToList();

            return Ok(
                new 
                { 
                    count = lst.Count(), 
                    results = lst,
                    vlr_tot = "[" + nu_tot+ "]",
                    vlr_nliq = "[" + nu_nliq + "]",
                    vlr_liq = "[" + nu_liq + "]",
                    vlr_pend = "[" + nu_pend + "]",
                });
        }        
    }
}
