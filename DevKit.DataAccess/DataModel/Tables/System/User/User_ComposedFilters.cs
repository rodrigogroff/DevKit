using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class UserFilter
	{
		public int skip, take;
		public string busca;
		public bool? ativo;
		public long? fkPerfil;
	}

	public partial class User
	{
		public IQueryable<User> ComposedFilters(DevKitDB db, ref int count, UserFilter filter)
		{
			var query = from e in db.Users select e;

			if (filter.ativo != null)
				query = from e in query where e.bActive == filter.ativo select e;

			if (filter.busca != null)
				query = from e in query where e.stLogin.ToUpper().Contains(filter.busca) select e;

			if (filter.fkPerfil != null)
				query = from e in query where e.fkProfile == filter.fkPerfil select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
