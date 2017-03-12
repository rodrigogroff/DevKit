using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class TaskController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new TaskFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),

					nuPriority = Request.GetQueryStringValue<long?>("nuPriority", null),
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),

					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new Task();

				var query = mdl.ComposedFilters(db, filter).
					OrderByDescending(y => y.id);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<Task> Output(IQueryable<Task> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.LoadAssociations(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Tasks select ne).Where(t => t.id == id).FirstOrDefault();

				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Post(Task mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Create(db, new Util().GetCurrentUser(db), ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Task mdl)
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
				var model = (from ne in db.Tasks select ne).Where(t => t.id == id).FirstOrDefault();

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = ""; if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				model.Delete(db);
								
				return Ok();
			}
		}
	}
}
