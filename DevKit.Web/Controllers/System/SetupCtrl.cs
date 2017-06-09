using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SetupController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetSetup();
				
			if (model != null)
				return Ok(model.LoadAssociations(db));

			return StatusCode(HttpStatusCode.NotFound);			
		}

		public IHttpActionResult Put(long id, Setup mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
			    return BadRequest(serviceResponse);

			return Ok();			
		}
	}
}
