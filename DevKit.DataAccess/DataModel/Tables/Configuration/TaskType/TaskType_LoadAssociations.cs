using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public TaskType LoadAssociations(DevKitDB db)
		{
			categories = LoadCategories(db);

			project = db.Project(fkProject);
			
			return this;
		}

		List<TaskCategory> LoadCategories(DevKitDB db)
		{
			var lst = (from e in db.TaskCategories where e.fkTaskType == id select e).
				OrderBy(t => t.stAbreviation).ThenBy ( y=> y.stName).
				ToList();

			return lst;
		}
	}
}
