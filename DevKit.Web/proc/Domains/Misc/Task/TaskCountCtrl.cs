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

            var filter = new TaskFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
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
            
            var proj = new Task().ComposedFilters(db, filter, options );

            filter = new TaskFilter
            {
                fkUserResponsible = db.currentUser.id,
            };
            
            var user = new Task().ComposedFilters(db, filter, options );

            return Ok(new { count_project_tasks = proj.count,
							count_user_tasks = user.count });			
		}
	}
}
