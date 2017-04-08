﻿using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCountController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var task = new Task();
				var usr = db.GetCurrentUser();

				int count_project_tasks = 0, 
					count_user_tasks = 0;

				task.ComposedFilters(db, ref count_project_tasks, new TaskFilter
				{
					complete = false,
					kpa = false,
					lstProjects = db.GetCurrentUserProjects(usr.id)
				});

				task.ComposedFilters(db, ref count_user_tasks, new TaskFilter
				{
					fkUserResponsible = usr.id,
				});
				
				return Ok(new { count_project_tasks = count_project_tasks,
								count_user_tasks = count_user_tasks });
			}
		}
	}
}
