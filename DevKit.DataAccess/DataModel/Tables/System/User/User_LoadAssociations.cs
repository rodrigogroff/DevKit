using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
		public User LoadAssociations(DevKitDB db)
		{
			profile = db.GetProfile(fkProfile);
            
			var setup = db.GetSetup();

			sdtLastLogin = dtLastLogin?.ToString(setup.stDateFormat);
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			phones = LoadPhones(db);
			emails = LoadEmails(db);
			logs = LoadLogs(db);

			return this;
		}

        public User ClearAssociations()
        {
            profile = null;
            phones = null;
            emails = null;
            logs = null;

            return this;
        }
		
		List<UserPhone> LoadPhones(DevKitDB db)
		{
			return (from e in db.UserPhone where e.fkUser == id select e).
				OrderBy(t => t.stPhone).
				ToList();
		}

		List<UserEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.UserEmail where e.fkUser == id select e).
				OrderByDescending(t => t.id).
				ToList();
		}

		List<UserLog> LoadLogs(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lstLogs = (from e in db.AuditLog
						   where e.nuType == EnumAuditType.User
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.User on e.fkUser equals eUser.id
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
