using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public TaskType LoadAssociations(DevKitDB db, loaderOptionsTaskType options)
		{
            if (options.bLoadProject)
			    project = db.GetProject(fkProject);

            if (options.bLoadCategories)
			    categories = LoadCategories(db);

            if (options.bLoadCheckPoints)
			    checkpoints = LoadCheckpoints(db);

            if (options.bLoadLogs)
                logs = LoadLogs(db);

            return this;
		}

        public TaskType ClearAssociations()
        {
            project = null;
            categories = null;
            checkpoints = null;
            logs = null;

            return this;
        }

        List<TaskCategory> LoadCategories(DevKitDB db)
		{
			return db.GetListTaskCategory(this.id).
                      OrderBy(t => t.stAbreviation).
                      ThenBy(y => y.stName).
                      ToList();
		}

		List<TaskTypeLog> LoadLogs(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lstLogs = (from e in db.AuditLog
						   where e.nuType == EnumAuditType.TaskType
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.User on e.fkUser equals eUser.id
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
			return (from e in db.TaskCheckPoint
					where e.fkCategory == this.id
					select e).
					OrderByDescending(y => y.id).
					ToList();
		}
	}
}
