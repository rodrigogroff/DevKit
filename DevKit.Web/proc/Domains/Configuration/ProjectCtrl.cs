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

            var hshReport = SetupCacheReport(CacheTags.ProjectReport);
            if (hshReport[parameters] is ProjectReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Project().ComposedFilters(db, filter);
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Project, id) is Project obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
        }

		public IHttpActionResult Post(Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Project, null);
            StoreCache(CacheTags.Project, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, Project mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            switch (mdl.updateCommand)
            {
                case "newPhase":
                case "removePhase":
                    CleanCache(db, CacheTags.ProjectPhase, null);
                    break;

                case "newSprint":
                case "removeSprint":
                    CleanCache(db, CacheTags.ProjectSprint, null);
                    break;
            }

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Project, null);
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
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

			mdl.Delete(db);

            CleanCache(db, CacheTags.Project, null);
            CleanCache(db, CacheTags.ProjectPhase, null);

            return Ok();
		}
	}
}
