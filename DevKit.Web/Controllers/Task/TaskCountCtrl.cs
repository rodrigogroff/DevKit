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
				var util = new Util();
				var usr = util.GetCurrentUser(db);
				
				var queryTaskProjects = new Task().ComposedFilters(db, new TaskFilter
				{
					complete = false,
					lstProjects = util.GetCurrentUserProjects(db, usr.id)
				});

				var queryTaskUser = new Task().ComposedFilters(db, new TaskFilter
				{
					fkUserResponsible = usr.id,
				});
								
				return Ok(new
				{
					count_project_tasks = queryTaskProjects.Count(),
					count_user_tasks = queryTaskUser.Count()
				});
			}
		}
	}
}
