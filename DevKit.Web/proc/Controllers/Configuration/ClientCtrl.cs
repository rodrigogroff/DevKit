using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ClientController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ClientFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var hshReport = SetupCacheReport(CacheTags.ClientReports);
            if (hshReport[filter.Parameters()] is ClientReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Client();

            var results = mdl.ComposedFilters(db, ref reportCount, filter, bSaveAudit:true);

            var ret = new ClientReport
            {
                count = reportCount,
                results = results
            };

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
        }

		public IHttpActionResult Get(long id)
		{
            var combo = Request.GetQueryStringValue("combo", false);

            if (RestoreCache(CacheTags.Client, id) is Client obj)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

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
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			if (!mdl.Create(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.Client, null);
            StoreCache(CacheTags.Client, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
			if (!mdl.Update(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

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
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

			mdl.Delete(db);

            CleanCache(db, CacheTags.Client, null);

            return Ok();
		}
	}
}
