using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskType
	{
		public List<TaskType> Loader(DevKitDB db, List<TaskType> results, bool precached)
        {
            if (precached)
            {
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
