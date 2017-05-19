using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TaskTypeFilter
	{
		public int skip, take;
		public string busca;
		public long? fkProject;
		public bool? managed, condensed, kpa;
	}

	public partial class TaskType
	{
		public List<TaskType> ComposedFilters(DevKitDB db, ref int count, TaskTypeFilter filter)
		{
			var lstUserProjetcs = db.GetCurrentUserProjects();

			var query = from e in db.TaskTypes select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject.Equals(filter.fkProject) select e;

			if (lstUserProjetcs.Count() > 0)
				query = from e in query where lstUserProjetcs.Contains(e.fkProject) select e;

			if (filter.managed != null)
				query = from e in query where e.bManaged == filter.managed select e;

			if (filter.condensed != null)
				query = from e in query where e.bCondensedView == filter.condensed select e;

			if (filter.kpa != null)
				query = from e in query where e.bKPA == filter.kpa select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
