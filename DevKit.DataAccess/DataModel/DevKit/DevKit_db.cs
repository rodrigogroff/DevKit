using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class DevKitDB
	{				
		public Hashtable Cache = new Hashtable();

        Setup _setup = null;

        public User currentUser = null;
        public Credenciado currentCredenciado = null;

        public List<long?> GetCurrentUserProjects()
        {
            if (currentUser != null)
                return (from e in ProjectUser
                        where e.fkUser == currentUser.id
                        select e.fkProject).
                    ToList();
            else
                return null;
        }
    }
}
