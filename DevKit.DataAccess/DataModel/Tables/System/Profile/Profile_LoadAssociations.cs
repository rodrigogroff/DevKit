using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Profile
	{
		public Profile LoadAssociations(DevKitDB db)
		{
			if ( stPermissions != null && stPermissions.Length > 0)
				qttyPermissions = stPermissions.Split('|').Length / 2;
			
			Users = LoadUsers(db);
						
			return this;
		}

		List<User> LoadUsers(DevKitDB db)
		{
			return (from e in db.Users where e.fkProfile == this.id select e).ToList();
		}
	}
}
