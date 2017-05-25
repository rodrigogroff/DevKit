using LinqToDB;
using System.Collections.Generic;
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
		public List<TaskTypeAccumulator> ComposedFilters(DevKitDB db, ref int count, TaskAccumulatorFilter filter)
		{
			var query = from e in db.TaskTypeAccumulator select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
