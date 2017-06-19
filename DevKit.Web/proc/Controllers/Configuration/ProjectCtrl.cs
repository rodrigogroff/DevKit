using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProjectController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProjectReports);
            if (hshReport[parameters] is ProjectReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Project();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new ProjectReport
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

            var obj = RestoreCache(CacheTags.Project, id) as Project;
            if (obj != null)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            BackupCache(mdl);

            if (combo)
                return Ok(mdl.ClearAssociations());
            else
                return Ok(mdl);
        }

		public IHttpActionResult Post(Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiResponse))
				return BadRequest(apiResponse);

            CleanCache(db, CacheTags.ProjectReports, null);
            StoreCache(CacheTags.Project, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
                return BadRequest(apiResponse);

            switch (mdl.updateCommand)
            {
                case "newPhase":
                case "removePhase":
                    CleanCache(db, CacheTags.ProjectPhaseReports, null);
                    break;
            }

            CleanCache(db, CacheTags.ProjectReports, null);
            StoreCache(CacheTags.Project, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

			mdl.Delete(db);

            CleanCache(db, CacheTags.ProjectReports, null);
            CleanCache(db, CacheTags.ProjectPhaseReports, null);

            return Ok();
		}
	}
}
