using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class loaderOptionsTaskType
    {
        public bool bLoadProject = false,
                    bLoadCategories = false,
                    bLoadCheckPoints = false,
                    bLoadLogs = false;
    }

    public partial class TaskType
	{
		public List<TaskType> Loader(DevKitDB db, List<TaskType> results, loaderOptionsTaskType options)
        {
            var lstIdsTaskTypes = results.Select(y => y.id).ToList();

            if (options.bLoadProject)
            {
                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();

                if (lstIdsProject.Any())
                {
                    var lst = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Project" + item.id] = item;
                }
            }            

            if (options.bLoadCategories)
            {                
                db.Cache["ListTaskCategory"] = (from e in db.TaskCategory
                                                where lstIdsTaskTypes.Contains((long)e.fkTaskType)
                                                select e).
                                                ToList();                
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db, options); });

            return results;
        }
    }
}
