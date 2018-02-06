using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
    public partial class Empresa
    {
        public ComboReport ComboFilters(DevKitDB db, EmpresaFilter filter)
        {
            var query = from e in db.Empresa select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query
                        where e.stNome.ToUpper().Contains(filter.busca)
                        select e;

            query = from e in query orderby e.stNome select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query
                           select new BaseComboResponse
                           {
                               id = e.id,
                               stName = e.nuEmpresa + " - " + e.stNome
                           }).
                    ToList()
            };
        }
    }
}
