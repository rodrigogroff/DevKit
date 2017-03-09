using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;
using System.Threading;

namespace DevKit.Web.Controllers
{
	public class ProjectController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new ProjectFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new Project();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<Project> Output(IQueryable<Project> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.Load(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Projects select ne).Where(t => t.id == id).FirstOrDefault();

				if (model != null)
					return Ok(model.Load(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Post(Project mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Create(db, new Util().GetCurrentUser(db), ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Project mdl)
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
				var model = (from ne in db.Projects select ne).Where(t => t.id == id).FirstOrDefault();

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = ""; if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);
					
				db.Delete(model);
				
				return Ok();
			}
		}
	}
}
