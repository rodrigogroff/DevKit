using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TaskCheckPointFilter : BaseFilter
    {
		public long? fkCategory;		
	}

	public partial class TaskCheckPoint
	{
		public List<TaskCheckPoint> ComposedFilters(DevKitDB db, ref int count, TaskCheckPointFilter filter)
		{
			var query = from e in db.TaskCheckPoint select e;

			if (filter.fkCategory != null)
				query = from e in query where e.fkCategory == filter.fkCategory select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();
			
			return results;
		}
	}
}
