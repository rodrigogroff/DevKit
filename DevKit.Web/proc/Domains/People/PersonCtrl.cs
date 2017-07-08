using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PersonController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new PersonFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                email = Request.GetQueryStringValue("email")?.ToUpper(),
                phone = Request.GetQueryStringValue("phone")?.ToUpper(),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.PersonReport);
            if (hshReport[parameters] is PersonReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Person().ComposedFilters(db, filter);
            
            hshReport[parameters] = ret;

            return Ok(ret);
        }
        
        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Person, id) is Person obj)
                return Ok(obj);
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetPerson(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(Person mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Person, null);
            StoreCache(CacheTags.Person, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Person mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);
                        
            CleanCache(db, CacheTags.Person, null);
            StoreCache(CacheTags.Person, mdl.id, mdl);

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

            CleanCache(db, CacheTags.Person, null);

            return Ok();
		}
	}
}
