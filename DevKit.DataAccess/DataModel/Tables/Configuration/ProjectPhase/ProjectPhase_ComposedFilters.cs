using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	public class ProjectPhaseFilter : BaseFilter
    {
		public int? fkProject;

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

            _exportResults = ret.ToString();

            return _exportResults;
        }
    }
    
    public partial class ProjectPhase
	{
		public List<ProjectPhase> ComposedFilters(DevKitDB db, ref int count, ProjectPhaseFilter filter)
		{
			var query = from e in db.ProjectPhase select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();
			
			return results;
		}
	}
}
