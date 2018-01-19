using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class EmpresaFilter : BaseFilter
    {
		public long? nuCodigo;
    }

    public partial class Empresa
    {
        public EmpresaReport ComposedFilters(DevKitDB db, EmpresaFilter filter)
        {
            var query = from e in db.Empresa
                        select e;

            var count = query.Count();

            query = query.OrderBy(y => y.id);

            return new EmpresaReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
