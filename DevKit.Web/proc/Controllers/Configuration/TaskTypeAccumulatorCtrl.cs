using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskTypeAccumulatorController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskTypeAccumulatorFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null)
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.SurveyReport);
            if (hshReport[parameters] is SurveyReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new TaskTypeAccumulator();
            
            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new TaskTypeAccumulatorReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.TaskTypeAccumulator, id) is TaskTypeAccumulator obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTaskTypeAccumulator(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
        }
	}
}
