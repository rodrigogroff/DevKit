using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCheckPointController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var count = 0; var mdl = new TaskCheckPoint();

            var results = mdl.ComposedFilters(db, ref count, new TaskCheckPointFilter()
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkCurrentUser = login.idUser,
				fkCategory = Request.GetQueryStringValue<long?>("fkCategory", null)
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetTaskCheckPoint(id);

			if (model != null)
				return Ok(model);

			return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
