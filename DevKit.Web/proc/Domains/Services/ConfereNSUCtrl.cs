using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class ConfereNSUController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var nsu = Request.GetQueryStringValue<int>("nsu", 0);
            var dt = ObtemData(Request.GetQueryStringValue<string>("dt", ""));

            DateTime _dt = DateTime.Now;

            if (dt != null)
                _dt = Convert.ToDateTime(dt);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            long? fkLoja = null;

            if (db.currentLojista != null)
                fkLoja = (long) db.currentLojista.i_unique;

            var q = (from e in db.LOG_Transacoes

                        where fkLoja != null && e.fk_loja == fkLoja || fkLoja == null
                       where e.nu_nsu == nsu

                       where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()
                       
                       where e.dt_transacao.Value.Year == _dt.Year && e.dt_transacao.Value.Month == _dt.Month && e.dt_transacao.Value.Day == _dt.Day
                       
                     orderby e.dt_transacao descending

                       select e);

            
            var lTr = q.FirstOrDefault();

            if (lTr == null)
                return BadRequest("NSU Inválido");

            var cart = (from e in db.T_Cartao
                        where e.i_unique == lTr.fk_cartao
                        select e).
                        FirstOrDefault();

            var prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();                

            var lstRes = new List<string>();
            
            lstRes.Add("Data e hora: " + Convert.ToDateTime(lTr.dt_transacao).ToString("dd/MM/yyyy HH:mm")); // Data da venda
            lstRes.Add("Associado: " + prop.st_nome); // proprietário
            lstRes.Add("Cartão: E" + cart.st_empresa + " M" + cart.st_matricula);// cartão
            lstRes.Add("Valor total: R$ " + new money().setMoneyFormat((long)lTr.vr_total));

            return Ok(new
            {
                count = 0,
                results = lstRes
            });
        }
    }
}
