using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
        public ComboReport ComboFilters(DevKitDB db, string searchItem)
        {
            var query = from e in db.User select e;

            if (searchItem != "")
                query = from e in query
                        where e.stLogin.ToUpper().Contains(searchItem)
                        select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stLogin }).ToList()
            };
        }
    }
}
