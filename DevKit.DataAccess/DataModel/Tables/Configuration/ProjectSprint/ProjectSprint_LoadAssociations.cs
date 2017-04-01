using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprint
	{
		public ProjectSprint LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			sdtEnd = dtEnd?.ToString(setup.stDateFormat);
			sfkProject = db.Project(fkProject)?.stName;
			sfkPhase = db.ProjectPhase(fkPhase)?.stName;

			versions = LoadVersions(db);

			return this;
		}

		List<ProjectSprintVersion> LoadVersions(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprintVersions where e.fkSprint == id select e).
				OrderByDescending(t => t.id).
				ToList();

			return lst;
		}
	}
}
