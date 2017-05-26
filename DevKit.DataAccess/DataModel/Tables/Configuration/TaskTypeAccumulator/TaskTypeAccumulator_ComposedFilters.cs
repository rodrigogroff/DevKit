using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class TaskTypeAccumulatorFilter
	{
		public int skip, take;
		public string busca;

		public long? fkTaskCategory;		
	}

	public partial class TaskTypeAccumulator
	{
		public List<TaskTypeAccumulator> ComposedFilters(DevKitDB db, ref int count, TaskTypeAccumulatorFilter filter)
		{
			var query = from e in db.TaskTypeAccumulator select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

            return Loader(db, (query.Skip(() => filter.skip).Take(() => filter.take)).ToList(), true);
        }
	}
}
