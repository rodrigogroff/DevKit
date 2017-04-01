using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class ProfileFilter
	{
		public int skip, take;				
		public string busca;
	}

	public partial class Profile
	{
		public IQueryable<Profile> ComposedFilters(DevKitDB db, ref int count, ProfileFilter filter)
		{
			var query = from e in db.Profiles select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
