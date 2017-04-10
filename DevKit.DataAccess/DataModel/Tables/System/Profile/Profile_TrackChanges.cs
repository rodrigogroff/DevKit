using System.Collections;

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
				Hashtable hshOldPerms = new Hashtable(),
						  hshNewPerms = new Hashtable();

				var oldPerms = oldEntity.stPermissions.Replace("||", "|").Split('|');
				var newPerms = oldEntity.stPermissions.Replace("||", "|").Split('|');

				foreach (var perm in oldPerms) hshOldPerms[perm] = true;
				foreach (var perm in newPerms) hshNewPerms[perm] = true;

				foreach (var perm in oldPerms)
					if (perm != "")
						if (hshNewPerms[perm] == null)
							ret += "Permission removed: " + perm + "; ";

				foreach (var perm in newPerms)
					if (perm != "")
						if (hshOldPerms[perm] == null)
							ret += "Permission added: " + perm + "; ";
			}

			return ret;
		}
	}
}
