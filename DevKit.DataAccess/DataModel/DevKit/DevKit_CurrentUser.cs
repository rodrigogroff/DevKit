using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DataModel
{
	public partial class DevKitDB
	{				
		public User currentUser = null;

		public User GetCurrentUser()
		{
			if (currentUser == null)
				currentUser = (from ne in User
							   where ne.stLogin.ToUpper() == Thread.CurrentPrincipal.Identity.Name.ToUpper()
							   select ne).FirstOrDefault();

			return currentUser;
		}

		public List<long?> GetCurrentUserProjects()
		{
			if (currentUser == null)
				currentUser = GetCurrentUser();

			return (from e in ProjectUser
					where e.fkUser == currentUser.id
					select e.fkProject).
					ToList();
		}

		public List<long?> GetCurrentUserProjects(long userId)
		{
			return (from e in ProjectUser
					where e.fkUser == userId
					select e.fkProject).
					ToList();
		}
    }
}
