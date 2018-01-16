using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprintVersion
	{
        public ComboReport ComboFilters(DevKitDB db, ProjectSprintVersionFilter filter)
        {
            var query = from e in db.ProjectSprintVersion
                        join spr in db.ProjectSprint on e.fkSprint equals spr.id
                        join proj in db.Project on spr.fkProject equals proj.id
                        where proj.fkEmpresa == filter.fkEmpresa
                        select e;

            if (filter.busca != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca)
                        select e;

            if (filter.fkSprint != null)
                query = from e in query
                        where e.fkSprint == filter.fkSprint
                        select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
    }
}
