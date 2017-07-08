using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Profile
	{
		public List<Profile> Loader(DevKitDB db, List<Profile> results, bool precached)
        {
            if (precached)
            {
                
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
