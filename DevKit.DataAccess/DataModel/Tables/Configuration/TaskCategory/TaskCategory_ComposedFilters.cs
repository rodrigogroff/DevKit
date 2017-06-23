using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	public class TaskCategoryFilter : BaseFilter
    {
		public long? fkTaskType;

        public string Parameters()
        {
            return Export();
        }

        string _exportResults = "";

        string Export()
        {
            if (_exportResults != "")
                return _exportResults;

            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");

            if (fkTaskType != null)
                ret.Append(fkTaskType);
            ret.Append(",");

            _exportResults = ret.ToString();

            return _exportResults;
        }
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
