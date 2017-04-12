using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class CompanyNews
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			/*
			if ((from e in db.Tasks where e.fkProject == id select e).Any())
			{
				resp = "This project is being used in a task";
				return false;
			}
			*/

			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.GetCurrentUser();

			db.Delete(this);

			/*
			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ProjectDelete,
				nuType = EnumAuditType.Project
			}.
			Create(db, "", "");
			*/
		}
	}
}
