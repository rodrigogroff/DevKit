using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Profile
	{
		public Profile LoadAssociations(DevKitDB db)
		{
			if ( stPermissions != null && stPermissions.Length > 0)
				qttyPermissions = stPermissions.Split('|').Length / 2;
			
			users = LoadUsers(db);
			logs = LoadLogs(db);
						
			return this;
		}

		List<User> LoadUsers(DevKitDB db)
		{
			return (from e in db.Users where e.fkProfile == this.id select e).ToList();
		}

		List<ProfileLog> LoadLogs(DevKitDB db)
		{
			var setup = db.Setup();

			var lstLogs = (from e in db.AuditLogs
						   where e.nuType == EnumAuditType.Profile
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending( y=> y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.Users on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<ProfileLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new ProfileLog
				{
					sdtLog = item.dtLog?.ToString(setup.stDateFormat),
					stUser = lstUsers.Where(y => y.id == item.fkUser).FirstOrDefault().stLogin,
					stDetails = item.stLog
				});
			}

			return ret;
		}
	}
}
