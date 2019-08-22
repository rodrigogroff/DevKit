using LinqToDB;
using System.Linq;
using System.Web.Http;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class ConfirmacaoAutoServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/ConfirmacaoAutoServerISO")]
        public IHttpActionResult Get()
        {
            using (var db = new AutorizadorCNDB())
            {
                var dtNow = DateTime.Now;

                var dtIni = dtNow.AddSeconds(-60 * 6);
                var dtFim = dtNow.AddDays(-2);

                var queryX = db.LOG_Transacoes.
                                Where(y => y.dt_transacao > dtFim && y.dt_transacao < dtIni &&
                                           y.tg_confirmada.ToString() == TipoConfirmacao.Pendente &&
                                           y.tg_contabil.ToString() == TipoCaptura.SITEF).
                                ToList();

                foreach (var item in queryX)
                {
                    item.tg_confirmada = Convert.ToChar(TipoConfirmacao.Confirmada);
                    item.st_msg_transacao = "Conf. Auto";

                    db.Update(item);
                }
            }

            return Ok();
        }
    }
}
