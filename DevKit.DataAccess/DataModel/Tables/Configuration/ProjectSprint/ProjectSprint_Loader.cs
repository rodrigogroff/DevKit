using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class ProjectSprint
    {
		public List<ProjectSprint> Loader(DevKitDB db, List<ProjectSprint> results, bool precached)
        {
            if (precached)
            {
                var lstIdsPhase = results.Select(y => y.fkPhase).Distinct().ToList();
                
                if (lstIdsPhase.Any())
                {
                    var lst = (from e in db.ProjectPhase where lstIdsPhase.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["ProjectPhase" + item.id] = item;
                }

                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();

                if (lstIdsProject.Any())
                {
                    var lst = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Project" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
