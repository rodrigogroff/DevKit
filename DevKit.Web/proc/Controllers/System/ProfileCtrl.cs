using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProfileController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProfileFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                stPermission = Request.GetQueryStringValue("stPermission")?.ToUpper(),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProfileReports);
            if (hshReport[parameters] is ProfileReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Profile();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new ProfileReport
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
            
            if (RestoreCache(CacheTags.Profile, id) is Profile obj)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProfile(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            BackupCache(mdl);

            if (combo)
                return Ok(mdl.ClearAssociations());
            else
                return Ok(mdl);
        }

        public IHttpActionResult Post(Profile mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Profile, null);
            StoreCache(CacheTags.Profile, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, Profile mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Profile, null);
            StoreCache(CacheTags.Profile, mdl.id, mdl);

            return Ok();		
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProfile(id);
				
			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

            CleanCache(db, CacheTags.Profile, null);

            mdl.Delete(db);
				
			return Ok();
		}
	}
}
