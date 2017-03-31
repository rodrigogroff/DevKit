using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class ProjectVersionFilter
	{
		public int skip, take;
		public string busca;

		public int? fkSprint;		
	}

	public partial class ProjectSprintVersion
	{
		public IQueryable<ProjectSprintVersion> ComposedFilters(DevKitDB db, ProjectVersionFilter filter)
		{
			var query = from e in db.ProjectSprintVersions select e;

			if (filter.fkSprint != null)
				query = from e in query where e.fkSprint == filter.fkSprint select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}
	}
}
