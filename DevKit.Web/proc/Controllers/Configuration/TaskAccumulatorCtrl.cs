using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskAccumulatorController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new TaskTypeAccumulator();

			var results = mdl.ComposedFilters(db, ref reportCount, new TaskTypeAccumulatorFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null)
			});

			return Ok(new { count = reportCount, results = results });
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTaskTypeAccumulator(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            var combo = Request.GetQueryStringValue("combo", false);

            if (combo)
                return Ok(mdl);

            return Ok(mdl.LoadAssociations(db));
		}
	}
}
