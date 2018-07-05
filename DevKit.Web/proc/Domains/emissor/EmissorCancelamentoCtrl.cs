using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmissorCancelamentoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Empresa().CancelamentoAutorizacao(db, new Cancelamento_PARAMS
            {
                codCredenciado = Request.GetQueryStringValue("codCredenciado"),
                matricula = Request.GetQueryStringValue("matricula"),
                fkSecao = Request.GetQueryStringValue("fkSecao"),
                lote = Request.GetQueryStringValue<bool?>("lote", null),
                dt = ObtemData(Request.GetQueryStringValue("dt")),
                nsu = Request.GetQueryStringValue("nsu"),
            });

            if (!string.IsNullOrEmpty(ret))
                return BadRequest(ret);
            else
                return Ok();
        }
    }
}
