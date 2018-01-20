using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TUSSFilter : BaseFilter
    {
		public long? nuCodigo;
    }

    public partial class TUSS
    {
        public TUSSReport ComposedFilters(DevKitDB db, TUSSFilter filter)
        {
            var query = from e in db.TUSS
                        select e;

            var count = query.Count();

            query = query.OrderBy(y => y.stProcedimento);

            return new TUSSReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
