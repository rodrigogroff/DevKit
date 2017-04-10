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

			db.Update(this);

			new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.SystemProfileUpdate, nuType = EnumAuditType.Profile }.Create(db, "", "");

			return true;
		}
	}
}
