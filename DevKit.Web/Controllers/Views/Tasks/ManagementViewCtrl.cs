using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ManagementViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var mdl = new ManagementView();
				
				return Ok(mdl.ComposedFilters(db, new ManagementViewFilter()
				{
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				}));				
			}
		}
	}
}
