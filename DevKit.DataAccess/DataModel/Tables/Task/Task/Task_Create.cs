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
		
		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.GetCurrentUser();

			bComplete = false;
			dtStart = DateTime.Now;
			fkUserStart = user.id;
			fkTaskFlowCurrent = (from e in db.TaskFlows
								 where e.fkTaskType == this.fkTaskType
								 where e.fkTaskCategory == this.fkTaskCategory
								 select e).
								 OrderBy(t => t.nuOrder).
								 FirstOrDefault().
								 id;
			
			var setup = db.Setup();

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
