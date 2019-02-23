using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class CancelaVendaDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var dt = Convert.ToDateTime(this.ObtemData(Request.GetQueryStringValue("dt")));
            var nsu = Request.GetQueryStringValue("nsu");
            
            var q = (from e in db.LOG_Transacoes

                     where e.nu_nsu.ToString() == nsu.ToString()
                     where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()

                     where e.dt_transacao.Value.Year == dt.Year
                     where e.dt_transacao.Value.Month == dt.Month
                     where e.dt_transacao.Value.Day == dt.Day

                     orderby e.dt_transacao descending

                     select e);

            var lTr = q.FirstOrDefault();

            if (lTr == null)
                return BadRequest("NSU Inválido");

            lTr.tg_confirmada = Convert.ToChar(TipoConfirmacao.Cancelada);
            lTr.st_msg_transacao = "Canc. DBA " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            db.Update(lTr);

            return Ok();
        }
    }
}
