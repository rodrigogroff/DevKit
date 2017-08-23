using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ClientController : ApiControllerBase
	{
        /*
		public IHttpActionResult Get()
		{
            var filter = new ClientFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ClientReport);
            if (hshReport[parameters] is ClientReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var ret = new Client().ComposedFilters(db, filter, bSaveAudit: true);
             
            hshReport[parameters] = ret;

            return Ok(ret);
        }

		public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Client, id) is Client obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClient(id);

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(Client mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            CleanCache(db, CacheTags.Client, null);
            StoreCache(CacheTags.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
			if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Client, null);
            StoreCache(CacheTags.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClient(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

			mdl.Delete(db);

            CleanCache(db, CacheTags.Client, null);

            return Ok();
		}
        */
	}
}
