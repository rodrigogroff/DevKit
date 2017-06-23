using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SprintController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectSprintFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
                fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProjectSprintReport);
            if (hshReport[parameters] is ProjectSprintReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ProjectSprint();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new ProjectSprintReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProjectSprint, id) is ProjectSprint obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProjectSprint(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
        }

		public IHttpActionResult Put(long id, ProjectSprint mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            CleanCache(db, CacheTags.ProjectSprint, null);
            StoreCache(CacheTags.ProjectSprint, mdl.id, mdl);

            return Ok();			
		}
	}
}
