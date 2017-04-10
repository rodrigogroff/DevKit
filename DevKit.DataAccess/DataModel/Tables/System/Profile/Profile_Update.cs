using LinqToDB;

namespace DataModel
{
	public partial class Profile
	{
		public bool Update(DevKitDB db, User user, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "The name '" + stName + "' is already taken";
				return false;
			}

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
