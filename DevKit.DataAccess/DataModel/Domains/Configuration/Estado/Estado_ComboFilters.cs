using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Estado
	{
		public ComboReport ComboFilters(DevKitDB db, string searchItem)
		{
			var query = from e in db.Estado select e;

            if (searchItem != "")
                query = from e in query
                        where e.stNome.ToUpper().Contains(searchItem)
                        select e;

            query = from e in query orderby e.stNome select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stNome }).ToList()
            };            
        }
	}
}
