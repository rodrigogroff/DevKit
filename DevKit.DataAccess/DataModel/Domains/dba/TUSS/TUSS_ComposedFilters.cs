using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class TUSSFilter : BaseFilter
    {
        public string   codigo, 
                        procedimento;
    }

    public partial class TUSS
    {
        public TUSSReport ComposedFilters(DevKitDB db, TUSSFilter filter)
        {
            if (filter.procedimento != null)
                filter.procedimento = filter.procedimento.ToUpper();

            var query = from e in db.TUSS select e;

            if (!string.IsNullOrEmpty(filter.busca))
            {
                query = from e in query
                        where e.nuCodTUSS.ToString() == filter.busca || e.stProcedimento.Contains(filter.busca)
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.codigo))
            {
                query = from e in query
                        where e.nuCodTUSS.ToString() == filter.codigo
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.procedimento))
            {
                query = from e in query
                        where e.stProcedimento.Contains(filter.procedimento)
                        select e;
            }

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
