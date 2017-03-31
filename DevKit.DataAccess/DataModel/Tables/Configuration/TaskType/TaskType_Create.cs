using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{ 
	public partial class TaskType
	{
		bool CheckDuplicate(TaskType item, DevKitDB db)
		{
			var query = from e in db.TaskTypes select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
