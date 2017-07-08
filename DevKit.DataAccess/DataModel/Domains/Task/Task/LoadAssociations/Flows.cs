using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskFlowChange> LoadFlows(DevKitDB db)
		{
			var ret = (from e in db.TaskFlowChange
					   where e.fkTask == id
					   select e).
						OrderByDescending(t => t.dtLog).
						ToList();

			var setup = db.GetSetup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.GetUser(item.fkUser).stLogin;

				if (item.fkOldFlowState != null)
					item.sfkOldFlowState = db.GetTaskFlow(item.fkOldFlowState).stName;

				if (item.fkNewFlowState != null)
					item.sfkNewFlowState = db.GetTaskFlow(item.fkNewFlowState).stName;
			}

			return ret;
		}
	}
}
