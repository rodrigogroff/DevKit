using LinqToDB;

namespace DataModel
{
	public partial class Setup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

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
