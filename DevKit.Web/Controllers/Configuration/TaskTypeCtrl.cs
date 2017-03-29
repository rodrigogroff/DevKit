using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class TaskTypeController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new TaskTypeFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),

					fkProject = Request.GetQueryStringValue<long?>("fkProject", null)
				};

				var mdl = new TaskType();
				var util = new Util();

				var query = mdl.ComposedFilters(db, filter, util.GetCurrentUserProjects(db)).
					OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<TaskType> Output(IQueryable<TaskType> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.LoadAssociations(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.TaskTypes select ne).
					Where(t => t.id == id).
					FirstOrDefault();

				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Post(TaskType mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Create(db, new Util().GetCurrentUser(db), ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, TaskType mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Update(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);				
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.TaskTypes select ne).
					Where(t => t.id == id).
					FirstOrDefault();

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = "";

				if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);
					
				db.Delete(model);
				
				return Ok();
			}
		}
	}
}
