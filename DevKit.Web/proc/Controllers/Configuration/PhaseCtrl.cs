using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PhaseController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ProjectPhase();

			var results = mdl.ComposedFilters(db, ref reportCount, new ProjectPhaseFilter()
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkProject = Request.GetQueryStringValue<int?>("fkProject", null),
			});

			return Ok(new { count = reportCount, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectPhase(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
                       
            return Ok(mdl);            
		}
	}
}
