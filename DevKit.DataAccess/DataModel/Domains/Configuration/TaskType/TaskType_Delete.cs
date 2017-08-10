using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Task where e.fkTaskType == id select e).Any())
			{
				resp = "Este tipo de tarefa já está sendo utilizado em uma tarefa";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			db.Delete(this);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskTypeDelete,
				nuType = EnumAuditType.TaskType,
				fkTarget = this.id
			}.
			Create(db, "", "");
		}
	}
}
