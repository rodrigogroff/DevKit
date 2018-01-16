using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
    public partial class Profile
    {
        public ComboReport ComboFilters(DevKitDB db, ProfileFilter filter )
        {
            var query = from e in db.Profile
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (filter.busca != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca)
                        select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
    }
}
