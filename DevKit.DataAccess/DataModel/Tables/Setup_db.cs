using LinqToDB;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Setup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			db.Update(this);

			return true;
		}
	}
}
