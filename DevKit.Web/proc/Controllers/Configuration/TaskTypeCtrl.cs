using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskTypeController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new TaskType();

            var filter = new TaskTypeFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null)
            };

            var hshReport = SetupCacheReport(CacheObject.ClientReports);
            if (hshReport[filter.Parameters()] is TaskTypeReport report)
                return Ok(report);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true
            };

            var results = mdl.ComposedFilters(db, ref count, filter, options);

            var ret = new TaskTypeReport
            {
                count = count,
                results = results
            };

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            var combo = Request.GetQueryStringValue("combo", false);

            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.TaskType, id) as Client;
            if (obj != null)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            var mdl = db.GetTaskType(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true,
                bLoadCheckPoints = true,
                bLoadLogs = true
            };            

            mdl.LoadAssociations(db, options);
            BackupCache(mdl);

            if (combo)
                return Ok(mdl.ClearAssociations());
            else
                return Ok(mdl);
        }

		public IHttpActionResult Post(TaskType mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true,
                bLoadCheckPoints = true,
                bLoadLogs = true
            };

            mdl.LoadAssociations(db, options);

            CleanCache(db, CacheObject.TaskType, null);
            StoreCache(CacheObject.TaskType, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, TaskType mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true,
                bLoadCheckPoints = true,
                bLoadLogs = true
            };

            mdl.LoadAssociations(db, options);

            StoreCache(CacheObject.TaskType, mdl.id, mdl);

            return Ok();
        }

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();
            
			var mdl = db.GetTaskType(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);

			if (!mdl.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);
				
			mdl.Delete(db);
            
            CleanCache(db, CacheObject.TaskType, null);

            return Ok();			
		}
	}
}
