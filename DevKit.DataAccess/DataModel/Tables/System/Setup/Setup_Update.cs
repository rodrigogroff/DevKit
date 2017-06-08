using LinqToDB;

namespace DataModel
{
	public partial class Setup
	{
		public bool Update(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemSetupUpdate,
				nuType = EnumAuditType.Setup
			}.
			Create(db, TrackChanges(db), "");

			db.Update(this);

			logs = LoadLogs(db);

			return true;
		}
	}
}
