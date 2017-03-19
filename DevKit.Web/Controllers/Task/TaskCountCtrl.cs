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
				var usr_id = new Util().GetCurrentUser(db).id;

				var queryTaskProjects = new Task().ComposedFilters(db, new TaskFilter
				{
					complete = false,
					lstProjects = new ProjectUser().ComposedFilters(db, new ProjectUserFilter { fkUser = usr_id }).Select(y => y.fkProject).ToList()
				});

				var queryTaskUser = new Task().ComposedFilters(db, new TaskFilter
				{
					fkUserResponsible = usr_id,
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
