/*
using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Client
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{			
			if ((from e in db.Task where e.fkProject == id select e).Any())
			{
				resp = "Este projeto está sendo usado em tarefas";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ClientDelete,
				nuType = EnumAuditType.Client
			}.
			Create(db, "Nome: " + this.stName, "");

			db.Delete(this);
		}
	}
}
*/