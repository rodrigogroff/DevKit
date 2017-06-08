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

            var count = 0; var mdl = new CompanyNews();

			var results = mdl.ComposedFilters(db, ref count, new CompanyNewsFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkCurrentUser = login.idUser,
				fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetNews(id);

            if (model != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(model);

                return Ok(model.LoadAssociations(db));
            }

            return StatusCode(HttpStatusCode.NotFound);
		}

		public IHttpActionResult Post(CompanyNews mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, mdl.login.idUser, ref serviceResponse))
					return BadRequest(serviceResponse);

			return Ok(mdl);			
		}

		public IHttpActionResult Put(long id, CompanyNews mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, mdl.login.idUser, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok(mdl);			
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

            model.Delete(db, login.idUser);
								
			return Ok();
		}
	}
}
