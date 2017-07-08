using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TaskTypeAccumulatorFilter : BaseFilter
    {
		public long? fkTaskCategory;

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

            if (fkTaskCategory != null)
                ret.Append(fkTaskCategory);

            return ret.ToString();
        }
    }

	public partial class TaskTypeAccumulator
	{
		public TaskTypeAccumulatorReport ComposedFilters(DevKitDB db, TaskTypeAccumulatorFilter filter)
		{
			var query = from e in db.TaskTypeAccumulator select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);

            return new TaskTypeAccumulatorReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
	}
}
