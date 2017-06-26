using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	public class TaskCheckPointFilter : BaseFilter
    {
		public long? fkCategory;

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

            if (fkCategory != null)
                ret.Append(busca + ",");

            _exportResults = ret.ToString();

            return _exportResults;
        }
    }

	public partial class TaskCheckPoint
	{
		public TaskCheckPointReport ComposedFilters(DevKitDB db, TaskCheckPointFilter filter)
		{
			var query = from e in db.TaskCheckPoint select e;

			if (filter.fkCategory != null)
				query = from e in query where e.fkCategory == filter.fkCategory select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

            query = from e in query orderby e.stName select e;
            
            return new TaskCheckPointReport
            {
                count = count,
                results = (query.Skip(filter.skip).Take(filter.take)).ToList()
            };
        }
	}
}
