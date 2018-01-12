using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class EstadoFilter : BaseFilter
	{
        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");

            return ret.ToString();
        }
    }
	
	public partial class Estado
	{
		public EstadoReport ComposedFilters(DevKitDB db, EstadoFilter filter)
		{
			var user = db.currentUser;
			
			var query = from e in db.Estado select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stNome.ToUpper().Contains(filter.busca) select e;

            var count = query.Count();

			query = query.OrderBy(y => y.stNome);
            
            return new EstadoReport
            {
                count = count,
                results = query.Skip(filter.skip).Take(filter.take).ToList()
            };
        }
	}
}
