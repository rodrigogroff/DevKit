using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class LoteGraficaFilter : BaseFilter
    {
		public long? nuCodigo;
    }

    public partial class LoteGrafica
    {
        public LoteGraficaReport ComposedFilters(DevKitDB db, LoteGraficaFilter filter)
        {
            var query = from e in db.LoteGrafica
                        select e;

            if (filter.nuCodigo != null)
                query = from e in query
                        where e.nuCodigo == filter.nuCodigo
                        select e;

            var count = query.Count();

            query = query.OrderBy(y => y.dtLog);

            return new LoteGraficaReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
