using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class DevKitDB
	{				
		public User currentUser = null;

		public User GetCurrentUser(long fkUser)
		{
			if (currentUser == null)
				currentUser = (from ne in User
							   where ne.id == fkUser
                               select ne).FirstOrDefault();

			return currentUser;
		}

		public List<long?> GetCurrentUserProjects(long fkUser)
		{
			if (currentUser == null)
				currentUser = GetCurrentUser(fkUser);

			return (from e in ProjectUser
					where e.fkUser == currentUser.id
					select e.fkProject).
					ToList();
		}
    }
}
