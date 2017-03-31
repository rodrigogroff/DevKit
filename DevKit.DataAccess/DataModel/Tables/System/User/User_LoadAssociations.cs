﻿using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
		public User LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sdtLastLogin = dtLastLogin?.ToString(setup.stDateFormat);
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			profile = db.Profile(fkProfile);

			phones = LoadPhones(db);
			emails = LoadEmails(db);

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
	}
}
