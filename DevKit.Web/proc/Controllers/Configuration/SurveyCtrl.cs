using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SurveyController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Survey();

			var results = mdl.ComposedFilters(db, ref count, new SurveyFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetSurvey(id);

            if (mdl != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(mdl);

                return Ok(mdl.LoadAssociations(db));
            }

            return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(Survey mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiResponse))
			    return BadRequest(apiResponse);

			return Ok();			
		}

		public IHttpActionResult Put(long id, Survey mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiResponse))
					return BadRequest(apiResponse);

			return Ok();
		}

		public IHttpActionResult Delete(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetSurvey(id);

			if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);
            
			if (!mdl.CanDelete(db, ref apiResponse))
				return BadRequest(apiResponse);

			mdl.Delete(db);
								
			return Ok();
		}
	}
}
