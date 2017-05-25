using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprint
	{
		public ProjectSprint LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			sfkProject = db.GetProject(fkProject)?.stName;
			sfkPhase = db.GetProjectPhase(fkPhase)?.stName;

			versions = LoadVersions(db);
			logs = LoadLogs(db);

			return this;
		}

		List<ProjectSprintVersion> LoadVersions(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprintVersion
						where e.fkSprint == id
						select e).
						OrderByDescending(t => t.id).
						ToList();

			var _enum = new EnumVersionState();

			foreach (var item in lst)
				item.sfkVersionState = _enum.Get((long)item.fkVersionState).stName;

			return lst;
		}

		List<SprintLog> LoadLogs(DevKitDB db)
		{
			var setup = db.GetSetup();

			var lstLogs = (from e in db.AuditLog
						   where e.nuType == EnumAuditType.Sprint
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.User on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<SprintLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new SprintLog
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
