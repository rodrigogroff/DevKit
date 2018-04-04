using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PrecoDiariaController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new PrecoDiariaFilter
            {                
                skip = Request.GetQueryStringValue("mes", 0),
                take = Request.GetQueryStringValue("ano", 15),
                codigo = Request.GetQueryStringValue("codigo"),                
            };

            return Ok(new SaudeValorDiaria().Listagem(db, filter));
        }
	}
}
