using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class CidadeFilter : BaseFilter
	{
        public long? fkEstado = 0;

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");
            ret.Append(fkEstado + ",");

            return ret.ToString();
        }
    }
	
	public partial class Cidade
	{
		public CidadeReport ComposedFilters(DevKitDB db, CidadeFilter filter)
		{
			var user = db.currentUser;
			
			var query = from e in db.Cidade select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stNome.ToUpper().Contains(filter.busca) select e;

            if (filter.fkEstado != null)
                query = from e in query where e.fkEstado == filter.fkEstado select e;

            var count = query.Count();

			query = query.OrderBy(y => y.stNome);
            
            return new CidadeReport
            {
                count = count,
                results = query.Skip(filter.skip).Take(filter.take).ToList()
            };
        }
	}
}
