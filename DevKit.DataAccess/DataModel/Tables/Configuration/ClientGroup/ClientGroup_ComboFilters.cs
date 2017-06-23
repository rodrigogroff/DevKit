using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class ClientGroup
	{
		public List<ClientGroup> ComboFilters(DevKitDB db, string searchItem)
		{
			var query = from e in db.ClientGroup select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            return query.ToList();
        }
	}
}
