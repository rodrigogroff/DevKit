using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class Cidade
	{
		public ComboReport ComboFilters(DevKitDB db, CidadeFilter filter)
		{
			var query = from e in db.Cidade select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query
                        where e.stNome.ToUpper().Contains(filter.busca)
                        select e;

            if (filter.fkEstado != null)
                query = from e in query
                        where e.fkEstado == filter.fkEstado
                        select e;

            query = from e in query orderby e.stNome select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse
                {
                    id = e.id,
                    stName = e.stNome }).
                    ToList()
            };            
        }
	}
}
