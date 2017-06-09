using DataModel;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var count = 0; var mdl = new Task();

			var results = mdl.ComposedFilters(db, ref count, new TaskFilter()
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				complete = Request.GetQueryStringValue<bool?>("complete", null),
				kpa = Request.GetQueryStringValue<bool?>("kpa", null),
				expired = Request.GetQueryStringValue<bool?>("expired", null),
				nuPriority = Request.GetQueryStringValue<long?>("nuPriority", null),
				fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
				fkSprint = Request.GetQueryStringValue<long?>("fkSprint", null),
				fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),					
				fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
				fkTaskFlowCurrent = Request.GetQueryStringValue<long?>("fkTaskFlowCurrent", null),
				fkUserStart = Request.GetQueryStringValue<long?>("fkUserStart", null),
				fkUserResponsible = Request.GetQueryStringValue<long?>("fkUserResponsible", null),
				fkClient = Request.GetQueryStringValue<long?>("fkClient", null),
				fkClientGroup = Request.GetQueryStringValue<long?>("fkClientGroup", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetTask(id);

            if (model != null)
				return Ok(model.LoadAssociations(db, new loaderOptionsTask(setupTask.TaskEdit)));

			return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(Task mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}

		public IHttpActionResult Put(long id, Task mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetTask(id);

			if (model == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!model.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            model.Delete(db);
								
			return Ok();			
		}
	}
}
