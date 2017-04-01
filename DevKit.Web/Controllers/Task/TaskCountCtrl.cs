using DataModel;
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
				var task = new Task();

				var usr = util.GetCurrentUser(db);

				int count_project_tasks = 0, 
					count_user_tasks = 0;

				task.ComposedFilters(db, ref count_project_tasks, new TaskFilter
				{
					complete = false,
					lstProjects = util.GetCurrentUserProjects(db, usr.id)
				});

				task.ComposedFilters(db, ref count_user_tasks, new TaskFilter
				{
					fkUserResponsible = usr.id,
				});
				
				return Ok(new { count_project_tasks = count_project_tasks, count_user_tasks = count_user_tasks });
			}
		}
	}
}
