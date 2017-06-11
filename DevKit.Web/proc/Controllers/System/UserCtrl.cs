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
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.User, id);
            if (obj != null)
                return Ok(obj);

            var model = db.GetUser(id);

            if (model != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(model);

                return Ok(model.LoadAssociations(db));
            }

            BackupCache(model);

            return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(User mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

			if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            CleanCache(db, CacheObject.User, null);

            return Ok();			
		}

		public IHttpActionResult Put(long id, User mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();
            
			if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            CleanCache(db, CacheObject.User, id);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetUser(id);

			if (model == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!model.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			model.Delete(db);

            CleanCache(db, CacheObject.User, null);

            return Ok();
		}
	}
}
