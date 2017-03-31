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
		public IQueryable<TaskType> ComposedFilters(DevKitDB db, TaskTypeFilter filter, List<long?> lstUserProjetcs)
		{
			var query = from e in db.TaskTypes select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject.Equals(filter.fkProject) select e;

			if (lstUserProjetcs.Count() > 0)
				query = from e in query where lstUserProjetcs.Contains(e.fkProject) select e;

			return query;
		}
	}
}
