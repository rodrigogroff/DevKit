using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Profile
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if (stName.ToUpper() == "ADMINISTRATOR")
			{
				resp = "Administrator profile cannot be deleted";
				return false;
			}
			
			if ((from e in db.Users where e.fkProfile == this.id select e).Any())
			{
				resp = "Profile still associated with other users";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db, User user )
		{
			db.Delete(this);

			new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.SystemProfileRemove }.Create(db, "", "");
		}
	}
}
