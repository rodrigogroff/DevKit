using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class TaskFlowFilter
	{
		public int skip, take;
		public string busca;

		public long? fkTaskType;		
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class TaskFlow
	{
		public IQueryable<TaskFlow> ComposedFilters(DevKitDB db, TaskFlowFilter filter)
		{
			var query = from e in db.TaskFlows select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public TaskFlow LoadAssociations(DevKitDB db)
		{
			return this;
		}		
	}
}
