using DataModel;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCountController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var usr = new Util().GetCurrentUser(db);

				var filter = new TaskFilter()
				{
					fkUserResponsible = usr.id,	
				};

				var queryTaskUser = new Task().ComposedFilters(db, filter);
				
				return Ok(new
				{
					count_user_tasks = queryTaskUser.Count()
				});
			}
		}
	}
}
