using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCountController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var task = new Task();
            
			int count_project_tasks = 0, 
				count_user_tasks = 0;

            var filter = new TaskFilter
            {
                complete = false,
                kpa = false,
                lstProjects = db.GetCurrentUserProjects()
            };

            var options = new loaderOptionsTask
            {
                bLoadTaskCategory = true,
                bLoadTaskType = true,
                bLoadProject = true,
                bLoadPhase = true,
                bLoadSprint = true,
                bLoadTaskFlow = true,
                bLoadVersion = true,
                bLoadUsers = true,
            };
            
            var r1 = task.ComposedFilters(db, filter, options );
            count_project_tasks = r1.count;

            filter = new TaskFilter
            {
                fkUserResponsible = db.currentUser.id,
            };
            
            var r2 = task.ComposedFilters(db, filter, options );
            count_user_tasks = r2.count;

            return Ok(new { count_project_tasks = count_project_tasks,
							count_user_tasks = count_user_tasks });			
		}
	}
}
