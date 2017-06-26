using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskFlowController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskFlowFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),
                fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskFlowReport);
            if (hshReport[parameters] is TaskFlowReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new TaskFlow().ComposedFilters(db, filter);
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.TaskFlow, id) is TaskFlow obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTaskFlow(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
	}
}
