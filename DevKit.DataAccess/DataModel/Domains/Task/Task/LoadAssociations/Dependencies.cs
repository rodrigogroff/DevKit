using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskDependency> LoadDependencies(DevKitDB db)
		{
			var ret = (from e in db.TaskDependency
					   where e.fkMainTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.GetSetup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.GetUser(item.fkUser).stLogin;

				var subTask = db.GetTask(item.fkSubTask);

				item.sfkTaskFlowCurrent = db.GetTaskFlow(subTask.fkTaskFlowCurrent).stName;
				item.stProtocol = subTask.stProtocol;
				item.stTitle = subTask.stTitle;
				item.stLocalization = subTask.stLocalization;
			}

			return ret;
		}
	}
}
