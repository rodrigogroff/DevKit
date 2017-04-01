using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskAccumulatorController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var count = 0; var mdl = new TaskTypeAccumulator();

				var results = mdl.ComposedFilters(db, ref count, new TaskAccumulatorFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),
					fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null)
				});

				return Ok(new { count = count, results = results });
			}
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.TaskTypeAccumulator(id);

				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}
	}
}
