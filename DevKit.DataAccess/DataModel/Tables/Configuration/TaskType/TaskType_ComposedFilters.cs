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
	}

	public partial class TaskType
	{
		public List<TaskType> ComposedFilters(DevKitDB db, ref int count, List<long?> lstUserProjetcs, TaskTypeFilter filter)
		{
			var query = from e in db.TaskTypes select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject.Equals(filter.fkProject) select e;

			if (lstUserProjetcs.Count() > 0)
				query = from e in query where lstUserProjetcs.Contains(e.fkProject) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
