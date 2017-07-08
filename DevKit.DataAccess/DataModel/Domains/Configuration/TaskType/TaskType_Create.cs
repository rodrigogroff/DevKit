using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{ 
	public partial class TaskType
	{
		bool CheckDuplicate(TaskType item, DevKitDB db)
		{
			var query = from e in db.TaskType select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskTypeAdd,
				nuType = EnumAuditType.TaskType,
				fkTarget = this.id
			}.
			Create(db, "", "");

			return true;
		}
	}
}
