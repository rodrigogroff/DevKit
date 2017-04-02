using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ManagementController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new ManagementFilter()
				{
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				};

				var mdl = new Management();
				var dto = mdl.ComposedFilters(db, filter, new Util().GetCurrentUserProjects(db));

				if (dto.fail)
					return StatusCode(HttpStatusCode.NotFound);

				return Ok(dto);				
			}
		}
	}
}
