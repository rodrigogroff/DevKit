using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class DtoRelLancItem
    {
        public string cartao { get; set; }
        public string folha { get; set; }
        public string associado { get; set; }
        public string valor { get; set; }
        public string valorLanc { get; set; }
        public string total { get; set; }
    }

    public class EmissoraRelLancCCController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var mes = Request.GetQueryStringValue("mes");
            var ano = Request.GetQueryStringValue("ano");

            var lancs = new List<DtoRelLancItem>();

            var t_empresa = db.currentEmpresa;

            var lstFechamento = db.LOG_Fechamento.Where(y => y.fk_empresa == t_empresa.i_unique && y.st_mes == mes.PadLeft(2, '0') && y.st_ano == ano).ToList();
            var lstLancs = db.LancamentosCC.Where(y => y.fkEmpresa == t_empresa.i_unique && y.nuMes == Convert.ToInt32(mes) && y.nuAno == Convert.ToInt32(ano)).ToList();

            var env = db.LancamentosCCEmissao.FirstOrDefault(y => y.fkEmpresa == t_empresa.i_unique && y.nuMes == Convert.ToInt32(mes) && y.nuAno == Convert.ToInt32(ano));

            var lstCartsLanc = lstLancs.Select ( y=> (int) y.fkCartao ).Distinct().ToList();
            var lstCartsFech = lstFechamento.Select(y => (int) y.fk_cartao ).Distinct().ToList();

            var lstCarts = new List<int>();

            foreach (var item in lstCartsFech)
                if (!lstCarts.Contains(item))
                    lstCarts.Add(item);

            foreach (var item in lstCartsLanc)
                if (!lstCarts.Contains(item))
                    lstCarts.Add(item);

            var m = new money();
            var vlrRemessa = 0;

            foreach (var item in lstCarts)
            {
                var t_cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == item);

                int vlrFech = lstFechamento.Where(y => y.fk_cartao == item).Sum(y => (int) y.vr_valor);
                int vlrLanc = lstLancs.Where(y => y.fkCartao == item).Sum(y => (int)y.vrValor);

                vlrRemessa += vlrFech + vlrLanc;

                lancs.Add(new DtoRelLancItem
                { 
                    associado = db.T_Proprietario.FirstOrDefault(y=> y.i_unique == t_cart.fk_dadosProprietario)?.st_nome,
                    cartao = t_cart.st_matricula,
                    folha = t_cart.stCodigoFOPA,
                    valor = m.setMoneyFormat(vlrFech),
                    valorLanc = m.setMoneyFormat(vlrLanc),
                    total = m.setMoneyFormat(vlrLanc + vlrFech),
                });
            }

            return Ok(new
            {
                emissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                envio = Convert.ToDateTime(env.dtLanc).ToString("dd/MM/yyyy HH:mm"),
                totRegistros = lancs.Count(),
                totalRemessa = m.setMoneyFormat(vlrRemessa),
                lancs                
            });            
        }
    }
}
