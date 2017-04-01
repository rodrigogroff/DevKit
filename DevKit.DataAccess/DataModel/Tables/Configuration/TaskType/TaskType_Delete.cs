using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Tasks where e.fkTaskType == id select e).Any())
			{
				resp = "This task type is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			db.Delete(this);
		}
	}
}
