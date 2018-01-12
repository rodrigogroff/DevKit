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
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
                managed = Request.GetQueryStringValue<bool?>("managed", null),
                condensed = Request.GetQueryStringValue<bool?>("condensed", null),
                kpa = Request.GetQueryStringValue<bool?>("kpa", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskTypeReport);
            if (hshReport[parameters] is TaskTypeReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new TaskType().ComposedFilters(db, filter, new loaderOptionsTaskType
            {
                bLoadProject = true,
                bLoadCategories = true
            });
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.TaskType, id) is Client obj)
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
                    CleanCache(db, CacheTags.TaskCategory, null);
                    break;

                case "newFlow": 
                case "removeFlow":
                    CleanCache(db, CacheTags.TaskFlowCombo, null);
                    break;

                case "newAcc": 
                case "removeAcc":
                    CleanCache(db, CacheTags.TaskTypeAccumulator, null);
                    break;

                case "newCC": 
                case "removeCC":
                    CleanCache(db, CacheTags.TaskCheckPoint, null);
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
