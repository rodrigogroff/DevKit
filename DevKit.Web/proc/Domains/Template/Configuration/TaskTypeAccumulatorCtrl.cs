using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskTypeAccumulatorController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new TaskTypeAccumulatorFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null)
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskTypeAccumulatorReport );
            if (hshReport[parameters] is TaskTypeAccumulatorReport report)
                return Ok(report);

            var ret = new TaskTypeAccumulator().ComposedFilters(db, filter);
            
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
