using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public TaskType LoadAssociations(DevKitDB db)
		{
			project = db.Project(fkProject);

			categories = LoadCategories(db);
			logs = LoadLogs(db);
			checkpoints = LoadCheckpoints(db);

			return this;
		}

		List<TaskCategory> LoadCategories(DevKitDB db)
		{
			var lst = (from e in db.TaskCategories where e.fkTaskType == id select e).
				OrderBy(t => t.stAbreviation).ThenBy ( y=> y.stName).
				ToList();

			return lst;
		}

		List<TaskTypeLog> LoadLogs(DevKitDB db)
		{
			var setup = db.Setup();

			var lstLogs = (from e in db.AuditLogs
						   where e.nuType == EnumAuditType.TaskType
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.Users on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<TaskTypeLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new TaskTypeLog
				{
					sdtLog = item.dtLog?.ToString(setup.stDateFormat),
					stUser = lstUsers.Where(y => y.id == item.fkUser).FirstOrDefault().stLogin,
					stDetails = item.stLog
				});
			}

			return ret;
		}

		List<TaskCheckPoint> LoadCheckpoints(DevKitDB db)
		{
			return (from e in db.TaskCheckPoints
					where e.fkCategory == this.id
					select e).
					OrderByDescending(y => y.id).
					ToList();
		}
	}
}
