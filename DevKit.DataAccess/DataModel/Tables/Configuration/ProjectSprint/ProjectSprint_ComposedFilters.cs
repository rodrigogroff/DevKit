using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ProjectSprintFilter
	{
		public int skip, take;
		public string busca;

		public long? fkProject, fkPhase;		
	}

	public partial class ProjectSprint
	{
		public List<ProjectSprint> ComposedFilters(DevKitDB db, ref int count, ProjectSprintFilter filter)
		{
			var lstUserProjects = db.GetCurrentUserProjects();

			var query = from e in db.ProjectSprints select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.fkProject) select e;

			if (filter.fkPhase != null)
				query = from e in query where e.fkPhase == filter.fkPhase select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName).ThenBy(y => y.fkProject).ThenBy(i => i.fkPhase);
				
			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
