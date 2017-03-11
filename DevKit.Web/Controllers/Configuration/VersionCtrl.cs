using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class VersionController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new ProjectVersionFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),

					fkSprint = Request.GetQueryStringValue<int?>("fkSprint", null),
				};

				var mdl = new ProjectSprintVersion();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = Output(query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<ProjectSprintVersion> Output(IQueryable<ProjectSprintVersion> query, DevKitDB db)
		{
			return query.ToList();
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.ProjectSprintVersions select ne).Where(t => t.id == id).FirstOrDefault();

				if (model != null)
					return Ok(model);

				return StatusCode(HttpStatusCode.NotFound);
			}
		}
	}
}
