using DataModel;
using LinqToDB;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class Util
	{
		string identityName = Thread.CurrentPrincipal.Identity.Name.ToUpper();

		public User GetCurrentUser(DevKitDB db)
		{
			return (from ne in db.Users
					where ne.stLogin.ToUpper() == identityName
					select ne).FirstOrDefault();
		}

		public List<long?> GetCurrentUserProjects(DevKitDB db)
		{
			var user = (from ne in db.Users
						where ne.stLogin.ToUpper() == identityName
						select ne).FirstOrDefault();

			return (from e in db.ProjectUsers
					where e.fkUser == user.id
					select e.fkProject).
					ToList();
		}

		public List<long?> GetCurrentUserProjects(DevKitDB db, long userId )
		{
			return (from e in db.ProjectUsers
					where e.fkUser == userId
					select e.fkProject).
					ToList();
		}
	}
}