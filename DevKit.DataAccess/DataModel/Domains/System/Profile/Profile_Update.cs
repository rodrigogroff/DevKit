using LinqToDB;

namespace DataModel
{
	public partial class Profile
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemProfileUpdate,
				nuType = EnumAuditType.Profile,
				fkTarget = this.id				
			}.
			Create(db, TrackChanges(db), "");

			db.Update(this);

			logs = LoadLogs(db);

			return true;
		}
	}
}
