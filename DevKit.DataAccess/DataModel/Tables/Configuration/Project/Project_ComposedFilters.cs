using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ProjectFilter
	{
		public int skip, take;
		public string busca;
	}
	
	public partial class Project
	{
		public IQueryable<Project> ComposedFilters(DevKitDB db, ProjectFilter filter, List<long?> lstUserProjects)
		{
			var query = from e in db.Projects select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.id) select e;

			return query;
		}
	}
}
