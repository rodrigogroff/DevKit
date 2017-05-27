using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public static class setupTaskType
    {
        public const int TaskTypeEdit = 1,
                         TaskTypeListing = 2;
    }

    public class loaderOptionsTaskType
    {
        public int setup = 0;

        public loaderOptionsTaskType(int choice)
        {
            setup = choice;

            switch (setup)
            {
                case setupTaskType.TaskTypeEdit: Setup_TaskTypeEdit(); break;
                case setupTaskType.TaskTypeListing: Setup_TaskTypeListing(); break;
            }
        }

        public bool bLoadProject = false,
                    bLoadCategories = false,
                    bLoadCheckPoints = false,
                    bLoadLogs = false;

        void Setup_TaskTypeListing()
        {
            bLoadProject = true;            
        }

        void Setup_TaskTypeEdit()
        {
            bLoadProject = true;
            bLoadCategories = true;
            bLoadCheckPoints = true;
            bLoadLogs = true;
        }
    }

    public partial class TaskType
	{
		public List<TaskType> Loader(DevKitDB db, List<TaskType> results, loaderOptionsTaskType options)
        {
            if (options.bLoadProject)
            {
                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();

                if (lstIdsProject.Any())
                {
                    var lst = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Project" + item.id] = item;
                }
            }            
            
            results.ForEach(y => { y = y.LoadAssociations(db, options); });

            return results;
        }
    }
}
