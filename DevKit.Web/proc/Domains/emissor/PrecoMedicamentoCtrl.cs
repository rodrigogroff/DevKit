using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PrecoMedicamentoController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new PrecoMedicamentoFilter
            {                
                skip = Request.GetQueryStringValue("mes", 0),
                take = Request.GetQueryStringValue("ano", 15),
                codigo = Request.GetQueryStringValue("codigo"),                
            };

            return Ok(new SaudeValorMedicamento().Listagem(db, filter));
        }
	}
}
