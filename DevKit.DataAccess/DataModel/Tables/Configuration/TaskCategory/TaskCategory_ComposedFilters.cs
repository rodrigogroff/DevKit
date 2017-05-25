using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TaskCategoryFilter
	{
		public int skip, take;
		public string busca;

		public long? fkTaskType;		
	}

	public partial class TaskCategory
	{
		public List<TaskCategory> ComposedFilters(DevKitDB db, ref int count, TaskCategoryFilter filter)
		{
			var query = from e in db.TaskCategory select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();
			
			return results;
		}
	}
}
