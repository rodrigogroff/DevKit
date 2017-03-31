using LinqToDB;

namespace DataModel
{	
	public partial class Task
	{
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return false;
		}

		public void Delete(DevKitDB db)
		{
			db.Delete(this);
		}
	}
}
