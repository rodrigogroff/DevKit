using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ProjectPhaseFilter
	{
		public int skip, take;
		public int? fkProject;
		public string busca;
	}
	
	public partial class ProjectPhase
	{
		public List<ProjectPhase> ComposedFilters(DevKitDB db, ref int count, ProjectPhaseFilter filter)
		{
			var query = from e in db.ProjectPhase select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();
			
			return results;
		}
	}
}
