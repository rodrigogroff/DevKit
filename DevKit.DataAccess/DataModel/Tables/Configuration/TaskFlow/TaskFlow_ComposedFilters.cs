using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class TaskFlowFilter
	{
		public int skip, take;
		public string busca;

		public long?	fkTaskType,
						fkTaskCategory;
	}

	public partial class TaskFlow
	{
		public List<TaskFlow> ComposedFilters(DevKitDB db, ref int count, TaskFlowFilter filter)
		{
			var query = from e in db.TaskFlows select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.nuOrder);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
