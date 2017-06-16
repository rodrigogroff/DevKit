using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCheckPointController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new TaskCheckPoint();

            var results = mdl.ComposedFilters(db, ref count, new TaskCheckPointFilter()
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkCategory = Request.GetQueryStringValue<long?>("fkCategory", null)
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTaskCheckPoint(id);

			if (mdl != null)
				return Ok(mdl);

			return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
