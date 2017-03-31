using LinqToDB;
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
		public IQueryable<TaskFlow> ComposedFilters(DevKitDB db, TaskFlowFilter filter)
		{
			var query = from e in db.TaskFlows select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}
	}
}
