using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
		public List<User> Loader(DevKitDB db, List<User> results)
        {
            var lstIdsProfile = results.Select(y => y.fkProfile).Distinct().ToList();
                
            if (lstIdsProfile.Any())
            {
                var lst = (from e in db.Profile where lstIdsProfile.Contains(e.id) select e).ToList();
                foreach (var item in lst) db.Cache["Profile" + item.id] = item;
            }
                        
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
