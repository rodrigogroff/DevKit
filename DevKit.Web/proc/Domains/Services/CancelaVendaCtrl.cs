using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class CancelaVendaController : ApiControllerBase
    {
        public string terminal,
                      empresa,
                      matricula,
                      strMessage,
                      retorno;

        public long titularidadeFinal;

        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var nsu = Request.GetQueryStringValue("nsu");

            terminal = userLoggedName.PadLeft(8, '0');

            var dtNow = DateTime.Now;

            var q = (from e in db.LOG_Transacoes

                     where e.fk_loja == db.currentLojista.i_unique
                     where e.nu_nsu.ToString() == nsu.ToString()

                     where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada.ToString()

                     where e.dt_transacao.Value.Year == dtNow.Year
                     where e.dt_transacao.Value.Month == dtNow.Month
                     where e.dt_transacao.Value.Day == dtNow.Day

                     orderby e.dt_transacao descending

                     select e);

            var lTr = q.FirstOrDefault();

            if (lTr == null)
                return BadRequest("NSU Inválido");

            var veCanc = new VendaEmpresarialCancelamento();

            veCanc.Run(db, nsu);

            if (veCanc.var_codResp != "0000")
                return BadRequest("Falha (0xE" + veCanc.var_codResp + ")");

            CleanCache(db, CacheTags.associado, (long)lTr.fk_cartao);

            var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == lTr.fk_cartao);
            var prop = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario);

            var cupom = new Cupom().
                Cancelamento(db,
                               cart,
                               veCanc.var_nu_nsuAtual,
                               terminal,
                               nsu,
                               lTr,
                               prop);

            return Ok(new
            {
                count = 1,
                results = cupom
            });
        }
    }
}
