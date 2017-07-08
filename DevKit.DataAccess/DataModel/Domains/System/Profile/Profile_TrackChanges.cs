using System.Collections;

namespace DataModel
{
	public partial class Profile
	{
		public string TrackChanges(DevKitDB db, ref bool bNameChanged)
		{
			var ret = "";

			var oldEntity = db.GetProfile(this.id);

			if (oldEntity.stName != this.stName )
			{
                ret += "Name: " + oldEntity.stName + " => " + this.stName + "; ";
                bNameChanged = true;
            }
			
			Hashtable hshOldPerms = new Hashtable(),
						hshNewPerms = new Hashtable();

			var oldPerms = oldEntity.stPermissions.Replace("||", "|").Split('|');
			var newPerms = stPermissions.Replace("||", "|").Split('|');

			foreach (var perm in oldPerms) hshOldPerms[perm] = true;
			foreach (var perm in newPerms) hshNewPerms[perm] = true;

			foreach (var perm in oldPerms)
				if (perm != "")
					if (hshNewPerms[perm] == null)
						ret += "Permission removed: " + perm.Substring(0,3) + "; ";

			foreach (var perm in newPerms)
				if (perm != "")
					if (hshOldPerms[perm] == null)
						ret += "Permission added: " + perm.Substring(0, 3) + "; ";

			return ret;
		}
	}
}
