using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if (stLogin.ToUpper() == "DBA")
			{
				resp = "Usuário DBA não pode ser removido";
				return false;
			}

			if ( (from e in db.ProjectUser where e.fkUser == id select e).Count() > 0)
			{
				resp = "Este usuário está alocado em um projeto e não pode ser removido";
				return false;
			}

			if ((from e in db.Task where e.fkUserStart == id select e).Count() > 0)
			{
				resp = "Este usuário está alocado em tarefas e não pode ser removido";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			foreach (var item in (from e in db.UserPhone where e.fkUser == id select e))
				db.Delete(item);

			foreach (var item in (from e in db.UserEmail where e.fkUser == id select e))
				db.Delete(item);

			db.Delete(this);

			new AuditLog
			{
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemUserDelete,
				nuType = EnumAuditType.User,
				fkTarget = this.id
			}.
			Create(db, "", "");
		}
	}
}
