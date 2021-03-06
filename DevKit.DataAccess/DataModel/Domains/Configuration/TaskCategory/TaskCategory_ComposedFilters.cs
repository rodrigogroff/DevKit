﻿using LinqToDB;
using System.Linq;
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

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");

            if (fkTaskType != null)
                ret.Append(fkTaskType);

            ret.Append(",");

            return ret.ToString();
        }
    }

	public partial class TaskCategory
	{
		public TaskCategoryReport ComposedFilters(DevKitDB db, TaskCategoryFilter filter)
		{
			var query = from e in db.TaskCategory select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

            var count = query.Count();

            query = query.OrderBy(y => y.stName);

            return new TaskCategoryReport
            {
                count = count,
                results = query.Skip(filter.skip).Take(filter.take).ToList()
            };
        }
	}
}
