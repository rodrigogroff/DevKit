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
				resp = "DBA user cannot be removed";
				return false;
			}

			if ( (from e in db.ProjectUser where e.fkUser == id select e).Count() > 0)
			{
				resp = "this user is allocated in a project and cannot be removed";
				return false;
			}

			if ((from e in db.Task where e.fkUserStart == id select e).Count() > 0)
			{
				resp = "this user is allocated in a task and cannot be removed";
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
