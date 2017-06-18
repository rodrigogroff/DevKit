using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class NewsController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new CompanyNewsFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.CompanyNewsReports);
            if (hshReport[parameters] is CompanyNewsReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new CompanyNews();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new CompanyNewsReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            var obj = RestoreCache(CacheTags.CompanyNews, id);
            if (obj != null)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetNews(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);
            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(CompanyNews mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiResponse))
			    return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.CompanyNews, null);
            StoreCache(CacheTags.CompanyNews, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, CompanyNews mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.CompanyNews, null);
            StoreCache(CacheTags.CompanyNews, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetNews(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

            mdl.Delete(db);

            CleanCache(db, CacheTags.CompanyNews, null);

            return Ok();
		}
	}
}
