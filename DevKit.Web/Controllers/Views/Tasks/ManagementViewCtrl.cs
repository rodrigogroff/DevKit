using DataModel;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ManagementViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
			{
				var mdl = new ManagementView();
				
				return Ok(mdl.ComposedFilters(db, new ManagementViewFilter()
				{
                    fkCurrentUser = login.idUser,
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
				}));				
			}
		}
	}
}
