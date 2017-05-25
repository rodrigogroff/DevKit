using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ProjectSprintVersionFilter
	{
		public int skip, take;
		public string busca;
		public int? fkSprint;		
	}

	public partial class ProjectSprintVersion
	{
		public List<ProjectSprintVersion> ComposedFilters(DevKitDB db, ref int count, ProjectSprintVersionFilter filter)
		{
			var query = from e in db.ProjectSprintVersion select e;

			if (filter.fkSprint != null)
				query = from e in query where e.fkSprint == filter.fkSprint select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();
			
			return results;
		}
	}
}
