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

            var results = mdl.ComposedFilters(db, ref count, filter);

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
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.Client, id);
            if (obj != null)
                return Ok(obj);

            var model = db.GetClient(id);

			if (model == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            var combo = Request.GetQueryStringValue("combo", false);
            if (combo)
                return Ok(model);

            BackupCache(model);

            return Ok(model.LoadAssociations(db));
		}

		public IHttpActionResult Post(Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

			if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            CleanCache(db, CacheObject.Client, null);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();
            
			if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            CleanCache(db, CacheObject.Client, id);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetClient(id);

			if (model == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!model.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			model.Delete(db);

            CleanCache(db, CacheObject.Client, null);

            return Ok();
		}
	}
}
