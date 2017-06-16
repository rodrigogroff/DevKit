using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ManagementViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ManagementView();
				
			return Ok(mdl.ComposedFilters(db, new ManagementViewFilter
			{
				fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
			}));							
		}
	}
}
