using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ProfileFilter
	{
		public int skip, take;				
		public string busca, stPermission;
		public long? fkUser;
	}

	public partial class Profile
	{
		public List<Profile> ComposedFilters(DevKitDB db, ref int count, ProfileFilter filter)
		{
			var query = from e in db.Profiles select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.stPermission != null)
				query = from e in query where e.stPermissions.ToUpper().Contains("||" + filter.stPermission) select e;

			if (filter.fkUser != null)
			{
				query = from e in query
						join eUser in db.Users on e.id equals eUser.fkProfile
						where eUser.id == filter.fkUser
						where eUser.fkProfile == e.id
						select e;
			}

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
