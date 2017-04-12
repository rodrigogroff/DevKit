using LinqToDB;

namespace DataModel
{
	public partial class Setup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.GetCurrentUser();

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
