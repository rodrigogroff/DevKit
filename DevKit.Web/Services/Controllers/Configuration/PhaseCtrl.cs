using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PhaseController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new ProjectPhase();

			var results = mdl.ComposedFilters(db, ref count, new ProjectPhaseFilter()
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkProject = Request.GetQueryStringValue<int?>("fkProject", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetProjectPhase(id);

            if (model != null)
                return Ok(model);

            return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
