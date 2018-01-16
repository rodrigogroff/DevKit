using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ProjectFilter : BaseFilter
    {
		public long? fkUser;

		public string ExportString()
		{
			var ret = "";

			ret += "Skip: " + skip + ";";
			ret += "take: " + take + ";";
            ret += "empresa: " + fkEmpresa + ";";

            if (fkUser!= null)
				ret += "fkUser: " + fkUser + ";";

			return ret;
		}

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

            if (fkUser != null)
                ret.Append(fkUser);
            ret.Append(",");

            return ret.ToString();
        }
    }

    public partial class Project
    {
        public ProjectReport ComposedFilters(DevKitDB db, ProjectFilter filter)
        {
            var user = db.currentUser;
            var lstUserProjects = db.GetCurrentUserProjects();

            var query = from e in db.Project
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

            if (lstUserProjects.Count() > 0)
                query = from e in query where lstUserProjects.Contains(e.id) select e;

            if (filter.fkUser != null)
            {
                query = from e in query
                        join eUser in db.ProjectUser on e.id equals eUser.fkProject
                        where e.id == eUser.fkProject
                        where eUser.fkUser == filter.fkUser
                        select e;
            }

            var count = query.Count();

            query = query.OrderBy(y => y.stName);

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.ProjectListing,
                nuType = EnumAuditType.Project
            }.
            Create(db, filter.ExportString(), "count: " + count);
            
            return new ProjectReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
    }
}
