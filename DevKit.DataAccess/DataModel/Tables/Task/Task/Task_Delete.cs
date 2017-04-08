using LinqToDB;

namespace DataModel
{	
	public partial class Task
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return false;
		}

		public void Delete(DevKitDB db, User user)
		{
			db.Delete(this);

			new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.TaskDelete }.Create(db, "", "");
		}
	}
}
