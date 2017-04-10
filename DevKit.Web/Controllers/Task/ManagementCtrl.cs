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
				var mdl = new Management();

				var dto = mdl.ComposedFilters(db, new ManagementFilter()
				{
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				});

				return Ok(dto);				
			}
		}
	}
}
