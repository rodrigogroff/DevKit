using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCategoryController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new TaskCategory();

			var results = mdl.ComposedFilters(db, ref count, new TaskCategoryFilter()
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null)
			});

			return Ok(new { count = count, results = results });			
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetTaskCategory(id);

            if (model != null)
                return Ok(model);

            return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
