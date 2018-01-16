using LinqToDB;
using System.Linq;

namespace DataModel
{
    public partial class Project
    {
        public ComboReport ComboFilters(DevKitDB db, ProjectFilter filter)
        {
            var lstUserProjects = db.GetCurrentUserProjects();

            var query = from e in db.Project
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

            if (lstUserProjects.Count() > 0)
                query = from e in query where lstUserProjects.Contains(e.id) select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
    }
}
