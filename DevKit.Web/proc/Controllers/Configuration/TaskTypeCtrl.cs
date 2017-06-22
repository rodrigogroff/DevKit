using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskTypeController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskTypeFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null)
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ClientReport);
            if (hshReport[parameters] is TaskTypeReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new TaskType();

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true
            };

            var results = mdl.ComposedFilters(db, ref reportCount, filter, options);

            var ret = new TaskTypeReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            var combo = Request.GetQueryStringValue("combo", false);

            var obj = RestoreCache(CacheTags.TaskType, id) as Client;
            if (obj != null)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

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
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true,
                bLoadCheckPoints = true,
                bLoadLogs = true
            };

            mdl.LoadAssociations(db, options);

            CleanCache(db, CacheTags.TaskType, null);
            StoreCache(CacheTags.TaskType, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, TaskType mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            var options = new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true,
                bLoadCheckPoints = true,
                bLoadLogs = true
            };

            mdl.LoadAssociations(db, options);

            
            
            switch (mdl.updateCommand)
            {                
                case "newCategorie": 
                case "removeCategorie":
                    break;

                case "newFlow": 
                case "removeFlow":
                    break;

                case "newAcc": 
                case "removeAcc":
                    CleanCache(db, CacheTags.TaskTypeAccumulator, null);
                    break;

                case "newCC": 
                case "removeCC":
                    break;
            }

            CleanCache(db, CacheTags.TaskType, null);
            StoreCache(CacheTags.TaskType, mdl.id, mdl);

            return Ok();
        }

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
			var mdl = db.GetTaskType(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);

			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);
				
			mdl.Delete(db);
            
            CleanCache(db, CacheTags.TaskType, null);

            return Ok();			
		}
	}
}
