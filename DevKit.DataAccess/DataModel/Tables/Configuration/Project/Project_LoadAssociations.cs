using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

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

		List<ProjectSprint> LoadSprints(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprints where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			return lst;
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
			var setup = db.Setup();

			var lst = (from e in db.ProjectPhases where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.sdtStart = item.dtStart?.ToString(setup.stDateFormat);
				item.sdtEnd = item.dtEnd?.ToString(setup.stDateFormat);
			}

			return lst;
		}
	}
}
