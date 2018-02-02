using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class NewsController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new CompanyNewsFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.CompanyNewsReport);
            if (hshReport[parameters] is CompanyNewsReport report)
                return Ok(report);

            var ret = new CompanyNews().ComposedFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.CompanyNews, id) is CompanyNews obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetCompanyNews(id);

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

            if (!mdl.Create(db, ref apiError))
			    return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.CompanyNews, null);
            StoreCache(CacheTags.CompanyNews, mdl.id, mdl);

            return Ok();			
		}

		public IHttpActionResult Put(long id, CompanyNews mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);

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
            
			if (!mdl.CanDelete(db, ref apiError))
				return BadRequest(apiError);

            mdl.Delete(db);

            CleanCache(db, CacheTags.CompanyNews, null);

            return Ok();
		}
	}
}
