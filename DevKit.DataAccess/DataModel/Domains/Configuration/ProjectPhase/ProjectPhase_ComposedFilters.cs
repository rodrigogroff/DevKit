using LinqToDB;
using System.Linq;
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

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");
            ret.Append(fkEmpresa + ",");

            if (fkProject != null)
                ret.Append(fkProject);
            ret.Append(",");

            return ret.ToString();
        }
    }
    
    public partial class ProjectPhase
	{
		public ProjectPhaseReport ComposedFilters(DevKitDB db, ProjectPhaseFilter filter)
		{
			var query = from e in db.ProjectPhase
                        join proj in db.Project on e.fkProject equals proj.id
                        where proj.fkEmpresa == filter.fkEmpresa
                        select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);

            return new ProjectPhaseReport
            {
                count = count,
                results = (query.Skip(filter.skip).Take(filter.take)).ToList()
            };
		}
	}
}
