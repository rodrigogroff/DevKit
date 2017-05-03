using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskMessage> LoadMessages(DevKitDB db)
		{
			var ret = (from e in db.TaskMessages
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				if (item.fkCurrentFlow != null)
					item.sfkFlow = db.TaskFlow(item.fkCurrentFlow).stName;
			}

			return ret;
		}
	}
}
