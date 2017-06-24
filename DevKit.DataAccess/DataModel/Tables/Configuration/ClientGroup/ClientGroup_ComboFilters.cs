using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ClientGroup
	{
		public ComboReport ComboFilters(DevKitDB db, string searchItem)
		{
			var query = from e in db.ClientGroup select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
	}
}
