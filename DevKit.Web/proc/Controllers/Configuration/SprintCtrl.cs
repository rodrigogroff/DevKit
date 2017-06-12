using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SprintController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new ProjectSprint();

			var results = mdl.ComposedFilters(db, ref count, new ProjectSprintFilter()
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = db.GetProjectSprint(id);

            if (mdl != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(mdl);

                return Ok(mdl.LoadAssociations(db));
            }

            return StatusCode(HttpStatusCode.NotFound);			
		}

		public IHttpActionResult Put(long id, ProjectSprint mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}
	}
}
