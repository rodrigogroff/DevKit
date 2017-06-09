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

			var results = mdl.ComposedFilters(db, ref count, new ClientFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetClient(id);

			if (model != null)
            {
                var combo = Request.GetQueryStringValue("combo", false);

                if (combo)
                    return Ok(model);

                return Ok(model.LoadAssociations(db));
            }

			return StatusCode(HttpStatusCode.NotFound);			
		}

		public IHttpActionResult Post(Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

			if (!mdl.Create(db, ref serviceResponse))
				return BadRequest(serviceResponse);

			return Ok();			
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();
            
			if (!mdl.Update(db, ref serviceResponse))
				return BadRequest(serviceResponse);

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
								
			return Ok();
		}
	}
}
