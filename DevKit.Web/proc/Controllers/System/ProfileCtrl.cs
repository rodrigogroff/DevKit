using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProfileController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new Profile();

            var filter = new ProfileFilter()
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                stPermission = Request.GetQueryStringValue("stPermission")?.ToUpper(),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            };

            var hshReport = SetupCacheReport(CacheObject.ProfileReports);
            if (hshReport[filter.Parameters()] is ProfileReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter);

            var ret = new ProfileReport
            {
                count = count,
                results = results
            };

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            var combo = Request.GetQueryStringValue("combo", false);

            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.Profile, id) as Profile;
            if (obj != null)
            {
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);
            }

            var mdl = db.GetProfile(id);

            if (mdl != null)
            {
                mdl.LoadAssociations(db);
                BackupCache(mdl);

                if (combo)
                    return Ok(mdl.ClearAssociations());
                else
                    return Ok(mdl);
            }

            return StatusCode(HttpStatusCode.NotFound);
        }

        public IHttpActionResult Post(Profile mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheObject.Profile, null);
            StoreCache(CacheObject.Profile, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, Profile mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            StoreCache(CacheObject.Profile, mdl.id, mdl);

            return Ok();		
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = db.GetProfile(id);
				
			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            CleanCache(db, CacheObject.Profile, null);

            mdl.Delete(db);
				
			return Ok();
		}
	}
}
