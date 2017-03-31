using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class TaskAccumulatorFilter
	{
		public int skip, take;
		public string busca;

		public long? fkTaskCategory;		
	}

	public partial class TaskTypeAccumulator
	{
		public IQueryable<TaskTypeAccumulator> ComposedFilters(DevKitDB db, TaskAccumulatorFilter filter)
		{
			var query = from e in db.TaskTypeAccumulators select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}
	}
}
