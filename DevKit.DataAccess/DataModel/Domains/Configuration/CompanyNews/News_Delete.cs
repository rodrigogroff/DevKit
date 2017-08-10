using LinqToDB;

namespace DataModel
{
	public partial class CompanyNews
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return true;
		}

		public void Delete(DevKitDB db)
		{
			var user = db.currentUser;

			db.Delete(this);
		}
	}
}
