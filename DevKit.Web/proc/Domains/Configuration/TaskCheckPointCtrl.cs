using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCheckPointController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new TaskCheckPointFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkCategory = Request.GetQueryStringValue<long?>("fkCategory", null)
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskCheckPointReport);
            if (hshReport[parameters] is TaskCheckPointReport report)
                return Ok(report);
            
            var ret = new TaskCheckPoint().ComposedFilters ( db, filter );
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.TaskCheckPoint, id) is TaskCheckPoint obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetTaskCheckPoint(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            BackupCache(mdl);

            return Ok(mdl);
        }
	}
}
