using DataModel;
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
				var count = 0; var mdl = new Task();

				var results = mdl.ComposedFilters(db, ref count, new TaskFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),
					complete = Request.GetQueryStringValue<bool?>("complete", null),
					kpa = Request.GetQueryStringValue<bool?>("kpa", null),
					nuPriority = Request.GetQueryStringValue<long?>("nuPriority", null),
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
					fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
					fkSprint = Request.GetQueryStringValue<long?>("fkSprint", null),
					fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),
					fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
					fkTaskFlowCurrent = Request.GetQueryStringValue<long?>("fkTaskFlowCurrent", null),
					fkUserStart = Request.GetQueryStringValue<long?>("fkUserStart", null),
					fkUserResponsible = Request.GetQueryStringValue<long?>("fkUserResponsible", null),
				});

				return Ok(new { count = count, results = results });
			}
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.Task(id);

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

				if (!mdl.Create(db, db.GetCurrentUser(), ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Task mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				var usr = db.GetCurrentUser();

				if (!mdl.Update(db, usr, ref resp))
					return BadRequest(resp);

				return Ok(mdl);				
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.Task(id);

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = "";

				if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				model.Delete(db, db.GetCurrentUser());
								
				return Ok();
			}
		}
	}
}
