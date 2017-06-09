using DataModel;
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
            
            var mdl = new Task();

            var filter = new TaskFilter()
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
            };

            var currentParameters = filter.Export();            

            var hshReport = SetupCacheReport(CachedObject.TaskReports);
            
            if (hshReport[currentParameters] is TaskReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter );

            var ret = new TaskReport
            {
                count = count,
                results = results
            };

            hshReport[currentParameters] = ret;

            return Ok(ret);
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

            SetupCacheReport(CachedObject.TaskReports).Clear();

            if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}

		public IHttpActionResult Put(long id, Task mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            SetupCacheReport(CachedObject.TaskReports).Clear();

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

            SetupCacheReport(CachedObject.TaskReports).Clear();
            
            return Ok();			
		}
	}
}
