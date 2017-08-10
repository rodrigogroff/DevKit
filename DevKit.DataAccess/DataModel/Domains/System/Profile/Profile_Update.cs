using LinqToDB;

namespace DataModel
{
	public partial class Profile
	{
		public bool Update(DevKitDB db, ref bool bNameChanged, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "O nome do perfil de usuário já existe";
				return false;
			}

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemProfileUpdate,
				nuType = EnumAuditType.Profile,
				fkTarget = this.id				
			}.
			Create(db, TrackChanges(db, ref bNameChanged), "");

			db.Update(this);

			logs = LoadLogs(db);

			return true;
		}
	}
}
