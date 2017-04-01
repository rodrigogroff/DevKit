using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;

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
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),
					complete = Request.GetQueryStringValue<bool?>("complete", null),
					nuPriority = Request.GetQueryStringValue<long?>("nuPriority", null),
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
					fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
					fkSprint = Request.GetQueryStringValue<long?>("fkSprint", null),
					fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),
					fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
					fkTaskFlowCurrent = Request.GetQueryStringValue<long?>("fkTaskFlowCurrent", null),
					fkUserStart = Request.GetQueryStringValue<long?>("fkUserStart", null),
					fkUserResponsible = Request.GetQueryStringValue<long?>("fkUserResponsible", null),	
				};

				var mdl = new Task();

				var query = mdl.ComposedFilters(db, filter).OrderByDescending(y => y.fkSprint);

				var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

				results.ForEach(y => { y = y.LoadAssociations(db); });

				return Ok(new { count = query.Count(), results = results });
			}
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Tasks select ne).
					Where(t => t.id == id).
					FirstOrDefault();

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

				var usr = new Util().GetCurrentUser(db);

				if (!mdl.Update(db, usr, ref resp))
					return BadRequest(resp);

				return Ok(mdl);				
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Tasks select ne).
					Where(t => t.id == id).
					FirstOrDefault();

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = "";

				if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				model.Delete(db);
								
				return Ok();
			}
		}
	}
}
