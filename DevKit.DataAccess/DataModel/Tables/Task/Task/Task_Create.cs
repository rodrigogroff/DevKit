using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		bool CheckDuplicate(Task item, DevKitDB db)
		{
			var query = from e in db.Tasks select e;

			if (item.stTitle != null)
			{
				var _st = item.stTitle.ToUpper();
				query = from e in query where e.stTitle.ToUpper().Contains(_st) select e;
			}

			if (item.fkProject != null)
				query = from e in query where e.fkProject == item.fkProject select e;

			if (item.fkSprint != null)
				query = from e in query where e.fkSprint == item.fkSprint select e;

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		int RandomNumber(int min, int max)
		{
			return new Random().Next(min, max);
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			bComplete = false;
			dtStart = DateTime.Now;
			fkUserStart = usr.id;
			fkTaskFlowCurrent = (from e in db.TaskFlows
								 where e.fkTaskType == this.fkTaskType
								 where e.fkTaskCategory == this.fkTaskCategory
								 select e).
								 OrderBy(t => t.nuOrder).
								 FirstOrDefault().
								 id;
			
			var setup = db.Setup();

			if (setup.stProtocolFormat != null)
				foreach (var i in setup.stProtocolFormat)
					if (Char.IsDigit(i))
						stProtocol += RandomNumber(0, 9).ToString();
					else
						stProtocol += i;

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
