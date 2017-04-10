using LinqToDB;

namespace DataModel
{
	public partial class Profile
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.Profile(this.id);

			if (oldEntity.stName != this.stName )
			{
				ret += "Name: " + oldEntity.stName + " => " + this.stName + "; ";
			}
			
			if (oldEntity.stPermissions != this.stPermissions)
			{
				//TODO! // Added, Removed
				ret += "Name: " + oldEntity.stPermissions + " => " + this.stPermissions + "; ";
			}

			return ret;
		}
	}
}
