using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCountController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
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
            
            task.ComposedFilters(db, ref count_project_tasks, filter, options );

            filter = new TaskFilter
            {
                fkUserResponsible = db.currentUser.id,
            };
            
            task.ComposedFilters(db, ref count_user_tasks, filter, options );
				
			return Ok(new { count_project_tasks = count_project_tasks,
							count_user_tasks = count_user_tasks });			
		}
	}
}
