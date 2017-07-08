using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskQuestion> LoadQuestions(DevKitDB db)
		{
			var ret = (from e in db.TaskQuestion
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.id).
					   ToList();

			var setup = db.GetSetup();

			foreach (var item in ret)
			{
				item.sfkUserOpen = db.GetUser(item.fkUserOpen).stLogin;
				item.sfkUserDirected = db.GetUser(item.fkUserDirected).stLogin;
				item.sdtOpen = item.dtOpen?.ToString(setup.stDateFormat);
				item.sdtClosed = item.dtClosed?.ToString(setup.stDateFormat);
			}

			return ret;
		}
	}
}
