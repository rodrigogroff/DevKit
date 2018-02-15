using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProfileController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new ProfileFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                stPermission = Request.GetQueryStringValue("stPermission")?.ToUpper(),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProfileReport);
            if (hshReport[parameters] is ProfileReport report)
                return Ok(report);

            var ret = new Profile().ComposedFilters(db, filter);
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            //if (RestoreCache(CacheTags.Profile, id) is Profile obj)
              //  return Ok(obj);
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProfile(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            //BackupCache(mdl);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Profile mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Profile, null);
            StoreCache(CacheTags.Profile, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Put(long id, Profile mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);
            
            mdl.LoadAssociations(db);
            
            CleanCache(db, CacheTags.User, null);
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
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

            CleanCache(db, CacheTags.Profile, null);

            mdl.Delete(db);
				
			return Ok();
		}
	}
}
