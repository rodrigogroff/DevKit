using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskDependency> LoadDependencies(DevKitDB db)
		{
			var ret = (from e in db.TaskDependencies
					   where e.fkMainTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				var subTask = db.Task(item.fkSubTask);

				item.sfkTaskFlowCurrent = db.TaskFlow(subTask.fkTaskFlowCurrent).stName;
				item.stProtocol = subTask.stProtocol;
				item.stTitle = subTask.stTitle;
				item.stLocalization = subTask.stLocalization;
			}

			return ret;
		}

	}
}
