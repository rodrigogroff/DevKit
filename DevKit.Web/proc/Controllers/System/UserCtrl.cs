using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class UserController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new UserFilter()
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                email = Request.GetQueryStringValue("email")?.ToUpper(),
                phone = Request.GetQueryStringValue("phone")?.ToUpper(),
                fkPerfil = Request.GetQueryStringValue<long?>("fkPerfil", null),
                ativo = Request.GetQueryStringValue<bool?>("ativo", null),
            };

            var hshReport = SetupCacheReport(CacheTags.UserReports);
            if (hshReport[filter.Parameters()] is UserReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new User();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new UserReport
            {
                count = reportCount,
                results = results
            };

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
        }
        
        public IHttpActionResult Get(long id)
		{
            if (id == 0)
            {
                StartDatabaseAndAuthorize();
                return Ok(db.currentUser);
            }                

            var combo = Request.GetQueryStringValue("combo", false);

            if (RestoreCache(CacheTags.User, id) is User obj)
            {
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);
            }

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetUser(id);

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

		public IHttpActionResult Post(User mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.User, null);            
            StoreCache(CacheTags.User, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, User mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
			if (!mdl.Update(db, ref apiResponse))
				return BadRequest(apiResponse);

            if (mdl.resetPassword != "")
            {
                StoreCache(CacheTags.User, mdl.id, null);
                mdl.ClearAssociations();
                return Ok(mdl);
            }

            mdl.LoadAssociations(db);
            StoreCache(CacheTags.User, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetUser(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

			mdl.Delete(db);

            CleanCache(db, CacheTags.User, null);

            return Ok();
		}
	}
}
