using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PhaseController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectPhaseFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<int?>("fkProject", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProjectPhaseReport);
            if (hshReport[parameters] is ProjectPhaseReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new ProjectPhase().ComposedFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProjectPhase, id) is ProjectPhase obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectPhase(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            BackupCache(mdl);
            
            return Ok(mdl);
        }
	}
}
