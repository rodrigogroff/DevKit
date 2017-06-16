using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProjectController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Project();

			var results = mdl.ComposedFilters(db, ref count, new ProjectFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

			if (mdl != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(mdl);

                if (!db.GetCurrentUserProjects().Contains(id))
                    return StatusCode(HttpStatusCode.NotFound);
                else
                    return Ok(mdl.LoadAssociations(db));
            }

            return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiResponse))
				return BadRequest(apiResponse);

			return Ok();
		}

		public IHttpActionResult Put(long id, Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
					return BadRequest(apiResponse);

			return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

			mdl.Delete(db);
								
			return Ok();
		}
	}
}
