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
																
				return Ok(new
				{
					count_project_tasks = new Task().ComposedFilters(db, new TaskFilter
					{
						complete = false,
						lstProjects = util.GetCurrentUserProjects(db, usr.id)
					}).
					Count(),

					count_user_tasks = new Task().ComposedFilters(db, new TaskFilter
					{
						fkUserResponsible = usr.id,
					}).
					Count()
				});
			}
		}
	}
}
