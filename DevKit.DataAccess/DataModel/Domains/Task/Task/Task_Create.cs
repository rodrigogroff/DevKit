using LinqToDB;
using System;
using System.Linq;
using System.Threading;

namespace DataModel
{
	public partial class Task
	{
		bool CheckDuplicate(Task item, DevKitDB db)
		{
			var query = from e in db.Task select e;

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
		
		public bool Create(DevKitDB db, ref string resp)
		{
			var setup = db.GetSetup();
			var user = db.currentUser;
			var category = db.GetTaskCategory(this.fkTaskCategory);

			bComplete = false;
			dtStart = DateTime.Now;
			fkUserStart = user.id;
			fkTaskFlowCurrent = (from e in db.TaskFlow
								 where e.fkTaskType == this.fkTaskType
								 where e.fkTaskCategory == this.fkTaskCategory
								 select e).
								 OrderBy(t => t.nuOrder).
								 FirstOrDefault().
								 id;

			if (category.bExpires == true)
			{
				this.dtExpired = DateTime.Now;

				if (category.nuExpiresDays > 0)
					this.dtExpired = dtExpired.Value.AddDays((int)category.nuExpiresDays);

				if (category.nuExpiresHours > 0)
					this.dtExpired = dtExpired.Value.AddHours((int)category.nuExpiresHours);

				if (category.nuExpiresMinutes > 0)
					this.dtExpired = dtExpired.Value.AddMinutes((int)category.nuExpiresMinutes);
			}

			stProtocol = setup.GetProtocol();	

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskCreate,
				nuType = EnumAuditType.Task,
				fkTarget = this.id
			}.
			Create(db, "", "");

			return true;
		}
	}
}
