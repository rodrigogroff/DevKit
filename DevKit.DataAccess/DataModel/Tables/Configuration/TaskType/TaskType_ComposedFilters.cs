using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TaskTypeFilter : BaseFilter
    {
		public long? fkProject;

		public bool? managed,
                     condensed, 
                     kpa;

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

            if (fkProject != null)
                ret.Append(fkProject);
            ret.Append(",");

            if (managed != null)
                ret.Append(managed);
            ret.Append(",");

            if (condensed != null)
                ret.Append(condensed);
            ret.Append(",");

            if (kpa != null)
                ret.Append(kpa);
            ret.Append(",");

            _exportResults = ret.ToString();

            return _exportResults;
        }
    }

	public partial class TaskType
	{
		public TaskTypeReport ComposedFilters(DevKitDB db, TaskTypeFilter filter, loaderOptionsTaskType options)
		{
			var lstUserProjetcs = db.GetCurrentUserProjects();

			var query = from e in db.TaskType select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject.Equals(filter.fkProject) select e;

			if (lstUserProjetcs.Count() > 0)
				query = from e in query where lstUserProjetcs.Contains(e.fkProject) select e;

			if (filter.managed != null)
				query = from e in query where e.bManaged == filter.managed select e;

			if (filter.condensed != null)
				query = from e in query where e.bCondensedView == filter.condensed select e;

			if (filter.kpa != null)
				query = from e in query where e.bKPA == filter.kpa select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);

            var results = query.Skip(filter.skip).
                                Take(filter.take).
                                ToList();

            return new TaskTypeReport
            {
                count = count,
                results = Loader(db, results, options)
            };
        }
	}
}
