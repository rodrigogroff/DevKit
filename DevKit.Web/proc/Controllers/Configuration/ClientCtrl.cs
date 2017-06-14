using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ClientController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new Client();

            var filter = new ClientFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var hshReport = SetupCacheReport(CacheObject.ClientReports);
            if (hshReport[filter.Parameters()] is ClientReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter, true);

            var ret = new ClientReport
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

            var obj = RestoreCache(CacheObject.Client, id) as Client;
            if (obj != null)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);
                
            var mdl = db.GetClient(id);

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);
            BackupCache(mdl);

            if (combo)
                return Ok(mdl.ClearAssociations());
            else
                return Ok(mdl);
		}

		public IHttpActionResult Post(Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

			if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheObject.Client, null);
            StoreCache(CacheObject.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();
            
			if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            StoreCache(CacheObject.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = db.GetClient(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			mdl.Delete(db);

            CleanCache(db, CacheObject.Client, null);

            return Ok();
		}
	}
}
