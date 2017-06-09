using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class CompanyNewsFilter : BaseFilter
    {
		public long? fkProject;

		public string ExportString()
		{
			var ret = "";

			ret += "Skip: " + skip + " ";
			ret += "take: " + take + " ";

			if (fkProject != null)
				ret += "fkProject: " + fkProject + " ";

			return ret;
		}
	}
	
	public partial class CompanyNews
	{
		public List<CompanyNews> ComposedFilters(DevKitDB db, ref int count, CompanyNewsFilter filter)
		{
			var user = db.currentUser;
			var lstUserProjects = db.GetCurrentUserProjects();

			var query = from e in db.CompanyNews select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query
						where e.stTitle.ToUpper().Contains(filter.busca) ||
							  e.stMessage.ToUpper().Contains(filter.busca)
						select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.id) select e;

			if (filter.fkProject != null)
			{
				query = from e in query
						join eUser in db.ProjectUser on e.id equals eUser.fkProject
						where e.id == eUser.fkProject
						where eUser.fkUser == filter.fkProject
						select e;
			}

			count = query.Count();

			query = query.OrderBy(y => y.id);

            return Loader(db, (query.Skip(() => filter.skip).Take(() => filter.take)).ToList(), true);
        }
	}
}
