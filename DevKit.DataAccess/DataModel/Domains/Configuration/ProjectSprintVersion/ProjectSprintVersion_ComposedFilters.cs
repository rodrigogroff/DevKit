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

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");
            ret.Append(fkEmpresa + ",");

            if (fkSprint != null)
                ret.Append(fkSprint);
            ret.Append(",");
            
            return ret.ToString();
        }
    }

	public partial class ProjectSprintVersion
	{
		public ProjectSprintVersionReport ComposedFilters(DevKitDB db, ProjectSprintVersionFilter filter)
		{
            var query = from e in db.ProjectSprintVersion
                        join spr in db.ProjectSprint on e.fkSprint equals spr.id
                        join pro in db.Project on spr.fkProject equals pro.id
                        where pro.fkEmpresa == filter.fkEmpresa
                        select e;

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
