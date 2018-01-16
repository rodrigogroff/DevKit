using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
        public ComboReport ComboFilters(DevKitDB db, UserFilter filter)
        {
            var query = from e in db.User
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (filter.busca != "")
                query = from e in query
                        where e.stLogin.ToUpper().Contains(filter.busca)
                        select e;

            query = from e in query orderby e.stLogin select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stLogin }).ToList()
            };
        }
    }
}
