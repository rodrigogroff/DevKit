using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class NewsController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new CompanyNews();

            var filter = new CompanyNewsFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
            };            

            var hshReport = SetupCacheReport(CacheObject.CompanyNewsReports);
            if (hshReport[filter.Parameters()] is CompanyNewsReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter);

            var ret = new CompanyNewsReport
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

            var obj = RestoreCache(CacheObject.CompanyNews, id);
            if (obj != null)
                return Ok(obj);

            var mdl = db.GetNews(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);
            BackupCache(mdl);

            return Ok(mdl);
		}

		public IHttpActionResult Post(CompanyNews mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
			    return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheObject.CompanyNews, null);
            StoreCache(CacheObject.CompanyNews, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, CompanyNews mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            StoreCache(CacheObject.CompanyNews, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Delete(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetNews(id);

			if (model == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!model.CanDelete(db, ref serviceResponse))
				return BadRequest(serviceResponse);

            model.Delete(db);

            CleanCache(db, CacheObject.CompanyNews, null);

            return Ok();
		}
	}
}
