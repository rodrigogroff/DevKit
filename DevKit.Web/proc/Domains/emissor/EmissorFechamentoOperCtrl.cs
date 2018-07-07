using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class EmissorFechamentoOperController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            return Ok(new Empresa().EmissorFechamentoOper(db, new EmissorFechamentoOper_PARAMS
            {
                nsu = Request.GetQueryStringValue("nsu"),
                oper = Request.GetQueryStringValue("oper"),
                tgSituacaoLote = Request.GetQueryStringValue("tgSituacaoLote"),
            }));
        }
	}
}
