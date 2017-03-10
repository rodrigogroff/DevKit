using DataModel;
using LinqToDB;
using System.Linq;
using System.Threading;

namespace DevKit.Web.Controllers
{
	public class Util
	{
		public User GetCurrentUser(DevKitDB db)
		{
			return (from ne in db.Users
					where ne.stLogin.ToUpper() == Thread.CurrentPrincipal.Identity.Name.ToUpper()
					select ne).FirstOrDefault();
		}
	}
}