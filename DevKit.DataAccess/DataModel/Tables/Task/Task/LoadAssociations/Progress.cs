using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskProgress> LoadProgress(DevKitDB db)
		{
			var ret = (from e in db.TaskProgresses
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUserAssigned = db.User(item.fkUserAssigned).stLogin;
			}

			return ret;
		}
	}
}
