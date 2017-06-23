using LinqToDB;
using System.Collections.Generic;
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

            if (fkTaskCategory != null)
                ret.Append(fkTaskCategory);

            ret.Append(",");

            _exportResults = ret.ToString();

            return _exportResults;
        }
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
