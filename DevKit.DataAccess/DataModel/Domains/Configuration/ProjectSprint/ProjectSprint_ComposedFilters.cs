using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ProjectSprintFilter : BaseFilter
    {
		public long? fkProject,
                     fkPhase;

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

            if (fkPhase != null)
                ret.Append(fkPhase);
            ret.Append(",");
            
            return ret.ToString();
        }
    }

	public partial class ProjectSprint
	{
		public ProjectSprintReport ComposedFilters(DevKitDB db, ProjectSprintFilter filter)
		{
			var lstUserProjects = db.GetCurrentUserProjects();

            var query = from e in db.ProjectSprint
                        join proj in db.Project on e.fkProject equals proj.id
                        where proj.fkEmpresa == filter.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.fkProject) select e;

			if (filter.fkPhase != null)
				query = from e in query where e.fkPhase == filter.fkPhase select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName).ThenBy(y => y.fkProject).ThenBy(i => i.fkPhase);

            return new ProjectSprintReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
    }
}
