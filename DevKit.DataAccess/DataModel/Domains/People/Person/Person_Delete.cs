using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Person
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.PersonPhone where e.fkPerson == id select e))
				db.Delete(item);

			foreach (var item in (from e in db.PersonEmail where e.fkPerson == id select e))
				db.Delete(item);

			db.Delete(this);
		}
	}
}
