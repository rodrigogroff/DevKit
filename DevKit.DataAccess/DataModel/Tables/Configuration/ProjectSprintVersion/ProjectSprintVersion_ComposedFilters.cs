using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ProjectSprintVersionFilter : BaseFilter
    {
		public long? fkSprint;

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

            if (fkSprint != null)
                ret.Append(fkSprint);
            ret.Append(",");

            _exportResults = ret.ToString();

            return _exportResults;
        }
    }

	public partial class ProjectSprintVersion
	{
		public ProjectSprintVersionReport ComposedFilters(DevKitDB db, ProjectSprintVersionFilter filter)
		{
			var query = from e in db.ProjectSprintVersion select e;

			if (filter.fkSprint != null)
				query = from e in query where e.fkSprint == filter.fkSprint select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);

            return new ProjectSprintVersionReport
            {
                count = count,
                results = (query.Skip(filter.skip).Take(filter.take)).ToList()
            };
		}
	}
}
