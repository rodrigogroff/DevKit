using LinqToDB;
using System.Linq;

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
		public IQueryable<TaskCategory> ComposedFilters(DevKitDB db, TaskCategoryFilter filter)
		{
			var query = from e in db.TaskCategories select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}
	}
}
