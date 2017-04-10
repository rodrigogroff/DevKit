﻿
namespace DataModel
{
	public partial class User
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.User(this.id);
			
			if(oldEntity.stLogin != this.stLogin)
				ret += "Login: " + oldEntity.stLogin + " => " + this.stLogin + "; ";

			if (oldEntity.bActive != this.bActive)
				ret += "Active: " + oldEntity.bActive + " => " + this.bActive + "; ";

			if (oldEntity.fkProfile != this.fkProfile)
				ret += "Profile: " + db.Profile(oldEntity.fkProfile) + " => " + db.Profile(this.fkProfile) + "; ";

			return ret;
		}
	}
}
