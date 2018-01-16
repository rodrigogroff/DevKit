using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
		bool CheckDuplicate(User item, DevKitDB db)
		{
			var query = from e in db.User select e;

			if (item.bActive != null)
				query = from e in query where e.bActive == item.bActive select e;

			if (item.stLogin != null)
			{
				var _stLogin = item.stLogin.ToUpper();
				query = from e in query where e.stLogin.ToUpper().Contains(_stLogin) select e;
			}
				
			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Login já utilizado";
				return false;
			}

			if (stLogin.Contains(" "))
			{
				resp = "Login inválido";
				return false;
			}

            fkEmpresa = user.fkEmpresa;
			stPassword = stLogin;
			dtCreation = DateTime.Now;
            
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.SystemUserCreate,
				nuType = EnumAuditType.User,
				fkTarget = this.id
			}.
			Create(db, "Novo usuário: " + stLogin, "");            

			return true;
		}
	}
}
