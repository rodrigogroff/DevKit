using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class VersionController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectSprintVersionFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkSprint = Request.GetQueryStringValue<int?>("fkSprint", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProjectSprintVersionReport);
            if (hshReport[parameters] is ProjectSprintVersionReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new ProjectSprintVersion().ComposedFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProjectSprintVersion, id) is ProjectSprintVersion obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectSprintVersion(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
	}
}
