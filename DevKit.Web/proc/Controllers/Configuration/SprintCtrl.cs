using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SprintController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ProjectSprint();

			var results = mdl.ComposedFilters(db, ref reportCount, new ProjectSprintFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
			});

			return Ok(new { count = reportCount, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectSprint(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            var combo = Request.GetQueryStringValue("combo", false);

            if (combo)
                return Ok(mdl);

            return Ok(mdl.LoadAssociations(db));
		}

		public IHttpActionResult Put(long id, ProjectSprint mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
				return BadRequest(apiResponse);

			return Ok();			
		}
	}
}
