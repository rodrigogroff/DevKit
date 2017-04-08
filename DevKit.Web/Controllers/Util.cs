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

		public User currentUser = null;

		public User GetCurrentUser(DevKitDB db)
		{
			if (currentUser == null)
				currentUser = (from ne in db.Users
							   where ne.stLogin.ToUpper() == identityName
							   select ne).FirstOrDefault();

			return currentUser;
		}

		public List<long?> GetCurrentUserProjects(DevKitDB db)
		{
			if (currentUser == null)
				currentUser = GetCurrentUser(db);

			return (from e in db.ProjectUsers
					where e.fkUser == currentUser.id
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