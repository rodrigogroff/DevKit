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

            var hshReport = SetupCacheReport(CacheObject.TaskReports);
            
            if (hshReport[filter.Parameters()] is TaskReport report)
                return Ok(report);
            
            var ret = mdl.Report(db, ref count, filter, new loaderOptionsTask
            {
                bLoadTaskCategory = true,
                bLoadTaskType = true,
                bLoadProject = true,
                bLoadPhase = true,
                bLoadSprint = true,
                bLoadTaskFlow = true,
                bLoadVersion = true,
                bLoadUsers = true,
            });

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetTask(id);

            var options = new loaderOptionsTask
            {
                bLoadTaskCategory = true,
                bLoadTaskType = true,
                bLoadProject = true,
                bLoadPhase = true,
                bLoadSprint = true,
                bLoadTaskFlow = true,
                bLoadVersion = true,
                bLoadUsers = true,
                bLoadProgress = true,
                bLoadMessages = true,
                bLoadFlows = true,
                bLoadAccs = true,
                bLoadDependencies = true,
                bLoadCheckpoints = true,
                bLoadQuestions = true,
                bLoadClients = true,
                bLoadClientGroups = true,
                bLoadCustomSteps = true,
                bLoadLogs = true
            };

            if (model != null)
				return Ok(model.LoadAssociations(db, options));

			return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(Task mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            SetupCacheReport(CacheObject.TaskReports).Clear();

            if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}

		public IHttpActionResult Put(long id, Task mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            SetupCacheReport(CacheObject.TaskReports).Clear();

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

            SetupCacheReport(CacheObject.TaskReports).Clear();
            
            return Ok();			
		}
	}
}
