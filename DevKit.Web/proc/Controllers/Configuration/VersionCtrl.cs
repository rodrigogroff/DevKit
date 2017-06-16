using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class VersionController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ProjectSprintVersion();

			var results = mdl.ComposedFilters(db, ref reportCount, new ProjectSprintVersionFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkSprint = Request.GetQueryStringValue<int?>("fkSprint", null),
			});

			return Ok(new { count = reportCount, results = results });
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectSprintVersion(id);

			if (mdl != null)
				return Ok(mdl);

			return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
