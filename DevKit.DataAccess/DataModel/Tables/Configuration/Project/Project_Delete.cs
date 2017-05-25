using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Task where e.fkProject == id select e).Any())
			{
				resp = "This project is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.GetCurrentUser();

			foreach (var item in (from e in db.ProjectSprint where e.fkProject == id select e))
				db.Delete(item);
				
			foreach (var item in (from e in db.ProjectPhase where e.fkProject == id select e))
				db.Delete(item);

			foreach (var item in (from e in db.ProjectUser where e.fkProject == id select e))
				db.Delete(item);

			db.Delete(this);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ProjectDelete,
				nuType = EnumAuditType.Project
			}.
			Create(db, "", "");
		}
	}
}
