using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace DataModel
{
	public partial class DevKitDB
	{				
		public User currentUser = null;

        public bool ValidateUser()
        {
            if (currentUser == null)
                currentUser = (from ne in User
                               where ne.stLogin.ToUpper() == Thread.CurrentPrincipal.Identity.Name.ToUpper()
                               select ne).
                               FirstOrDefault();

            if (currentUser == null)
                return false;
            
            return true;
        }
        
		public List<long?> GetCurrentUserProjects()
		{
			return (from e in ProjectUser
					where e.fkUser == currentUser.id
					select e.fkProject).
					ToList();
		}
    }
}
