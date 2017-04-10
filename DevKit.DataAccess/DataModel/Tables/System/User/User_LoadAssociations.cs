using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
		public User LoadAssociations(DevKitDB db, bool simplifyed = false)
		{
			profile = db.Profile(fkProfile);

			if (simplifyed)
				return this;

			var setup = db.Setup();

			sdtLastLogin = dtLastLogin?.ToString(setup.stDateFormat);
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			phones = LoadPhones(db);
			emails = LoadEmails(db);
			logs = LoadLogs(db);

			return this;
		}
		
		List<UserPhone> LoadPhones(DevKitDB db)
		{
			return (from e in db.UserPhones where e.fkUser == id select e).
				OrderBy(t => t.stPhone).
				ToList();
		}

		List<UserEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.UserEmails where e.fkUser == id select e).
				OrderByDescending(t => t.id).
				ToList();
		}

		List<UserLog> LoadLogs(DevKitDB db)
		{
			var setup = db.Setup();

			var lstLogs = (from e in db.AuditLogs
						   where e.nuType == EnumAuditType.User
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.Users on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<UserLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new UserLog
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
