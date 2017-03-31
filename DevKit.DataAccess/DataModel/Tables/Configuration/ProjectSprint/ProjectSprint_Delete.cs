using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprint
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Tasks where e.fkSprint == id select e).Any())
			{
				resp = "This sprint is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.ProjectSprintVersions where e.fkSprint == id select e))
				db.Delete(item);

			db.Delete(this);
		}
	}
}
