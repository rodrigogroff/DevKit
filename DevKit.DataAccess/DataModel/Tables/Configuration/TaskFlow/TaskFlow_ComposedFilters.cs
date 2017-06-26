using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TaskFlowFilter : BaseFilter
    {
		public long? fkTaskType,
					 fkTaskCategory;

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
                ret.Append(fkTaskType + ",");

            ret.Append(",");

            if (fkTaskCategory != null)
                ret.Append(fkTaskCategory);
            
            return ret.ToString();
        }
    }

	public partial class TaskFlow
	{
		public TaskFlowReport ComposedFilters(DevKitDB db, TaskFlowFilter filter)
		{
			var query = from e in db.TaskFlow select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.nuOrder);

            return new TaskFlowReport
            {
                count = count,
                results = query.Skip(filter.skip).Take(filter.take).ToList()
            };
        }
	}
}
