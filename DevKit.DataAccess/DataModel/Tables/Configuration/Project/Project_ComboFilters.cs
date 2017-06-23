using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public partial class Project
    {
        public List<Project> ComboFilters(DevKitDB db, ProjectFilter filter)
        {
            var user = db.currentUser;
            var lstUserProjects = db.GetCurrentUserProjects();

            var query = from e in db.Project select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

            if (lstUserProjects.Count() > 0)
                query = from e in query where lstUserProjects.Contains(e.id) select e;

            if (filter.fkUser != null)
            {
                query = from e in query
                        join eUser in db.ProjectUser on e.id equals eUser.fkProject
                        where e.id == eUser.fkProject
                        where eUser.fkUser == filter.fkUser
                        select e;
            }

            return query.ToList();
        }
    }
}
