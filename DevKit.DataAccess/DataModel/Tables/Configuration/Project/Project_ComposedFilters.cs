using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ProjectFilter
	{
		public int skip, take;
		public string busca;
		public long? fkUser;

		public string ExportString()
		{
			var ret = "";

			ret += "Skip: " + skip + " ";
			ret += "take: " + take + " ";

			if (fkUser!= null)
				ret += "fkUser: " + fkUser + " ";

			return ret;
		}
	}
	
	public partial class Project
	{
		public List<Project> ComposedFilters(DevKitDB db, ref int count, ProjectFilter filter)
		{
			var user = db.GetCurrentUser();
			var lstUserProjects = db.GetCurrentUserProjects();

			var query = from e in db.Projects select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.id) select e;

			if (filter.fkUser != null)
			{
				query = from e in query
						join eUser in db.ProjectUsers on e.id equals eUser.fkProject
						where e.id == eUser.fkProject
						where eUser.fkUser == filter.fkUser
						select e;
			}

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ProjectListing,
				nuType = EnumAuditType.Project
			}.
			Create(db, filter.ExportString(), "count: " + count);

			return results;
		}
	}
}
