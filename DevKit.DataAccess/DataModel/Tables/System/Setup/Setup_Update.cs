using LinqToDB;

namespace DataModel
{
	public partial class Setup
	{
		public bool Update(DevKitDB db, User user, ref string resp)
		{
			db.Update(this);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemSetupUpdate,
				nuType = EnumAuditType.Setup
			}.
			Create(db, "", "");

			return true;
		}
	}
}
