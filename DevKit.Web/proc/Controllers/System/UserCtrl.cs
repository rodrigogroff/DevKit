using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class UserController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

			var mdl = new User();

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

            var hshReport = SetupCacheReport(CacheObject.UserReports);
            if (hshReport[filter.Parameters()] is UserReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter);

            var ret = new UserReport
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

            var obj = RestoreCache(CacheObject.User, id) as User;
            if (obj != null)
            {
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);
            }

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
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

			if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheObject.User, null);            
            StoreCache(CacheObject.User, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, User mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();
            
			if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            if (mdl.resetPassword != "")
            {
                StoreCache(CacheObject.User, mdl.id, null);
                mdl.ClearAssociations();
                return Ok(mdl);
            }

            mdl.LoadAssociations(db);
            StoreCache(CacheObject.User, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = db.GetUser(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			mdl.Delete(db);

            CleanCache(db, CacheObject.User, null);

            return Ok();
		}
	}
}
