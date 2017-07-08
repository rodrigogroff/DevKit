using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskTypeAccumulator
    {
		public List<TaskTypeAccumulator> Loader(DevKitDB db, List<TaskTypeAccumulator> results, bool precached)
        {
            if (precached)
            {
                var lstIdsTaskFlow = results.Select(y => y.fkTaskFlow).Distinct().ToList();

                if (lstIdsTaskFlow.Any())
                {
                    var lst = (from e in db.TaskFlow where lstIdsTaskFlow.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["TaskFlow" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
