using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class EspecialidadeFilter : BaseFilter
    {
		public long? nuCodigo;
    }

    public partial class Especialidade
    {
        public EspecialidadeReport ComposedFilters(DevKitDB db, EspecialidadeFilter filter)
        {
            var query = from e in db.Especialidade
                        select e;

            var count = query.Count();

            query = query.OrderBy(y => y.id);

            return new EspecialidadeReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
