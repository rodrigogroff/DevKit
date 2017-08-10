using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{
		public Project LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			var mdlUser = db.GetUser(this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			users = LoadUsers(db);
			phases = LoadPhases(db);
			sprints = LoadSprints(db);
			logs = LoadLogs(db);

			return this;
		}
		
		List<ProjectUser> LoadUsers(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lst = (from e in db.ProjectUser
                       where e.fkProject == id
                       select e).
				       OrderBy(t => t.id).
				       ToList();

			foreach (var item in lst)
			{
				item.stUser = db.GetUser(item.fkUser).stLogin;
				item.sdtJoin = item.dtJoin?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		List<ProjectPhase> LoadPhases(DevKitDB db)
		{
			return (from e in db.ProjectPhase where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();
		}

		List<ProjectSprint> LoadSprints(DevKitDB db)
		{
			var resp = (from e in db.ProjectSprint where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in resp)
				item.LoadAssociations(db);

			return resp;
		}

		List<ProjectLog> LoadLogs(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lstLogs = (from e in db.AuditLog
						   where e.nuType == EnumAuditType.Project
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.User on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<ProjectLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new ProjectLog
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
