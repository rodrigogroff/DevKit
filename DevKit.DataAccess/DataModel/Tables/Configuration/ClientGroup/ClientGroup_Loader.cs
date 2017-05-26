using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class ClientGroup
	{
		public List<ClientGroup> Loader(DevKitDB db, List<ClientGroup> results, bool precached)
        {
            if (precached)
            {
                var lstIdsUser = results.Select(y => y.fkUser).Distinct().ToList();
                
                if (lstIdsUser.Any())
                {
                    var lst = (from e in db.User where lstIdsUser.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["User" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
