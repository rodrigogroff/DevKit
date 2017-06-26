using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskFilter
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

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskReport);            
            if (hshReport[parameters] is TaskReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Task().ComposedFilters(db, filter, new loaderOptionsTask
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

            hshReport[parameters] = ret;

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Task, id) != null)
                return base.Ok(RestoreCache(CacheTags.Task, id));

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTask(id);
            
            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db, new loaderOptionsTask
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
            });

            BackupCache(mdl);

            return Ok(mdl);		
		}

		public IHttpActionResult Post(Task mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            CleanCache(db, CacheTags.Task, null);
            StoreCache(CacheTags.Task, mdl.id, mdl);

            return Ok(mdl);			
		}

		public IHttpActionResult Put(long id, Task mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            StoreCache(CacheTags.Task, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTask(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

            mdl.Delete(db);

            CleanCache(db, CacheTags.Task, null);

            return Ok();			
		}
	}
}
