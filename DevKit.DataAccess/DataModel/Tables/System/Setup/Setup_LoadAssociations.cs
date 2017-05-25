using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Setup
	{
		public Setup LoadAssociations(DevKitDB db)
		{
			logs = LoadLogs(db);

			return this;
		}

		List<SetupLog> LoadLogs(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lstLogs = (from e in db.AuditLog
						   where e.nuType == EnumAuditType.Setup
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.User on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<SetupLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new SetupLog
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
