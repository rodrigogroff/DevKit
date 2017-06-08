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
            var login = GetLoginFromRequest();

            if (login == null)
                return BadRequest();

            using (var db = new DevKitDB())
			{
                if (!db.ValidateUser(login.idUser))
                    return BadRequest();

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
