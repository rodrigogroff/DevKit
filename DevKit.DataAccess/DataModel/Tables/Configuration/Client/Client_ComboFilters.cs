using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
	public partial class Client
	{
		public List<Client> ComboFilters(DevKitDB db, string searchItem)
		{
			var query = from e in db.Client select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            return query.ToList();
        }
	}
}
