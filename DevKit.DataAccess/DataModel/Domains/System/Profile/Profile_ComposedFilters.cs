using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ProfileFilter : BaseFilter
	{
		public string stPermission;
		public long? fkUser;

        public string Parameters()
        {
            return Export();
        }

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");
            ret.Append(stPermission + ",");

            if (fkUser != null)
                ret.Append(fkUser);
            
            return ret.ToString();
        }
    }

	public partial class Profile
	{
		public ProfileReport ComposedFilters(DevKitDB db, ProfileFilter filter)
		{
			var query = from e in db.Profile select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (filter.stPermission != null)
				query = from e in query where e.stPermissions.ToUpper().Contains("||" + filter.stPermission) select e;

			if (filter.fkUser != null)
			{
				query = from e in query
						join eUser in db.User on e.id equals eUser.fkProfile
						where eUser.id == filter.fkUser
						where eUser.fkProfile == e.id
						select e;
			}

			var count = query.Count();

			query = query.OrderBy(y => y.stName);
                        
            return new ProfileReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
	}
}
