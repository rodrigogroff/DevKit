using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class MedicoListagemAutorizacaoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var m = new Medico().ListagemAutorizacao(db, new ListagemMedicoAutorizacaoFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 0),
                tuss = Request.GetQueryStringValue("tuss"),
                nomeAssociado = Request.GetQueryStringValue("nomeAssociado"),
            });

            return Ok(m);
        }
    }
}
