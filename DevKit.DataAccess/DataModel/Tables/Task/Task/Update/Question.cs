using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_newQuestion(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskQuestion>(anexedEntity.ToString());

			if (ent.id == 0)
			{
				ent.fkTask = this.id;
				ent.bFinal = false;
				ent.dtOpen = DateTime.Now;
				ent.fkUserOpen = user.id;

				db.Insert(ent);

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.TaskUpdateAddQuestion,
					nuType = EnumAuditType.Task,
					fkTarget = this.id
				}.
				Create(db, "New question added", "");
			}
			else
			{
				if (ent.bFinal == true)
					if (ent.dtClosed == null)
						ent.dtClosed = DateTime.Now;

				ent.fkUserDirected = user.id;

				db.Update(ent);

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.TaskUpdateEditQuestion,
					nuType = EnumAuditType.Task,
					fkTarget = this.id
				}.
				Create(db, "Edit question", "");
			}
            
			return true;
		}

		public bool Update_removeQuestion(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskQuestion>(anexedEntity.ToString());

			var entDb = (from e in db.TaskQuestion
						 where e.id == ent.id
						 select e).
						 FirstOrDefault();

			new AuditLog
			{
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskUpdateRemoveQuestion,
				nuType = EnumAuditType.Task,
				fkTarget = this.id
			}.
			Create(db, "Question removed", "");

			db.Delete(entDb);

			return true;
		}
	}
}
