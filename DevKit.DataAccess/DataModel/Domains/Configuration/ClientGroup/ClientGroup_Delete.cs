using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ClientGroup
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.TaskClientGroup where e.fkClientGroup == id select e).Any())
			{
				resp = "Este grupo é usado em alguma tarefa";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ClientGroupDelete,
				nuType = EnumAuditType.ClientGroup
			}.
			Create(db, "Nome: " + this.stName, "");

			db.Delete(this);
		}
	}
}
