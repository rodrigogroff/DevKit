using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SurveyController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new SurveyFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.SurveyReport);
            if (hshReport[parameters] is SurveyReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Survey();

            var results = mdl.ComposedFilters(db, ref reportCount, filter);

            var ret = new SurveyReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.Survey, id) is Survey obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetSurvey(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);                        
        }

		public IHttpActionResult Post(Survey mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
			    return BadRequest(apiError);

            CleanCache(db, CacheTags.Survey, null);
            
            return Ok();			
		}

		public IHttpActionResult Put(long id, Survey mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
				return BadRequest(apiError);
            
            CleanCache(db, CacheTags.Survey, null);
            StoreCache(CacheTags.Survey, mdl.id, mdl);

            return Ok();
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetSurvey(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (!mdl.CanDelete(db, ref apiError))
                return BadRequest(apiError);

            mdl.Delete(db);

            CleanCache(db, CacheTags.Survey, null);
            
            return Ok();
        }
    }
}
