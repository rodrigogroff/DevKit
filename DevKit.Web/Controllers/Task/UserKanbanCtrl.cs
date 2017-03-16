using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class UserKanbanController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new UserKanbanFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new UserKanban();

				return Ok(mdl.ComposedFilters(db, filter, new Util().GetCurrentUser(db)));
			}
		}
	}
}
