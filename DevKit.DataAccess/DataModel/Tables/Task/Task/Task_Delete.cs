using LinqToDB;

namespace DataModel
{	
	public partial class Task
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return false;
		}

		public void Delete(DevKitDB db, long fkCurrentUser)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.TaskDelete,
				nuType = EnumAuditType.Task,
				fkTarget = this.id
			}.
			Create(db, "", "");

			db.Delete(this);
		}
	}
}
