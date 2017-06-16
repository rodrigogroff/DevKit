using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace DataModel
{
	public partial class DevKitDB
	{				
		public User currentUser = null;

		public List<long?> GetCurrentUserProjects()
		{
			return (from e in ProjectUser
					where e.fkUser == currentUser.id
					select e.fkProject).
					ToList();
		}
    }
}
