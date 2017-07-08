using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskClient> LoadClients(DevKitDB db)
		{
			var ret = (from e in db.TaskClient
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.id).
					   ToList();

			foreach (var item in ret)
				item.sfkClient = db.GetClient(item.fkClient).stName;

			return ret;
		}
	}
}
