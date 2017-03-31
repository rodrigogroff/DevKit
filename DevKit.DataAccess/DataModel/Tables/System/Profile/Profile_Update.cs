using LinqToDB;

namespace DataModel
{
	public partial class Profile
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "The name '" + stName + "' is already taken";
				return false;
			}

			db.Update(this);

			return true;
		}
	}
}
