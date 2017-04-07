using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{
		public Project LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			users = LoadUsers(db);
			phases = LoadPhases(db);
			sprints = LoadSprints(db);

			return this;
		}
		
		List<ProjectUser> LoadUsers(DevKitDB db)
		{
			var setup = db.Setup();

			var lst = (from e in db.ProjectUsers where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.stUser = db.User(item.fkUser).stLogin;
				item.sdtJoin = item.dtJoin?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		List<ProjectPhase> LoadPhases(DevKitDB db)
		{
			return (from e in db.ProjectPhases where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();
		}

		List<ProjectSprint> LoadSprints(DevKitDB db)
		{
			var resp = (from e in db.ProjectSprints where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in resp)
				item.LoadAssociations(db);

			return resp;
		}
	}
}
