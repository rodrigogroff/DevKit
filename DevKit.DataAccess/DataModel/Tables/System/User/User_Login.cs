using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public User Login(DevKitDB db, string login, string password)
		{
			var user = (from e in db.Users
						   where e.stLogin.ToUpper() == login.ToUpper()
						   where e.stPassword.ToUpper() == password.ToUpper()
						   where e.bActive == true
						   select e).
						   FirstOrDefault();

			if (user != null)
			{
				user.LoadAssociations(db, true);

				new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.Login, nuType = EnumAuditType.User }.Create(db, "", "");

				return user;
			}
			
			return null;
		}
	}
}
