using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class TaskAccumulatorController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new TaskAccumulatorFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),

					fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),

					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new TaskTypeAccumulator();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<TaskTypeAccumulator> Output(IQueryable<TaskTypeAccumulator> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.LoadAssociations(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.TaskTypeAccumulators select ne).
					Where(t => t.id == id).
					FirstOrDefault();

				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}
	}
}
