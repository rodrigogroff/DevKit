using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		List<TaskCheckPoint> LoadCheckpoints(DevKitDB db)
		{
			var setup = db.Setup();

			var lst = (from e in db.TaskCheckPoints
					   where e.fkCategory == this.fkTaskCategory
					   select e).
					   ToList();

			foreach (var item in lst)
			{
				var mark = (from e in db.TaskCheckPointMarks
							where e.fkCheckPoint == item.id
							where e.fkTask == this.id
							select e).
							FirstOrDefault();

				if (mark != null)
				{
					item.bSelected = true;
					item.sdtLog = mark.dtLog?.ToString(setup.stDateFormat);
					item.sfkUser = db.User(mark.fkUser).stLogin;
				}
				else
					item.bSelected = false;
			}

			return lst;
		}
	}
}
