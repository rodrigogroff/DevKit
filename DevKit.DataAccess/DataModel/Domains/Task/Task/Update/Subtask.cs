using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_newSubtask(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskDependency>(anexedEntity.ToString());

			var subTask = (from e in db.Task
							where e.stProtocol == ent.stProtocol
							select e).
							FirstOrDefault();

			if (subTask == null)
			{
				resp = "Protocol not found";
				return false;
			}

			db.Insert(new TaskDependency
			{
				dtLog = DateTime.Now,
				fkMainTask = this.id,
				fkUser = user.id,
				fkSubTask = subTask.id,
			});

			new AuditLog
			{
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskUpdateAccAddDependency,
				nuType = EnumAuditType.Task,
				fkTarget = this.id
			}.
			Create(db, "New dependency added:" + ent.stProtocol, "");

			return true;
		}

		public bool Update_removeSubtask(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskDependency>(anexedEntity.ToString());

			var entDb = (from e in db.TaskDependency
							where e.id == ent.id
							select e).
							FirstOrDefault();

			new AuditLog
			{
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskUpdateAccRemoveDependency,
				nuType = EnumAuditType.Task,
				fkTarget = this.id
			}.
			Create(db, "Dependency removed: " + entDb.stProtocol, "");

			db.Delete(entDb);

			return true;
		}
	}
}
