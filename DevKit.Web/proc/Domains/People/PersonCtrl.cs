using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PersonController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new UserFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                email = Request.GetQueryStringValue("email")?.ToUpper(),
                phone = Request.GetQueryStringValue("phone")?.ToUpper(),
                fkPerfil = Request.GetQueryStringValue<long?>("fkPerfil", null),
                ativo = Request.GetQueryStringValue<bool?>("ativo", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.UserReport);
            if (hshReport[parameters] is UserReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new User().ComposedFilters(db, filter);
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }
        
        public IHttpActionResult Get(long id)
		{
            if (id == OperationTags.GET_CURRENT_USER)
            {
                StartDatabaseAndAuthorize();
                return Ok(db.currentUser);
            }                

            if (RestoreCache(CacheTags.User, id) is User obj)
                return Ok(obj);
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetUser(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(User mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.User, null);
            CleanCache(db, CacheTags.Profile, null);
            CleanCache(db, CacheTags.Project, null);

            StoreCache(CacheTags.User, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, User mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            bool bProfileChanged = false;

			if (!mdl.Update(db, ref apiError,ref bProfileChanged))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.User, null);
            CleanCache(db, CacheTags.Project, null);

            if (bProfileChanged)
                CleanCache(db, CacheTags.Profile, null);

            StoreCache(CacheTags.User, mdl.id, mdl);

            if (mdl.resetPassword != "")
                return Ok(mdl);
            else
                return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetUser(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

			mdl.Delete(db);

            CleanCache(db, CacheTags.User, null);

            return Ok();
		}
	}
}
