
namespace DataModel
{
	public partial class User
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetUser(this.id);
			
			if(oldEntity.stLogin != this.stLogin)
				ret += "Login: " + oldEntity.stLogin + " => " + this.stLogin + "; ";

			if (oldEntity.bActive != this.bActive)
				ret += "Active: " + oldEntity.bActive + " => " + this.bActive + "; ";

			if (oldEntity.fkProfile != this.fkProfile)
				ret += "Profile: " + db.GetProfile(oldEntity.fkProfile) + " => " + db.GetProfile(this.fkProfile) + "; ";

			return ret;
		}
	}
}
