using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class MedicoFilter : BaseFilter
    {
		public long? nuCodigo;
    }

    public partial class Medico
    {
        public MedicoReport ComposedFilters(DevKitDB db, MedicoFilter filter)
        {
            var query = from e in db.Medico
                        select e;

            var count = query.Count();

            query = query.OrderBy(y => y.stNome);

            return new MedicoReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
