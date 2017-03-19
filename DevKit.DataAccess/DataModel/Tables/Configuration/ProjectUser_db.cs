using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class ProjectUserFilter
	{
		public long? fkUser;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class ProjectUser
	{
		public IQueryable<ProjectUser> ComposedFilters(DevKitDB db, ProjectUserFilter filter)
		{
			var query = from e in db.ProjectUsers select e;

			if (filter.fkUser != null)
				query = from e in query where e.fkUser == filter.fkUser select e;

			return query;
		}

	}
}
